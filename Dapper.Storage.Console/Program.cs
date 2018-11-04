using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dapper.Storage.Autofac;
using Dapper.Storage.Core;
using Dapper.Storage.Entities;
using DapperExtensions;

namespace Dapper.Storage.Console
{
	public class Benchmark
	{

		public void Run(Action action)
		{
			var reps = new List<long>();

			for (int i = 0; i < 1000; i++)
			{
				var stopwatch = Stopwatch.StartNew();
				action();
				stopwatch.Stop();

				reps.Add(stopwatch.ElapsedMilliseconds);
			}

			System.Console.WriteLine(
				$"MIN: {reps.Min()}, MAX: {reps.Max()}, AVG: {reps.Average()}");
		}
	}
	internal class Program
	{
		private static void Main(string[] args)
		{
			var provider = Bootstrapper.ConfigureProvider();

			var storage = provider.GetService(typeof(IStorage))
				as IStorage;

			var benchmark = new Benchmark();

			var user = storage
				.Select<UserEntity, int>()
				.Where(u => u.Id > 3 && u.Password.Equals("test") && u.Id < 19)
				.AsEnumerable();

			System.Console.ReadKey();
		}

		private static bool CanConvert(object left, object right) => true;
	}
}
