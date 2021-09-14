using System.IO;
using System.Reflection;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;

namespace Fogsoft.SimpleTasks.Library
{
	/// <summary>
	/// Набор таблиц для ORM.
	/// </summary>
	public class Db : DataConnection
	{
		static Db()
		{
			AddDataProvider(new SQLiteDataProvider("SQLite"));
		}

		public static Db Open()
		{
			var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var dbFile = Path.Combine(currentDirectory, "db.sqlite");
			return new Db($"Data Source={dbFile}");
		}

		private Db(string connectionString)
			: base("SQLite", connectionString)
		{

		}

		public ITable<Task> Tasks => GetTable<Task>();

		public ITable<User> Users => GetTable<User>();
		public ITable<TaskUser> TasksUsers => GetTable<TaskUser>();
	}
}