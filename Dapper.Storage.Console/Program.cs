using Dapper.Storage.Autofac;

namespace Dapper.Storage.Console
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var provider = Bootstrapper.ConfigureProvider();

			var scope = provider.GetService(typeof(QueryScope)) 
				as QueryScope;
		}
	}
}
