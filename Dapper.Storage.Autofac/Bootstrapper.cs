using System;
using System.Configuration;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dapper.Storage.Context;
using Dapper.Storage.Core;
using Autofac.Features.AttributeFilters;
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
				.Keyed<IStorage>(StorageType.Postgres);
			builder.Register(_ => CreateStorage(() => new SybaseContext()))
				.Keyed<IStorage>(StorageType.Sybase);

			builder.RegisterType<QueryScope>()
				.WithAttributeFiltering()
				.AsSelf();

			return new AutofacServiceProvider(builder.Build());
		}

		private static Storage CreateStorage(Func<IStorageContext> createContext)
		{
			var context = createContext();
			return new Storage(context);
		}
	}
}
