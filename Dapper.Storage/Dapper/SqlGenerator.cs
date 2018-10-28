using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Storage.Attributes;
using Dapper.Storage.Core;
using DapperExtensions;
using DapperExtensions.Sql;

namespace Dapper.Storage.Dapper
{
	using DapperExtensions.Mapper;
	using DialectMapper = Dictionary<StorageType, ISqlDialect>;
	using Parameters = IDictionary<string, object>;

	public class QueryBuilder
	{
		private StringBuilder Query { get; } =
			new StringBuilder();

		private ISqlDialect Dialect { get; set; }

		public QueryBuilder(ISqlDialect dialect)
		{
			Dialect = dialect;
		}

		public QueryBuilder()
			:this(new PostgreSqlDialect())
		{

		}

		public QueryBuilder UseDialect(ISqlDialect dialect)
		{
			Dialect = dialect;
			return this;
		}

		public QueryBuilder Select(string names)
		{
			Query.Append($"SELECT {names}");
			return this;
		}

		public QueryBuilder SelectCount()
		{
			Query.Append($"SELECT COUNT(*) AS {Dialect.OpenQuote}Total{Dialect.CloseQuote}");
			return this;
		}

		public QueryBuilder Insert(string into)
		{
			Query.Append($"INSERT INTO {into}");
			return this;
		}

		public QueryBuilder Update(string table)
		{
			Query.Append($"UPDATE {table}");
			return this;
		}

		public QueryBuilder Delete(string from)
		{
			Query.Append($"DELETE FROM {from}");
			return this;
		}

		public QueryBuilder From(string table)
		{
			Query.Append($"FROM {table}");
			return this;
		}

		public QueryBuilder Where(string predicate)
		{
			if(String.IsNullOrEmpty(predicate))
			{
				return this;
			}

			Query.Append($"WHERE {predicate}");
			return this;
		}

		public QueryBuilder OrderBy(string columns)
		{
			if (String.IsNullOrEmpty(columns))
			{
				return this;
			}

			Query.Append($"ORDER BY {columns}");
			return this;
		}

		public QueryBuilder Set(IEnumerable<string> values)
		{
			Query.Append($"SET {values.AppendStrings()}");
			return this;
		}


		public QueryBuilder Values(
			IEnumerable<string> names, 
			IEnumerable<string> values)
		{
			Query.Append($"({names.AppendStrings()}) VALUES ({values.AppendStrings()})");
			return this;
		}

		public QueryBuilder Returning(string column)
		{
			if (String.IsNullOrEmpty(column))
			{
				return this;
			}

			Query.Append($" RETURNING {column} INTO {Dialect.ParameterPrefix}IdOutParam");
			return this;
		}

		public string Build() =>
			Query.ToString();
	}

	public class SqlGenerator : SqlGeneratorImpl
	{
		private static ISqlDialect SqlServer { get; } =
			new SqlServerDialect();
		private static ISqlDialect Postgres { get; } =
			new PostgreSqlDialect();
		private static ISqlDialect Oracle { get; } =
			new OracleDialect();

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
			var query = new QueryBuilder(dialect);

			var table = this.GetTableName(map);

			var sortingColumns = sort.Select(column =>
				this.GetColumnName(map, column.PropertyName, false) +
				(column.Ascending ? " ASC" : " DESC"));

			query
				.Select(BuildSelectColumns(map))
				.From(table)
				.Where(predicate?.GetSql(this, parameters))
				.OrderBy(sortingColumns.AppendStrings());

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
			var query = new QueryBuilder(dialect);

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
			var query = new QueryBuilder(dialect);

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

			var triggerColumn = triggerIdentityColumn
				.Select(p => GetColumnName(map, p, false))
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
			var query = new QueryBuilder(dialect);

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

			var query = new QueryBuilder();
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

		private ISqlDialect GetDialect(Type entity)
		{
			var dialect = StorageType.Postgres;
			if(entity.HasAttribute(out DialectAttribute attribute))
			{
				dialect = attribute.Dialect;
			}
			
			return Dialects[dialect];
		}

		private static readonly DialectMapper Dialects =
			new DialectMapper
			{
				{ StorageType.Postgres, Postgres },
				{ StorageType.Sybase, SqlServer }
			};
	}
}
