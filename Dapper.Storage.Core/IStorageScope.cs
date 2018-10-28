using System;
using System.Transactions;

namespace Dapper.Storage.Core
{
	public interface IStorageScope : IDisposable
	{
		void Complete();
	}
}
