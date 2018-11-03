using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using Autofac;
using Dapper.Storage.Core;
using Dapper.Storage.Core.Linq;

namespace Dapper.Storage
{
	public static class StorageResourceExtensions
	{
		public static void UsePostgres(this IStorageResource storage) =>
			storage.Use(StorageType.Postgres);
		public static void UseSybase(this IStorageResource storage) =>
			storage.Use(StorageType.Sybase);
	}

	public class StorageResource : IStorageResource
	{
		public StorageType StorageType { get; private set; }

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

		public void Use(StorageType storage) =>
			StorageType = storage;
		#endregion

		#region IStorage
		public IQueryBuilder<TEntity, TResult> Select<TEntity, TResult>(
			Expression<Func<TEntity, TResult>> predicate)
			where TEntity : class => Storage.Select(predicate);

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
		public async Task<IEnumerable<TEntity>> QueryProcedure<TEntity>(string name, object parameters)
			where TEntity : class => await
			Query.QueryProcedure<TEntity>(name, parameters);

		public async Task<IEnumerable<TEntity>> QueryProcedure<TEntity>(IStoredProcedure procedure)
			where TEntity : class => await
			Query.QueryProcedure<TEntity>(procedure);

		public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string query, TEntity entity)
			where TEntity : class => await
			Query.QueryAsync(query, entity);

		public async Task<TEntity> QueryScalarAsync<TEntity>(string query, TEntity entity)
			where TEntity : class => await
			Query.QueryScalarAsync(query, entity);

		public async Task QueryAsync(string query, object entity = null) =>
			await Query.QueryAsync(query, entity);
		#endregion

		#region IHaveConnection
		public IDbConnection OpenConnection() =>
			Storage.OpenConnection();
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
