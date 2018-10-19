using System;

namespace Dapper.Storage.Attributes
{
	public class TableAttribute : Attribute
	{
		public string Name { get; }
		public string Schema { get; set; }

		public TableAttribute(string name)
		{
			Name = name ??
				throw new ArgumentNullException(name);
		}
	}
}
