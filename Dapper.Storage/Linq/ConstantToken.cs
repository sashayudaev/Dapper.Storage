using System.Linq.Expressions;

namespace Dapper.Storage.Linq
{
	public class ConstantToken : Token
	{
		public new ConstantExpression Expression =>
			(ConstantExpression)base.Expression;

		public override string Name =>
			null;

		public override object Value =>
			Expression.Value;

		public ConstantToken(ConstantExpression expression)
			:base(expression)
		{
		}
	}
}
