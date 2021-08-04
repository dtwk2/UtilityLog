using System;
using System.IO;
using Splat;
using Utility.Log.Logger;

namespace Utility.Log.Infrastructure
{
    public class BootStrapper : IEnableLogger
    {
        public static void Register()
        {
            Locator.CurrentMutable.RegisterConstant(CreateConnection(), Constants.LogConnection);
            Locator.CurrentMutable.RegisterConstant(ConnectionDirectory, Constants.LogDirectory);
            var sqliteLogger = new SQLiteLogger(Locator.Current.GetService<SQLite.SQLiteConnection>(Constants.LogConnection));
            Locator.CurrentMutable.RegisterConstant<IObservableLogger>(sqliteLogger);

            //var log = new LoggerConfiguration()
            //    .WriteTo.Debug()
            //    .WriteTo.File(directory + "\\Log.txt", rollingInterval: RollingInterval.Day)
            //    .CreateLogger();

            //Locator.CurrentMutable.UseSerilogFullLogger();

            //log.Information("Serilog configured");

            var logger = Locator.Current.GetService<Splat.ILogger>();
            Locator.CurrentMutable.Register<Splat.ILogger>(() => new CombinedLogger(logger, sqliteLogger));
        }

        private static SQLite.SQLiteConnection CreateConnection()
        {
            string path = ConnectionDirectory.FullName + $"/Log_{DateTime.Now:yyyy-MM-dd}.sqlite";
            var conn = ConnectionFactory.Create<Model.Log>(path);
            return conn;
        }

        public static DirectoryInfo ConnectionDirectory => Directory.CreateDirectory(@"..\..\..\Log");
    }
}