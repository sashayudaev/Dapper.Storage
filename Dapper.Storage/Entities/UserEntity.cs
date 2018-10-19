using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Storage.Entities
{
	public class UserEntity
	{

		public int Id { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
	}
}
