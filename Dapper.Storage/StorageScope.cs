using System;
using System.Transactions;
using Dapper.Storage.Core;

namespace Dapper.Storage
{
	using Debug = System.Diagnostics.Debug;
	public class StorageScope : IStorageScope
	{
		private class TransactionToken : IDisposable
		{
			public IStorageResource Scope { get; }

			public TransactionToken(IStorageResource scope)
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

		public StorageScope(IStorageResource scope)
		{
			Token = new TransactionToken(scope);
			TransactionScope = this.StartTransaction();
		}

		public void Complete() =>
			TransactionScope.Complete();

		public void Dispose()
		{
			Token.Dispose();
			TransactionScope.Dispose();
		}

		private TransactionScope StartTransaction()
		{
			var options = new TransactionOptions
			{
				IsolationLevel = IsolationLevel.ReadCommitted

			};

			return new TransactionScope(
				TransactionScopeOption.RequiresNew,
				options,
				TransactionScopeAsyncFlowOption.Enabled);
		}
	}
}
