namespace Dapper.Storage.Entities
{
	using Table = Attributes.TableAttribute;
	using Key = Attributes.KeyAttribute;
	using Column = Attributes.ColumnAttribute;

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
