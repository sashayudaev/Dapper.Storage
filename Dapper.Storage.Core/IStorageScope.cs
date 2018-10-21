using System;
using System.Transactions;

namespace Dapper.Storage.Core
{
	public interface ITransactionScope : IDisposable
	{
		void Complete();
	}

	public interface IStorageScope : IStorage, IQuery
	{
		bool HasTransaction { get; }
		int TransactionLevel { get; set; }

		ITransactionScope Begin();
		void End();
	}
}
