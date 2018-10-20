using System.Linq;
using System.Threading.Tasks;

namespace Dapper.Storage.Core
{
	public interface IStorage : IHaveConnection
	{
		IQueryable<TEntity> Select<TEntity>()
			where TEntity : class;
		Task InsertAsync<TEntity>(TEntity entity)
			where TEntity : class;
		Task<bool> UpdateAsync<TEntity>(TEntity entity)
			where TEntity : class;
		Task<bool> DeleteAsync<TEntity>(TEntity entity)
			where TEntity : class;
	}
}
