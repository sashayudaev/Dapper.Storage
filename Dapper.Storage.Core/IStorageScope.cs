using System;
using System.Transactions;

namespace Dapper.Storage.Core
{
	public interface IStorageScope : IDisposable
	{
		void Complete();
	}

	public interface IStorageResource : IStorage, IQuery
	{
		bool HasTransaction { get; }
		int TransactionLevel { get; set; }

		IStorageScope Begin();
		void End();
	}
}
