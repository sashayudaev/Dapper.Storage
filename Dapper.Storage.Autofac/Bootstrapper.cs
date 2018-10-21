using System;
using System.Configuration;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dapper.Storage.Context;
using Dapper.Storage.Core;
using Dapper.Storage.Factories;

namespace Dapper.Storage.Autofac
{
	public static class Bootstrapper
	{
		public static string Connection =>
			ConfigurationManager.ConnectionStrings["Inferno"].ConnectionString;

		public static IServiceProvider ConfigureProvider()
		{
			var builder = new ContainerBuilder();

			builder.Register(_ => CreateStorage(() => new PostgresContext(Connection)))
				.Keyed<IStorage>(StorageType.Postgres)
				.InstancePerLifetimeScope();
			builder.Register(_ => CreateStorage(() => new SybaseContext()))
				.Keyed<IStorage>(StorageType.Sybase)
				.InstancePerLifetimeScope();

			builder.RegisterType<PostgresFactory>()
				.As<IStorageFactory>();

			builder.RegisterType<StorageResource>()
				.As<IStorageResource>();

			return new AutofacServiceProvider(builder.Build());
		}

		private static Storage CreateStorage(Func<IStorageContext> createContext)
		{
			var context = createContext();
			return new Storage(context);
		}
	}
}
