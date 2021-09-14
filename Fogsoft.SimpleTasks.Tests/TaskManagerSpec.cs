using System.IO;
using System.Linq;
using System.Reflection;
using Fogsoft.SimpleTasks.Library;
using LinqToDB.Data;
using NUnit.Framework;

namespace Fogsoft.SimpleTasks.Tests
{
	public class TaskManagerSpec
	{
		private User[] users;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			// всегда удаляем тестовую БД, если есть
			var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var dbFile = Path.Combine(currentDirectory, "db.sqlite");
			if (File.Exists(dbFile))
				File.Delete(dbFile);

			// запускаем скрипт, создающий тестовую БД
			var scriptPath = Path.Combine(currentDirectory, @"..\..\..\..\CreateTables.sql");
			var sql = File.ReadAllText(scriptPath);
			using var db = Db.Open();
			db.Execute(sql);
			users = db.Users.Select(u => u).ToArray();
		}

		[Test]
		public void Typical_scenario_must_work()
		{
			var taskManager = new TaskManager();

			var tasks = taskManager.GetAllTasks();
			Assert.AreEqual(tasks.Length, 0, "сначала задач не должно быть");

			var newTask1 = taskManager.Add("test1", "description1");
			tasks = taskManager.GetAllTasks();
			Assert.AreEqual(tasks.Length, 1, "должна быть одна новая задача");
			AssertTask(tasks[0], newTask1);

			// проверка на каком пользователе какая задача
			AssertAssignee(taskManager, users);

			// назначение на задачу1 пользователей User0, User1, User2
			taskManager.Assign(newTask1.Id, new long[] { users[0].Id, users[1].Id, users[2].Id });
			AssertAssignee(taskManager, users);
			// проверка того, что нужная задача назначена именно на нажного пользователя
			AssertTaskAssignee(taskManager.GetAllTasksUsers(), users);

			var newTask2 = taskManager.Add("test2", "description2");
			tasks = taskManager.GetAllTasks();
			Assert.AreEqual(tasks.Length, 2, "должна быть одна новая задача(итого:всего 2 задачи)");
			AssertTask(tasks[1], newTask2);

			taskManager.Assign(newTask2.Id, new long[] { users[2].Id, users[3].Id, users[4].Id });
			AssertAssignee(taskManager, users);
			AssertTaskAssignee(taskManager.GetAllTasksUsers(), users);

			// снятие пользователей User0, User1, User2 с задачи1
			taskManager.Unassign(newTask1.Id, users[0].Id);
			taskManager.Unassign(newTask1.Id, users[1].Id);
			taskManager.Unassign(newTask1.Id, users[2].Id);

			tasks = taskManager.GetAllTasks();
			Assert.AreEqual(tasks.Length, 2, "задачи все также 2");
			AssertTask(tasks[0], newTask1);
			AssertTask(tasks[1], newTask2);

			AssertAssignee(taskManager, users);

			taskManager.Unassign(newTask2.Id, users[2].Id);
			taskManager.Unassign(newTask2.Id, users[3].Id);
			taskManager.Unassign(newTask2.Id, users[4].Id);

			tasks = taskManager.GetAllTasks();
			Assert.AreEqual(tasks.Length, 2, "задачи все также 2");
			AssertTask(tasks[0], newTask1);
			AssertTask(tasks[1], newTask2);

			AssertAssignee(taskManager, users);
		}

		private static void AssertTask(Task actual, Task expected)
		{
			Assert.That(actual.Id, Is.EqualTo(expected.Id), "Id должно совпадать");
			Assert.That(actual.Subject, Is.EqualTo(expected.Subject), "Subject должно совпадать");
			Assert.That(actual.Description, Is.EqualTo(expected.Description), "Description должно совпадать");
		}

		private static void AssertAssignee(TaskManager taskManager, User[] users)
		{
			for (int i = 0; i < users.Length; i++)
			{
				var taskCountOnUser = taskManager.GetByAssignee(users[i].Id).Length;

				switch (taskCountOnUser)
				{
					case 0:
						Assert.AreEqual(taskCountOnUser, 0, $"ни одна задача не должна быть назначена на {users[i].Name}");
						break;
					case 1:
						Assert.AreEqual(taskCountOnUser, 1, $"задача1 должна быть назначена на {users[i].Name}");
						break;
					case 2:
						Assert.AreEqual(taskCountOnUser, 2, $"задача1 и задача2 должны быть назначены {users[i].Name}");
						break;
					default:
						break;
				}
			}
		}

		private static void AssertTaskAssignee(TaskUser[] taskUsers, User[] users)
		{
			var tasks = taskUsers
				.GroupBy(tu => tu.TaskId)
				.ToArray();

			int j = 0;

			for (int i = 0; i < tasks.Length; i++)
			{
				foreach (var item in tasks[i])
				{
					Assert.AreEqual(item.AssigneeId, users[j].Id, $"исполнителем задачи{item.TaskId} назначен {users[j].Name}");
					j++;
				}
				j--;
			}
		}
	}
}