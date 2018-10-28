namespace Dapper.Storage.Core
{
	public interface IStorageResource : IStorage, IQuery
	{
		bool HasTransaction { get; }
		int TransactionLevel { get; set; }

		IStorageScope Begin();
		void End();

		void Use(StorageType storage);
	}
}
