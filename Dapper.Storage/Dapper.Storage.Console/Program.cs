using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Storage.Context;

namespace Dapper.Storage.Console
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var connection = 
				"Server=127.0.0.1;Port=5432;Database=postgres;" +
				"User Id=postgres;Password=nothingissafe123;";

			var postgresContext = new PostgresContext(connection);

			using (var storage = new Storage(postgresContext))
			{
				var users = storage.Select
			}

			System.Console.ReadKey();
		}
	}
}
