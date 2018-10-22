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
			var table = this.GetTableName(map);


			var select = new StringBuilder(
				$"SELECT {this.BuildSelectColumns(map)}");

			var from = $" FROM {table}";

			var where = String.Empty;
			if (predicate != null)
			{
				where = $" WHERE {predicate.GetSql(this, parameters)}";
			}

			var orderby = String.Empty;
			if (sort != null && sort.Any())
			{
				var sortingColumns = sort.Select(column =>
					this.GetColumnName(map, column.PropertyName, false) +
					(column.Ascending ? " ASC" : " DESC"));

				orderby = $" ORDER BY {sortingColumns}";
			}

			return select.Append(from).Append(where).Append(orderby).ToString();
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
			var table = this.GetTableName(map);

			var openQuote = dialect.OpenQuote;
			var closeQuote = dialect.CloseQuote;

			var select = new StringBuilder(
				$"SELECT COUNT(*) AS {openQuote}Total{closeQuote}");
			var from = $" FROM {table}";

			var where = String.Empty;
			if (predicate != null)
			{
				where = $" WHERE {predicate.GetSql(this, parameters)}";
			}

			return select.Append(from).Append(where).ToString();
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

			var columnNames = columns.Select(p => GetColumnName(map, p, false));
			var parameters = columns.Select(p => dialect.ParameterPrefix + p.Name);

			var insert = new StringBuilder("INSERT INTO");
			var table = $" {this.GetTableName(map)} ";
			var names = $" ({columnNames.AppendStrings()}) ";
			var values = $"VALUES ({parameters.AppendStrings()})";

			insert.Append(table)
				.Append(names)
				.Append(values);

			var triggerIdentityColumn = map.Properties
				.Where(p => p.KeyType == KeyType.TriggerIdentity)
				.ToList();

			if (triggerIdentityColumn.Count > 1)
			{
				throw new ArgumentException(
					"TriggerIdentity generator cannot be used with multi-column keys");
			}

			var returning = String.Empty;
			if (triggerIdentityColumn.Count > 0)
			{
				var triggerColumn = triggerIdentityColumn.Select(p => GetColumnName(map, p, false)).First();
				returning = $" RETURNING {triggerColumn} INTO {dialect.ParameterPrefix}IdOutParam";
			}

			return insert.Append(returning).ToString();
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

			var update = new StringBuilder("UPDATE");
			var table = $" {this.GetTableName(map)} ";
			var set = columns.Select(c => $"{this.GetColumnName(map, c, false)} = {dialect.ParameterPrefix}{c.Name}");
			var where = predicate.GetSql(this, parameters);

			return update.Append(table).Append(set).Append(where).ToString();
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

			var table = this.GetTableName(map);

			var delete = new StringBuilder("DELETE");
			var from = $"FROM {table}";
			var where = $" WHERE {predicate.GetSql(this, parameters)}";

			return delete.Append(from).Append(where).ToString();
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
				{ StorageType.SqlServer, SqlServer },
				{ StorageType.Postgres, Postgres },
				{ StorageType.Sybase, SqlServer },
				{ StorageType.Oracle, Oracle }
			};
	}
}
