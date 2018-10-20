using Autofac;
using Dapper.Storage.Core;

namespace Dapper.Storage.Factories
{
	public class PostgresFactory : IStorageFactory
	{
		public IComponentContext Context { get; }

		public PostgresFactory(IComponentContext context)
		{
			Context = context;
		}

		public TStorage CreateStorage<TStorage>()
			where TStorage : IHaveConnection => 
			Context.ResolveKeyed<TStorage>(StorageType.Postgres);
	}
}
