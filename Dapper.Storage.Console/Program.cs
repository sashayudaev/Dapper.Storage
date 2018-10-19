using System;
using System.Linq;
using Dapper.Storage.Context;
using Dapper.Storage.Entities;

namespace Dapper.Storage.Console
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var connectionString = 
				"Server=127.0.0.1;Port=5432;Database=Inferno;" +
				"User Id=postgres;Password=nothingissafe123;";

			var postgresContext = new PostgresContext(connectionString);

			using (var storage = new Storage(postgresContext))
			using(var connection = storage.OpenConnection("ololo", "ololo"))
			{
				
			}

			System.Console.ReadKey();
		}
	}
}
