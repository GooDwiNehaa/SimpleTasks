/* Это класс, отвечающий за бизнес-логику по созданию задач.
 * В реальном проекте он использовался бы одним или несколькими сервисами, предоставляющими API для клиентских приложений.
 *
 * Чтобы не усложнять тестовое задание приняты следующие упрощения (их дорабатывать не нужно):
 * - в методах GetX не предусмотрен пейджинг;
 * - отсутствует удаление задач и пользователей;
 * - отсутствует проверка прав, лог действий и ошибок;
 * - не предусмотрена многопользовательская работа;
 * - используется БД SQLite.
 */
using System.Linq;
using LinqToDB;
using LinqToDB.Linq;

namespace Fogsoft.SimpleTasks.Library
{
	/// <summary>
	/// Предоставляет методы для работы с задачами.
	/// </summary>
	public class TaskManager
	{
		/// <summary>
		/// Добавляет новую задачу в БД.
		/// </summary>
		public Task Add(string subject, string description)
		{
			using var db = Db.Open();
			var task = new Task
			{
				Id = db.Tasks.Select(t => t).Count() + 1,
				Description = description,
				Subject = subject
			};

			db.Insert(task);
			return task;
		}

		/// <summary>
		/// Назначает исполнителя на задачу.
		/// </summary>
		public void Assign(long taskId, long[] assigneeIds)
		{
			using var db = Db.Open();

			var task = db.Tasks.Where(t => t.Id == taskId);
			var users = db.Users.Where(u => assigneeIds.Contains(u.Id));

			if (task != null && users != null)
			{
				for (int i = 0; i < assigneeIds.Length; i++)
				{
					db.TasksUsers.Insert(() => new TaskUser { TaskId = taskId, AssigneeId = assigneeIds[i] });
				}
			}
		}

		/// <summary>
		/// Снимает исполнителя с задачи.
		/// </summary>
		public void Unassign(long taskId, long assigneeId)
		{
			using var db = Db.Open();

			db.TasksUsers
				.Where(tu => tu.TaskId == taskId && tu.AssigneeId == assigneeId)
				.Delete();
		}

		/// <summary>
		/// Возвращает все задачи.
		/// </summary>
		public Task[] GetAllTasks()
		{
			using var db = Db.Open();
			return db.Tasks.ToArray();
		}

		/// <summary>
		/// Возвращает все задачи и их исполнителей.
		/// </summary>
		public TaskUser[] GetAllTasksUsers()
		{
			using var db = Db.Open();
			return db.TasksUsers.ToArray();
		}

		/// <summary>
		/// Возвращает все задачи, назначенные на конкретного исполнителя.
		/// </summary>
		public Task[] GetByAssignee(long assigneeId)
		{
			using var db = Db.Open();

			var tasksIds = db.TasksUsers
				.Where(tu => tu.AssigneeId == assigneeId)
				.Select(tu => tu.TaskId).ToArray();

			return db.Tasks.Where(t => tasksIds.Contains(t.Id)).ToArray();
		}
	}
}