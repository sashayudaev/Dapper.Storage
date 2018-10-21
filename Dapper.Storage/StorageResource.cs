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
			
		public Task InsertAsync<TEntity>(TEntity entity)
			where TEntity : class =>
			Storage.InsertAsync(entity);

		public Task<bool> UpdateAsync<TEntity>(TEntity entity)
			where TEntity : class =>
			Storage.UpdateAsync(entity);

		public Task<bool> DeleteAsync<TEntity>(TEntity entity)
			where TEntity : class =>
			Storage.DeleteAsync(entity);
		#endregion

		#region IQuery
		public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string query, TEntity entity)
			where TEntity : class =>
			Query.QueryAsync(query, entity);

		public Task<TEntity> QueryScalarAsync<TEntity>(string query, TEntity entity)
			where TEntity : class =>
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

		private IStorage CreateStorage() =>
			this.CreateStorage<IStorage>();
		private IQuery CreateQuery() =>
			this.CreateStorage<IQuery>();

		private TStorage CreateStorage<TStorage>()
			where TStorage : IHaveConnection =>
			RootScope.ResolveKeyed<TStorage>(StorageType.Postgres);
	}
}
