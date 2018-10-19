﻿using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper.Storage.Core;

namespace Dapper.Storage
{
	using DapperExtensions;

	public class Storage : IStorage, IQuery
	{
		public IStorageContext Context { get; }
		protected IDbConnection Connection { get; }

		public Storage(IStorageContext context)
		{
			Context = context;
			Connection = context.ConfigureConnection();
		}

		#region IStorage
		public IQueryable<TEntity> Select<TEntity>()
			where TEntity : class => 
			Connection.GetList<TEntity>().AsQueryable();

		public Task InsertAsync<TEntity>(TEntity entity)
			where TEntity : class => 
			Connection.InsertAsync(entity);

		public Task<bool> UpdateAsync<TEntity>(TEntity entity)
			where TEntity : class =>
			Connection.UpdateAsync(entity);

		public Task<bool> DeleteAsync<TEntity>(TEntity entity)
			where TEntity : class =>
			Connection.DeleteAsync(entity);
		#endregion

		#region IQuery
		public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string query, TEntity entity)
			where TEntity : class => 
			Connection.QueryAsync<TEntity>(query, entity);

		public Task<TEntity> QueryScalarAsync<TEntity>(string query, TEntity entity)
			where TEntity : class =>
			Connection.ExecuteScalarAsync<TEntity>(query, entity);
		#endregion

		#region IHaveConnection
		public IDbConnection OpenConnection(string login, string password) =>
			Context.ConfigureConnection(login, password);
		#endregion

		#region IDisposable
		public void Dispose() =>
			Connection?.Dispose();
		#endregion
	}
}