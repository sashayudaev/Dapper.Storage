using Dapper.Storage.Attributes;

namespace Dapper.Storage.Procedures
{
	public class GetUserProcedure : ProcedureBase
	{
		[ProcedureParameter("user_id")]
		public int Id { get; }

		public GetUserProcedure(int id) 
			: base("getuser")
		{
			Id = id;
		}
	}
}
