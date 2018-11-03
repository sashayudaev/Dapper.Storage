using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper.Storage.Core.Linq;
using DapperExtensions;

namespace Dapper.Storage.Linq
{

	public class QueryBuilder<TEntity, TResult> : IQueryBuilder<TEntity, TResult>
		where TEntity : class
	{
		public IDbConnection Connection { get; }

		public IList<IPredicate> Predicates { get; } =
			new List<IPredicate>();

		public QueryBuilder(
			IDbConnection connection,
			Expression<Func<TEntity, TResult>> expression)
		{
			Connection = connection;
		}

		public IQueryBuilder<TEntity, TResult> Where(Expression<Func<TEntity, bool>> expression)
		{
			var token = Token.Create(expression.Body);

			var predicate = new FieldPredicate<TEntity>
			{
				PropertyName = token.Name,
				Value = token.Value,
				Operator = this.GetOperatorType(expression.Body.NodeType)
			};

			this.AddPredicate(predicate);
			return this;
		}

		private Operator GetOperatorType(ExpressionType nodeType)
		{
			switch (nodeType)
			{
				case ExpressionType.Equal:
					return Operator.Eq;
				case ExpressionType.GreaterThan:
					return Operator.Gt;
				case ExpressionType.GreaterThanOrEqual:
					return Operator.Ge;
				case ExpressionType.LessThan:
					return Operator.Lt;
				case ExpressionType.LessThanOrEqual:
					return Operator.Le;
				default:
					throw new NotImplementedException();
			}
		}

		private void AddPredicate(FieldPredicate<TEntity> predicate) =>
			Predicates.Add(predicate);

		public IEnumerable<TEntity> AsEnumerable()
		{
			var group = new PredicateGroup
			{
				Predicates = Predicates,
				Operator = GroupOperator.And
			};

			return Connection.GetList<TEntity>(group);
		}
	}
}
