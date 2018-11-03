using System;
using System.Configuration;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dapper.Storage.Context;
using Dapper.Storage.Core;
using Dapper.Storage.Dapper;
using DapperExtensions;

namespace Dapper.Storage.Autofac
{
	using DapperExtensions = DapperExtensions.DapperExtensions;

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

			SetupDapper();
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

		private static void SetupDapper()
		{
			DapperExtensions.DefaultMapper = typeof(EntityClassMapper<>);
			DapperAsyncExtensions.DefaultMapper = typeof(EntityClassMapper<>);

			DapperExtensions.InstanceFactory = config => new DapperImplementor(new SqlGenerator(config));
			DapperAsyncExtensions.InstanceFactory = config => new DapperAsyncImplementor(new SqlGenerator(config));
		}

	}
}
