using System.Data;

namespace Dapper.Storage.Core
{
	public interface IStorageContext
	{
		IDbConnection ConfigureConnection();
		IDbConnection ConfigureConnection(string login, string password);
	}
}
