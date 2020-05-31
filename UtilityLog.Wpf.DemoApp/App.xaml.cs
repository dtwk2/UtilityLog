using Splat;
using System;
using System.Windows;
using UtilityLog.View.Infrastructure;

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

            // Need to include this line to make logging work
            // https://reactiveui.net/docs/handbook/logging/

            Locator.CurrentMutable.RegisterConstant(ObservableLogger.Instance, typeof(ILogger));
            Locator.CurrentMutable.RegisterConstant(new CombinedLogger(ObservableLogger.Instance,new UtilityLog.ConsoleLogger()), typeof(ILogger));
            Locator.CurrentMutable.RegisterConstant(CreateDefault(), Constants.LogConnection);

            new UIFreezeObserver().Observe();

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
            var conn = UtilityDAL.Sqlite.ConnectionFactory.Create<Log>(path);
            _ = new SQLiteLogger(conn);
            return conn;
        }
    }
}

