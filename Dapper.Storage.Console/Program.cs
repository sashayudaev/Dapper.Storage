using System.Linq;
using Dapper.Storage.Autofac;
using Dapper.Storage.Core;
using Dapper.Storage.Entities;

namespace Dapper.Storage.Console
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var provider = Bootstrapper.ConfigureProvider();

			var scope = provider.GetService(typeof(IStorageScope)) 
				as IStorageScope;

			using (var first = scope.Begin())
			{
				var users = scope.Select<UserEntity>();
				using (var second = scope.Begin())
				{
					var others = scope.Select<UserEntity>();
				}
				var left = scope.Select<UserEntity>();
			}

			var entities = scope.Select<UserEntity>();

			System.Console.ReadKey();
		}
	}
}
