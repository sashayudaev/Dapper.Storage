using System.Transactions;

namespace Dapper.Storage.Core
{
	public interface IStorageScope : IStorage, IQuery
	{
		TransactionScope Begin();
	}
}
