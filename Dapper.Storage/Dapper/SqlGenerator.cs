﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Storage.Attributes;
using Dapper.Storage.Core;
using DapperExtensions;
using DapperExtensions.Sql;

namespace Dapper.Storage.Dapper
{
	using DapperExtensions.Mapper;
	using DialectMapper = Dictionary<StorageType, ISqlDialect>;
	using Parameters = IDictionary<string, object>;

	public class SqlGenerator : SqlGeneratorImpl
	{
		private static class DialectCache
		{
			private static ISqlDialect SqlServer { get; } =
				new SqlServerDialect();
			private static ISqlDialect Postgres { get; } =
				new PostgreSqlDialect();
			private static ISqlDialect Oracle { get; } =
				new OracleDialect();

			public static ISqlDialect GetDialectOrDefault(Type entity)
			{
				if(Cache.TryGetValue(entity, out var dialect))
				{
					return dialect;
				}

				if (entity.HasAttribute(out DialectAttribute attribute))
				{
					dialect = Dialects[attribute.Dialect];
					Cache.Add(entity, dialect);

					return dialect;
				}

				return Postgres;
			}

			private static readonly IDictionary<Type, ISqlDialect> Cache =
				new Dictionary<Type, ISqlDialect>();

			private static readonly DialectMapper Dialects =
				new DialectMapper
				{
					{ StorageType.Postgres, Postgres },
					{ StorageType.Sybase, SqlServer }
				};
		}

		public SqlGenerator(IDapperExtensionsConfiguration configuration) 
			: base(configuration)
		{

		}

		public override string Select(
			IClassMapper map, 
			IPredicate predicate, 
			IList<ISort> sort, 
			Parameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			var dialect = this.GetDialect(map.EntityType);
			var query = new SqlBuilder(dialect);

			var table = this.GetTableName(map);

			var sortingColumns = sort?.Select(column =>
				this.GetColumnName(map, column.PropertyName, false) +
				(column.Ascending ? " ASC" : " DESC"));

			query
				.Select(BuildSelectColumns(map))
				.From(table)
				.Where(predicate?.GetSql(this, parameters))
				.OrderBy(sortingColumns?.AppendStrings());

			return query.Build();
		}

		public override string Count(
			IClassMapper map, 
			IPredicate predicate, 
			Parameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			var dialect = this.GetDialect(map.EntityType);
			var query = new SqlBuilder(dialect);

			var table = this.GetTableName(map);

			query
				.SelectCount()
				.From(table)
				.Where(predicate.GetSql(this, parameters));

			return query.Build();
		}

		public override string Insert(IClassMapper map)
		{
			var columns = map.Properties.Where(p => 
				!(p.Ignored || p.IsReadOnly || 
				  p.KeyType == KeyType.Identity || 
				  p.KeyType == KeyType.TriggerIdentity));

			if (!columns.Any())
			{
				throw new ArgumentException("No columns were mapped.");
			}

			var dialect = this.GetDialect(map.EntityType);
			var query = new SqlBuilder(dialect);

			var table = this.GetTableName(map);

			var columnNames = columns.Select(p => GetColumnName(map, p, false));
			var parameters = columns.Select(p => dialect.ParameterPrefix + p.Name);

			var triggerIdentityColumn = map.Properties
				.Where(p => p.KeyType == KeyType.TriggerIdentity)
				.ToList();

			if (triggerIdentityColumn.Count > 1)
			{
				throw new ArgumentException(
					"TriggerIdentity generator cannot be used with multi-column keys");
			}

			var triggerColumn = triggerIdentityColumn.Count == 0
				? String.Empty
				: triggerIdentityColumn.Select(p => GetColumnName(map, p, false))
						.First();
			
			query
				.Insert(into: table)
				.Values(columnNames, parameters)
				.Returning(triggerColumn);

			return query.Build();
		}

		public override string Update(
			IClassMapper map, 
			IPredicate predicate,
			Parameters parameters, 
			bool ignoreKeyProperties)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}

			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			var columns = ignoreKeyProperties
				? map.Properties.Where(PropertyIsNotAKey)
				: map.Properties.Where(PropertyIsKey);

			if (!columns.Any())
			{
				throw new ArgumentException("No columns were mapped.");
			}

			var dialect = this.GetDialect(map.EntityType);
			var query = new SqlBuilder(dialect);

			var table = this.GetTableName(map);
			var values = columns.Select(c => $"{this.GetColumnName(map, c, false)} = {dialect.ParameterPrefix}{c.Name}");

			query
				.Update(table)
				.Set(values)
				.Where(predicate.GetSql(this, parameters));

			return query.Build();
		}

		public override string Delete(
			IClassMapper map, 
			IPredicate predicate, 
			Parameters parameters)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate));
			}

			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			var query = new SqlBuilder();
			var table = this.GetTableName(map);

			query
				.Delete(from: table)
				.Where(predicate.GetSql(this, parameters));

			return query.Build();
		}

		public override string IdentitySql(IClassMapper map)
		{
			var table = this.GetTableName(map);
			var dialect = this.GetDialect(map.EntityType);

			return dialect.GetIdentitySql(table);
		}

		public override string GetTableName(IClassMapper map) =>
			this.GetDialect(map.EntityType)
				.GetTableName(map.SchemaName, map.TableName, null);

		public override string GetColumnName(
			IClassMapper map, 
			IPropertyMap property, 
			bool includeAlias)
		{
			string alias = null;
			if (property.ColumnName != property.Name &&includeAlias)
			{
				alias = property.Name;
			}

			var table = this.GetTableName(map);
			var dialect = this.GetDialect(map.EntityType);

			return dialect.GetColumnName(table, property.ColumnName, alias);
		}

		private static bool PropertyIsKey(IPropertyMap property) =>
			!(property.Ignored || property.IsReadOnly ||
			  property.KeyType == KeyType.Identity ||
			  property.KeyType == KeyType.Assigned);

		private static bool PropertyIsNotAKey(IPropertyMap property) =>
			!(property.Ignored || property.IsReadOnly) &&
			  property.KeyType == KeyType.NotAKey;

		private ISqlDialect GetDialect(Type entity) =>
			DialectCache.GetDialectOrDefault(entity);
	}
}
