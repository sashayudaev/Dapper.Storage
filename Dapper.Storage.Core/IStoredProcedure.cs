namespace Dapper.Storage.Core
{
	public interface IStoredProcedure
	{
		string Name { get; }
		object Parameter { get; }
	}
}
