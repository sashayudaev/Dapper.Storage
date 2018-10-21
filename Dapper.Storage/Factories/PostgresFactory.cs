using Autofac;
using Dapper.Storage.Core;

namespace Dapper.Storage.Factories
{
	public class PostgresFactory : IStorageFactory
	{
		public ILifetimeScope Context { get; }

		public PostgresFactory(IComponentContext context)
		{
			Context = (ILifetimeScope) context;
		}

		public TStorage CreateStorage<TStorage>()
			where TStorage : IHaveConnection => 
			Context.BeginLifetimeScope()
			.ResolveKeyed<TStorage>(StorageType.Postgres);
	}
}
