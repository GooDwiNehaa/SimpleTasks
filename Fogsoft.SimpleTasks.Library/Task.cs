using LinqToDB.Mapping;

namespace Fogsoft.SimpleTasks.Library
{
	/// <summary>
	/// Задача. Может быть назначена на <see cref="User"/>, а может быть ни на кого не назначена.
	/// </summary>
	[Table("Tasks")]
	public class Task
	{
		[PrimaryKey]
		[NotNull]
		public long Id { get; set; }

		[Column, NotNull]
		public string Subject { get; set; }

		[Column, Nullable]
		public string Description { get; set; }
	}
}