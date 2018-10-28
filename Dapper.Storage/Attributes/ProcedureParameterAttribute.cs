using System;

namespace Dapper.Storage.Attributes
{
	public class ProcedureParameterAttribute : Attribute
	{
		public string Name { get; }

		public ProcedureParameterAttribute(string name)
		{
			Name = name;
		}
	}
}
