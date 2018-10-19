using System;
using System.Data;
using Dapper.Storage.Core;

namespace Dapper.Storage.Context
{
	public class SybaseContext : IStorageContext
	{
		public IDbConnection ConfigureConnection() => null;

		public IDbConnection ConfigureConnection(string login, string password) => null;
	}
}
