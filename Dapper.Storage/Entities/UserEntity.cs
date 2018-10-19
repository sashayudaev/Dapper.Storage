namespace Dapper.Storage.Entities
{
	using Key = Attributes.KeyAttribute;
	using Column = Attributes.ColumnAttribute;
	using Table = Attributes.TableAttribute;

	[@Table("users", Schema = "public")]
	public class UserEntity
	{
		[@Key]
		[@Column("id")]
		public int Id { get; set; }

		[@Column("login")]
		public string Login { get; set; }

		[@Column("password")]
		public string Password { get; set; }
	}
}
