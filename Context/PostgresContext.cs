using System.Data;
using Dapper.Storage.Core;
using Npgsql;

namespace Dapper.Storage.Context
{
	public class PostgresContext : IStorageContext
	{
		public NpgsqlConnectionStringBuilder Builder { get; }

		public PostgresContext(string connection)
		{
			Builder = new NpgsqlConnectionStringBuilder(connection);
		}

		public IDbConnection ConfigureConnection() =>
			new NpgsqlConnection(Builder.ConnectionString);

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
	}
}
