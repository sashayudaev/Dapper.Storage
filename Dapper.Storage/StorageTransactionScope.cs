using System;
using System.Transactions;
using Autofac;
using Dapper.Storage.Core;

namespace Dapper.Storage
{
	using Debug = System.Diagnostics.Debug;
	public class StorageTransactionScope : ITransactionScope
	{
		private class TransactionToken : IDisposable
		{
			public IStorageScope Scope { get; }

			public TransactionToken(IStorageScope scope)
			{
				Scope = scope;
				Scope.TransactionLevel++;
				Debug.WriteLine($"[TransactionLevel]: {Scope.TransactionLevel}");
			}

			public void Dispose()
			{
				Scope.TransactionLevel--;
				Debug.WriteLine($"[TransactionLevel]: {Scope.TransactionLevel}");

				if (!Scope.HasTransaction)
				{
					Scope.End();
				}
			}
		}

		private TransactionToken Token { get; }
		private TransactionScope TransactionScope { get; }

		public StorageTransactionScope(IStorageScope scope)
		{
			Token = new TransactionToken(scope);
			TransactionScope = this.StartTransaction();
		}

		public void Complete()
		{
			TransactionScope.Complete();
		}

		public void Dispose()
		{
			Token.Dispose();
			TransactionScope.Dispose();
		}

		private TransactionScope StartTransaction() =>
			new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
	}
}
