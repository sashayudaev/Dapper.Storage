using System.Data;

namespace Dapper.Storage.Core
{
	public interface IHaveConnection
	{
		IDbConnection OpenConnection();
		IDbConnection OpenConnection(string login, string password);
	}
}
