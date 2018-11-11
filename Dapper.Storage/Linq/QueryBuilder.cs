using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Dapper.Storage.Core.Linq;
using Dapper.Storage.Linq.Tokens;
using DapperExtensions;

namespace Dapper.Storage.Linq
{
	public class QueryBuilder<TEntity, TResult> : IQueryBuilder<TEntity, TResult>
		where TEntity : class
	{
		public IPredicate Predicate { get; private set; }
		public IDbConnection Connection { get; }

		public QueryBuilder(
			IDbConnection connection,
			Expression<Func<TEntity, TResult>> expression)
		{
			Connection = connection;
		}

		public IQueryBuilder<TEntity, TResult> Where(Expression<Func<TEntity, bool>> expression)
		{
			Predicate = this.WhereInternal(expression.Body);
			return this;
		}

		public IPredicate WhereInternal(Expression expression)
		{
			if(IsBinaryNodeType(expression.NodeType) &&
			   expression is BinaryExpression binary)
			{
				var left = WhereInternal(binary.Left);
				var right = WhereInternal(binary.Right);

				return CollectPredicates(left, right, expression.NodeType);
			}

			var token = Token.Create(expression);
			return CreatePredicate(token, expression.NodeType);
		}

		public IEnumerable<TEntity> AsEnumerable() =>
			Connection.GetList<TEntity>(Predicate);

		private static IPredicate CollectPredicates(
			IPredicate left, 
			IPredicate right, 
			ExpressionType nodeType)
		{
			GroupOperator groupOperator;

			switch (nodeType)
			{
				case ExpressionType.OrElse:
					groupOperator = GroupOperator.Or;
					break;
				case ExpressionType.AndAlso:
				default:
					groupOperator = GroupOperator.And;
					break;
			}

			return new PredicateGroup
			{
				Operator = groupOperator,
				Predicates = new[] { left, right }
			};
		}
		private static IPredicate CreatePredicate(IToken token, ExpressionType nodeType) =>
			new FieldPredicate<TEntity>
			{
				Value = token.Value,
				PropertyName = token.Name,
				Operator = GetOperatorType(nodeType)
			};

		private static bool IsBinaryNodeType(ExpressionType type) =>
			type == ExpressionType.AndAlso ||
			type == ExpressionType.OrElse;
		private static Operator GetOperatorType(ExpressionType nodeType)
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
	}
}
