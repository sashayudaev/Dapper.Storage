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

			var storage = provider.GetService(typeof(IQuery)) 
				as IQuery;

			var getUser = new GetUserProcedure(id: 9);

			var users = storage.QueryProcedure<UserEntity>(getUser).Result;

			foreach (var user in users)
			{
				System.Console.WriteLine(
					$"{user.Id} - {user.Login}");
			}

			System.Console.ReadKey();
		}
	}
}
