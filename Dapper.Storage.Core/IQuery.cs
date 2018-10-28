using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dapper.Storage.Core
{
	public interface IQuery : IHaveConnection, IDisposable
	{
		Task QueryAsync(string query, object entity = null);

		Task<IEnumerable<TEntity>> QueryProcedure<TEntity>(string name, object parameters = null)
			where TEntity : class;
		Task<IEnumerable<TEntity>> QueryProcedure<TEntity>(IStoredProcedure procedure)
			where TEntity : class;
		Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string query, TEntity entity)
			where TEntity : class;
		Task<TEntity> QueryScalarAsync<TEntity>(string query, TEntity entity)
			where TEntity : class;
	}
}
