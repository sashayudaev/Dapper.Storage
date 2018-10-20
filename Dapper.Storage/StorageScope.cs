using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper.Storage.Core;

namespace Dapper.Storage
{
	public class StorageScope : IStorageScope
	{
		internal IStorage Storage =>
			Factory.CreateStorage<IStorage>();
		internal IQuery Query =>
			Factory.CreateStorage<IQuery>();

		public IStorageFactory Factory { get; }

		public StorageScope(IStorageFactory factory)
		{
			Factory = factory;
		}

		#region IStorageScope
		public TransactionScope Begin() =>
			new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
		#endregion

		#region IStorage
		public IQueryable<TEntity> Select<TEntity>()
			where TEntity : class =>
			Storage.Select<TEntity>().AsQueryable();

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
	}
}
