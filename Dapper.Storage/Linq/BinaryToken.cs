using System.Linq.Expressions;

namespace Dapper.Storage.Linq
{
	public class BinaryToken : Token
	{
		public new BinaryExpression Expression =>
			(BinaryExpression)base.Expression;

		public override string Name =>
			Token.Create(Expression.Left).Name;

		public override object Value =>
			Token.Create(Expression.Right).Value;

		public BinaryToken(BinaryExpression expression)
			:base(expression)
		{
		}
	}
}
