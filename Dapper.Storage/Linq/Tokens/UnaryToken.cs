using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Dapper.Storage.Linq.Tokens
{
	public class UnaryToken : Token
	{
		public new UnaryExpression Expression =>
			(UnaryExpression)base.Expression;

		public MemberExpression Operand =>
			(MemberExpression) Expression.Operand;

		public override string Name =>
			Operand.Member.Name;

		public override object Value
		{
			get
			{
				var property = Operand.Member as PropertyInfo;
				property.GetValue()
			}
		}

		public UnaryToken(UnaryExpression expression) 
			: base(expression)
		{
		}
	}
}
