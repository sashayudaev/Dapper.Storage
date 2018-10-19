using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Storage.Core
{
	public interface IStorageFactory
	{
		IStorage CreateStorage();
		//IQuery CreateStorage();
	}
}
