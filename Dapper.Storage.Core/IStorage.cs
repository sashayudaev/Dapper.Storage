using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper.Storage.Core.Linq;

namespace Dapper.Storage.Core
{
	public interface IStorage : IHaveConnection, IDisposable
	{
		IQueryBuilder<TEntity, TResult> Select<TEntity, TResult>(
			Expression<Func<TEntity, TResult>> predicate)
			where TEntity : class;
		Task InsertAsync<TEntity>(TEntity entity)
			where TEntity : class;
		Task<bool> UpdateAsync<TEntity>(TEntity entity)
			where TEntity : class;
		Task<bool> DeleteAsync<TEntity>(TEntity entity)
			where TEntity : class;
	}
}
