using System;
using System.Linq;
using System.Transactions;
using Dapper.Storage.Autofac;
using Dapper.Storage.Context;
using Dapper.Storage.Entities;

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
