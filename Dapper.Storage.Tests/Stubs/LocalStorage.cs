using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Storage.Core;

namespace Dapper.Storage.Tests.Stubs
{
	public class LocalStorage : IStorage, IQuery
	{
		#region IStorage
		public IQueryable<TEntity> Select<TEntity>() 
			where TEntity : class
		{
			throw new NotImplementedException();
		}
		public Task InsertAsync<TEntity>(TEntity entity)
			where TEntity : class
		{
			throw new NotImplementedException();
		}
		public Task<bool> UpdateAsync<TEntity>(TEntity entity)
			where TEntity : class
		{
			throw new NotImplementedException();
		}
		public Task<bool> DeleteAsync<TEntity>(TEntity entity) 
			where TEntity : class
		{
			throw new NotImplementedException();
		}
		#endregion

		#region IQuery
		public Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string query, TEntity entity)
			where TEntity : class
		{
			throw new NotImplementedException();
		}

		public Task<TEntity> QueryScalarAsync<TEntity>(string query, TEntity entity) 
			where TEntity : class
		{
			throw new NotImplementedException();
		}
		#endregion

		#region IHaveConnection
		public IDbConnection OpenConnection(string login, string password)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region IDisposable
		public void Dispose()
		{
			throw new NotImplementedException();
		}
		#endregion
		
	}
}
