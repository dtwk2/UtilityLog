using System;
using System.Linq;
using SQLite;

namespace Utility.Log.Infrastructure
{
    public class ConnectionFactory
    {
        private const string DefaultDbDirectory = "../../../Data";
        private const string SqliteDbExtension = "sqlite";

        public static SQLiteConnection Create<T>(string path = null, Func<Type, bool> func = null)
        {
            return Create(string.IsNullOrEmpty(path) ? $"{DefaultDbDirectory}{typeof(T).Name}.{SqliteDbExtension}" : path, GetTypes());

            Type[] GetTypes() =>
                typeof(T).Assembly.GetTypes()
                    .Where(func ?? (type => type.GetMethods().Any() == false))
                    .ToArray();
        }

        public static SQLiteConnection Create(string path, params Type[] types)
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            SQLiteConnection conn = new SQLiteConnection(path);

            foreach (var type in types)
            {
                conn.CreateTable(type, CreateFlags.AutoIncPK);
            }

            return conn;
        }
    }
}