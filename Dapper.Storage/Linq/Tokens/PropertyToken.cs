using System;
using System.Linq.Expressions;

namespace Dapper.Storage.Linq.Tokens
{
	using LExpression = Expression;
	public class PropertyToken : Token
	{
		public new MemberExpression Expression =>
			(MemberExpression)base.Expression;

		public override string Name =>
			Expression.Member.Name;

		public override object Value
		{
			get
			{
				var unary = LExpression.Convert(Expression, typeof(object));
				var lambda = LExpression.Lambda<Func<object>>(unary);

				return lambda.Compile().Invoke();
			}
		}

		public PropertyToken(MemberExpression expression)
			:base(expression)
		{
		}
	}
}
