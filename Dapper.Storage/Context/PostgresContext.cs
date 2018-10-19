using System.Data;
using Dapper.Storage.Core;
using DapperExtensions;
using DapperExtensions.Sql;
using Npgsql;

namespace Dapper.Storage.Context
{
	using DapperExtensions = DapperExtensions.DapperExtensions;

	public class PostgresContext : IStorageContext
	{
		internal PostgreSqlDialect Dialect { get; } =
			new PostgreSqlDialect();

		internal NpgsqlConnectionStringBuilder Builder { get; }

		public PostgresContext(string connection)
		{
			Builder = new NpgsqlConnectionStringBuilder(connection);

			DapperExtensions.SqlDialect = Dialect;
			DapperAsyncExtensions.SqlDialect = Dialect;
		}

		public IDbConnection ConfigureConnection(string login, string password)
		{
			var builder = new NpgsqlConnectionStringBuilder(
				Builder.ConnectionString)
			{
				Username = login,
				Password = password
			};
			
			return new NpgsqlConnection(builder.ConnectionString);
		}

		public IDbConnection ConfigureConnection() =>
			new NpgsqlConnection(Builder.ConnectionString);
	}
}
