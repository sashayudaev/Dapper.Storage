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

			using (var connection = scope.OpenConnection("postgres", "nothingissafe123"))
			{
				var user = connection.Query<UserEntity>("getuser", new { id = 9 }, commandType: System.Data.CommandType.StoredProcedure);
			}
		}
	}
}
