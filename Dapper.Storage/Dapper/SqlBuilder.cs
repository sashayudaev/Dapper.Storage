using System;
using System.Collections.Generic;
using System.Text;
using DapperExtensions;
using DapperExtensions.Sql;

namespace Dapper.Storage.Dapper
{
	public class SqlBuilder
	{
		private StringBuilder Query { get; } =
			new StringBuilder();

		private ISqlDialect Dialect { get; set; }

		public SqlBuilder(ISqlDialect dialect)
		{
			Dialect = dialect;
		}

		public SqlBuilder()
			:this(new PostgreSqlDialect())
		{

		}

		public SqlBuilder UseDialect(ISqlDialect dialect)
		{
			Dialect = dialect;
			return this;
		}

		public SqlBuilder Select(string names)
		{
			Query.Append($"SELECT {names}");
			return this;
		}

		public SqlBuilder SelectCount()
		{
			Query.Append($"SELECT COUNT(*) AS {Dialect.OpenQuote}Total{Dialect.CloseQuote}");
			return this;
		}

		public SqlBuilder Insert(string into)
		{
			Query.Append($"INSERT INTO {into}");
			return this;
		}

		public SqlBuilder Update(string table)
		{
			Query.Append($"UPDATE {table}");
			return this;
		}

		public SqlBuilder Delete(string from)
		{
			Query.Append($"DELETE FROM {from}");
			return this;
		}

		public SqlBuilder From(string table)
		{
			Query.Append($"FROM {table}");
			return this;
		}

		public SqlBuilder Where(string predicate)
		{
			if(String.IsNullOrEmpty(predicate))
			{
				return this;
			}

			Query.Append($"WHERE {predicate}");
			return this;
		}

		public SqlBuilder OrderBy(string columns)
		{
			if (String.IsNullOrEmpty(columns))
			{
				return this;
			}

			Query.Append($"ORDER BY {columns}");
			return this;
		}

		public SqlBuilder Set(IEnumerable<string> values)
		{
			Query.Append($"SET {values.AppendStrings()}");
			return this;
		}


		public SqlBuilder Values(
			IEnumerable<string> names, 
			IEnumerable<string> values)
		{
			Query.Append($"({names.AppendStrings()}) VALUES ({values.AppendStrings()})");
			return this;
		}

		public SqlBuilder Returning(string column)
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
}
