using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Autofac;
using Dapper.Storage.Core;

namespace Dapper.Storage
{
	public class StorageResource : IStorageResource
	{
		public StorageType StorageType { get; } =
			StorageType.Postgres;

		internal IQuery Query => 
			AvailableScope.ResolveKeyed<IQuery>(StorageType);
		internal IStorage Storage => 
			AvailableScope.ResolveKeyed<IStorage>(StorageType);

		public bool HasTransaction =>
			TransactionLevel > 0 && 
			Transaction.Current != null;

		private ILifetimeScope AvailableScope => HasTransaction
			? TransactionScope
			: RootScope;

		private ILifetimeScope RootScope { get; }
		private ILifetimeScope TransactionScope { get; set; }

		public int TransactionLevel { get; set; }

		public StorageResource(IComponentContext context)
		{
			RootScope = (ILifetimeScope) context;
		}

		#region IStorageScope
		public IStorageScope Begin()
		{
			if(TransactionScope == null)
			{
				TransactionScope = this.NewScope();
			}

			return new StorageScope(this);
		}

		public void End() =>
			TransactionScope.Dispose();
		#endregion

		#region IStorage
		public IQueryable<TEntity> Select<TEntity>()
			where TEntity : class =>
			Storage.Select<TEntity>();
			
		public async Task InsertAsync<TEntity>(TEntity entity)
			where TEntity : class => await
			Storage.InsertAsync(entity);

		public async Task<bool> UpdateAsync<TEntity>(TEntity entity)
			where TEntity : class => await
			Storage.UpdateAsync(entity);

		public async Task<bool> DeleteAsync<TEntity>(TEntity entity)
			where TEntity : class => await
			Storage.DeleteAsync(entity);
		#endregion

		#region IQuery
		public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string query, TEntity entity)
			where TEntity : class => await
			Query.QueryAsync(query, entity);

		public async Task<TEntity> QueryScalarAsync<TEntity>(string query, TEntity entity)
			where TEntity : class => await
			Query.QueryScalarAsync(query, entity);
		#endregion

		#region IHaveConnection
		public IDbConnection OpenConnection(string login, string password) =>
			Storage.OpenConnection(login, password);
		#endregion

		#region IDisposable
		public void Dispose() =>
			RootScope.Dispose();
		#endregion

		private ILifetimeScope NewScope() =>
			RootScope.BeginLifetimeScope();
	}
}
