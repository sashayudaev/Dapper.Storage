using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DapperExtensions;

namespace Dapper.Storage.Core.Linq
{
	public interface IQueryBuilder<TEntity, TResult>
		where TEntity : class
	{
		IPredicate Predicate { get; }

		IQueryBuilder<TEntity, TResult> Where(Expression<Func<TEntity, bool>> expression);
		IEnumerable<TEntity> AsEnumerable();
	}
}
