using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper.Storage.Core;

namespace Dapper.Storage
{
	using System;
	using DapperExtensions;
	using Debug = System.Diagnostics.Debug;

	public class Storage : IStorage, IQuery, IDisposable
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

		public async Task InsertAsync<TEntity>(TEntity entity)
			where TEntity : class => await
			Connection.InsertAsync(entity);

		public async Task<bool> UpdateAsync<TEntity>(TEntity entity)
			where TEntity : class => await
			Connection.UpdateAsync(entity);

		public async Task<bool> DeleteAsync<TEntity>(TEntity entity)
			where TEntity : class => await
			Connection.DeleteAsync(entity);
		#endregion

		#region IQuery
		public async Task<IEnumerable<TEntity>> QueryProcedure<TEntity>(string name, object parameters)
			where TEntity : class => await
			Connection.QueryAsync<TEntity>(
				sql: name, 
				param: parameters, 
				commandType: CommandType.StoredProcedure);

		public async Task<IEnumerable<TEntity>> QueryProcedure<TEntity>(IStoredProcedure procedure)
			where TEntity : class => await
			Connection.QueryAsync<TEntity>(
				sql: procedure.Name,
				param: procedure.Parameter,
				commandType: CommandType.StoredProcedure);

		public async Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string query, TEntity entity)
			where TEntity : class => await
			Connection.QueryAsync<TEntity>(query, entity);

		public async Task<TEntity> QueryScalarAsync<TEntity>(string query, TEntity entity)
			where TEntity : class => await
			Connection.ExecuteScalarAsync<TEntity>(query, entity);

		public async Task QueryAsync(string query, object entity = null) =>
			await Connection.QueryAsync(query, entity);
		#endregion

		#region IHaveConnection
		public IDbConnection OpenConnection() =>
			Context.ConfigureConnection();
		public IDbConnection OpenConnection(string login, string password) =>
			Context.ConfigureConnection(login, password);
		#endregion

		#region IDisposable
		public void Dispose()
		{
			Debug.WriteLine("Disposed");
			Connection?.Dispose();
		}
		#endregion
	}
}
