using Autofac.Features.AttributeFilters;
using Dapper.Storage.Core;

namespace Dapper.Storage
{
	public class QueryScope
	{
		public IStorage Postgres { get; }
		public IStorage Sybase { get; }

		public QueryScope(
			[KeyFilter(StorageType.Postgres)] IStorage postgres,
			[KeyFilter(StorageType.Sybase)] IStorage sybase)
		{
			Postgres = postgres;
			Sybase = sybase;
		}
	}
}
