using System;
using System.Linq.Expressions;

namespace Dapper.Storage.Linq.Tokens
{
	public abstract class Token : IToken
	{
		public virtual Expression Expression { get; }

		public abstract string Name { get; }
		public abstract object Value { get; }

		public Token(Expression expression)
		{
			Expression = expression;
		}

		public static IToken Create(Expression expression)
		{
			switch (expression)
			{
				case BinaryExpression binary:
					return new BinaryToken(binary);
				case MemberExpression property:
					return new PropertyToken(property);
				case ConstantExpression constant:
					return new ConstantToken(constant);
				case MethodCallExpression method:
					return CreateMethodToken(method);
				default:
					throw new ArgumentException(expression.GetType().ToString());
			}
		}

		private static IToken CreateMethodToken(MethodCallExpression expression)
		{
			if(expression.Method.Name.Equals("Equals"))
			{

			}

			var binary = Expression.MakeBinary(
				ExpressionType.Equal,
				expression.Object,
				expression.Arguments[0]);

			return new BinaryToken(binary);
		}
	}
}
