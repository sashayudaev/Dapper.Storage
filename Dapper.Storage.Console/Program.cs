using System;
using System.Linq;
using System.Threading;
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

			var storage = provider.GetService(typeof(IStorageResource)) 
				as IStorageResource;

			var func = new ThreadStart(() => BeginThread(storage));
			var thread = new Thread(func);

			thread.Start();

			using (var first = storage.Begin())
			{
				while (true)
				{
					storage.Select<UserEntity>();
				}
			}
		}

		private static void BeginThread(IStorageResource storage)
		{
			while(storage != null)
			{
				storage.Select<UserEntity>();
				Thread.Sleep(1);
			}
		}
	}
}
