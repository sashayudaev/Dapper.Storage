using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Dapper.Storage.Core;

namespace Dapper.Storage.Context
{
	public class SqlServerContext : IStorageContext
	{
		internal DbConnectionStringBuilder Builder { get; }

		public SqlServerContext(string connection)
		{
			Builder = new SqlConnectionStringBuilder(connection);
		}

		public IDbConnection ConfigureConnection(string login, string password)
		{
			var builder = new SqlConnectionStringBuilder(
				Builder.ConnectionString)
			{
				UserID = login,
				Password = password
			};

			return new SqlConnection(builder.ConnectionString);
		}

		public IDbConnection ConfigureConnection() =>
			new SqlConnection(Builder.ConnectionString);
	}
}
