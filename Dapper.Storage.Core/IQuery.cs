using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dapper.Storage.Core
{
	public interface IQuery : IHaveConnection, IDisposable
	{
		Task<IEnumerable<TEntity>> QueryAsync<TEntity>(string query, TEntity entity)
			where TEntity : class;
		Task<TEntity> QueryScalarAsync<TEntity>(string query, TEntity entity)
		   where TEntity : class;
	}
}
