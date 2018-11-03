using Dapper.Storage.Autofac;
using Dapper.Storage.Core;
using Dapper.Storage.Entities;
using Dapper.Storage.Procedures;

namespace Dapper.Storage.Console
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var provider = Bootstrapper.ConfigureProvider();

			var storage = provider.GetService(typeof(IStorage)) 
				as IStorage;

			var user = storage.Select<UserEntity>(u => u.Login.Contains("sasha"));

			System.Console.ReadKey();
		}
	}
}
