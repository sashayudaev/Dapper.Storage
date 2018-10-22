using System;
using Dapper.Storage.Core;

namespace Dapper.Storage.Attributes
{
	public class DialectAttribute : Attribute
	{
		public StorageType Dialect { get; }

		public DialectAttribute(StorageType dialect)
		{
			Dialect = dialect;
		}
	}
}
