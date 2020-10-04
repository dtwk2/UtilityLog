using Splat;
using System;
using System.Windows;
using UtilityLog.View.Infrastructure;
using UtilityLog.Wpf.DemoApp.Utility;

namespace UtilityLog.Wpf.DemoApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IEnableLogger
    {

        public App()
        {

            SQLitePCL.Batteries.Init();

            Register();

            new UIFreezeObserver().Observe();

        }

        static void Register()
        {
            Locator.CurrentMutable.RegisterConstant(CreateDefault(), Constants.LogConnection);
            var sqliteLogger = new SQLiteLogger(Locator.Current.GetService<SQLite.SQLiteConnection>(Constants.LogConnection));
            Locator.CurrentMutable.RegisterConstant<ILogger>(sqliteLogger);
            Locator.CurrentMutable.RegisterConstant<IObservableLogger>(sqliteLogger);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            _ = new GlobalExceptionHandler(window.MainView);

        }


        static SQLite.SQLiteConnection CreateDefault()
        {
            const string path = "../../../Data/Log.sqlite";
            var conn = ConnectionFactory.Create<Log>(path);
            return conn;
        }
    }
}

