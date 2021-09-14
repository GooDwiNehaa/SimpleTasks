using LinqToDB.Mapping;

namespace Fogsoft.SimpleTasks.Library
{
	/// <summary>
	/// Пользователь. Сейчас исполнитель задачи, а в будущем может использоваться и для других целей.
	/// </summary>
	[Table("Users")]
	public class User
	{
		[PrimaryKey]
		[NotNull]
		public long Id { get; set; }

		[Column, NotNull]
		public string Name { get; set; }
	}
}