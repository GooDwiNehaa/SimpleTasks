using LinqToDB.Mapping;

namespace Fogsoft.SimpleTasks.Library
{
	[Table("TasksUsers")]
	public class TaskUser
	{
		[PrimaryKey]
		[NotNull]
		public long TaskId { get; set; }
		[PrimaryKey]
		[NotNull]
		public long AssigneeId { get; set; }
	}
}
