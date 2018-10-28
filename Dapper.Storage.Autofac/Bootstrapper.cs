using System;
using System.Configuration;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dapper.Storage.Context;
using Dapper.Storage.Core;

namespace Dapper.Storage.Autofac
{
	public static class Bootstrapper
	{
		public static string Connection =>
			ConfigurationManager.ConnectionStrings["Inferno"].ConnectionString;

		public static IServiceProvider ConfigureProvider()
		{
			var builder = new ContainerBuilder();

			RegisterStorage(
				builder,
				() => new PostgresContext(Connection),
				StorageType.Postgres);

			RegisterStorage(
				builder,
				() => new SybaseContext(),
				StorageType.Sybase);

			builder.RegisterType<StorageResource>()
				.As<IStorageResource>()
				.As<IHaveConnection>()
				.As<IStorage>()
				.As<IQuery>();

			return new AutofacServiceProvider(builder.Build());
		}

		private static void RegisterStorage(
			ContainerBuilder container,
			Func<IStorageContext> createContext,
			StorageType key)
		{
			container.Register(_ => CreateStorage(createContext))
				.Keyed<IStorage>(key)
				.Keyed<IQuery>(key)
				.InstancePerLifetimeScope();
		}

		private static Storage CreateStorage(Func<IStorageContext> createContext)
		{
			var context = createContext();
			return new Storage(context);
		}
	}
}
