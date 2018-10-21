using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Autofac;
using Dapper.Storage.Core;

namespace Dapper.Storage
{
	public class StorageScope : IStorageScope
	{
		internal IQuery Query =>
			RootScope.ResolveKeyed<IQuery>(StorageType.Postgres);
		internal IStorage Storage =>
			RootScope.ResolveKeyed<IStorage>(StorageType.Postgres);

		public bool HasTransaction =>
			TransactionLevel > 0;

		private ILifetimeScope NewScope =>
			Context.BeginLifetimeScope();

		private ILifetimeScope RootScope { get; set; }
		public ILifetimeScope Context { get; }

		public int TransactionLevel { get; set; }

		public StorageScope(IComponentContext context)
		{
			Context = (ILifetimeScope) context;
		}

		#region IStorageScope
		public ITransactionScope Begin()
		{
			if(RootScope == null)
			{
				RootScope = NewScope;
			}

			return new StorageTransactionScope(this);
		}

		public void End() =>
			RootScope.Dispose();
		#endregion

		#region IStorage
		public IQueryable<TEntity> Select<TEntity>()
			where TEntity : class
		{
			if(HasTransaction)
			{
				return Storage.Select<TEntity>();
			}

			using (RootScope = NewScope)
			{
				return Storage.Select<TEntity>();
			}
		}
			
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
		public void Dispose() { }
		#endregion
	}
}
