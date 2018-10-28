using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dapper.Storage.Attributes;
using Dapper.Storage.Core;

namespace Dapper.Storage.Procedures
{
	public abstract class ProcedureBase : IStoredProcedure
	{
		public string Name { get; }

		public object Parameter =>
			this.GenerateParameters();

		public ProcedureBase(string name)
		{
			Name = name ??
				throw new ArgumentNullException(nameof(name));
		}

		private IDictionary<string, object> GenerateParameters() =>
			this.GetType().GetProperties()
				.Where(HasParameterAttribute)
				.ToDictionary(
					GetParameterName,
					x => x.GetValue(this));

		private static string GetParameterName(PropertyInfo property) =>
			property.GetCustomAttribute<ProcedureParameterAttribute>().Name;

		private static bool HasParameterAttribute(PropertyInfo property) =>
			Attribute.IsDefined(
				property, 
				typeof(ProcedureParameterAttribute));
	}
}
