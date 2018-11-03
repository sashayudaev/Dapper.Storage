using System;
using System.Linq.Expressions;
using DapperExtensions;

namespace Dapper.Storage.Linq
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
				default:
					throw new ArgumentException(expression.GetType().ToString());
			}
		}
	}
}
