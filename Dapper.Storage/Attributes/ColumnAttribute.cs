using System;

namespace Dapper.Storage.Attributes
{
	public class ColumnAttribute : Attribute
	{
		public string Name { get; }

		public ColumnAttribute(string name)
		{
			Name = name ??
				throw new ArgumentNullException(name);
		}
	}
}
