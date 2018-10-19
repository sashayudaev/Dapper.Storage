using Autofac.Features.AttributeFilters;
using Dapper.Storage.Core;

namespace Dapper.Storage
{
	using Filter = KeyFilterAttribute;

	public class QueryScope
	{
		public IStorage Postgres { get; }
		public IStorage Sybase { get; }

		public QueryScope(
			[Filter(StorageType.Sybase)] IStorage sybase,
			[Filter(StorageType.Postgres)] IStorage postgres)
		{
			Sybase = sybase;
			Postgres = postgres;
		}
	}
}
