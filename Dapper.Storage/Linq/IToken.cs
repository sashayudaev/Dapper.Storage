namespace Dapper.Storage.Linq
{
	public interface IToken
	{
		string Name { get; }
		object Value { get; }
	}
}
