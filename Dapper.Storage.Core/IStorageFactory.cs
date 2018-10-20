namespace Dapper.Storage.Core
{
	public interface IStorageFactory
	{
		TStorage CreateStorage<TStorage>()
			where TStorage : IHaveConnection;
	}
}
