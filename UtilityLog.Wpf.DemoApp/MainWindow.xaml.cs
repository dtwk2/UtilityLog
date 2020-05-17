using System;
using System.Reactive.Linq;
using System.Windows;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace UtilityLog.Wpf.DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string path = "../../../Data/Log.sqlite";

        public MainWindow()
        {
            InitializeComponent();
            SQLitePCL.Batteries.Init();
            var conn = UtilityDAL.Sqlite.ConnectionFactory.Create<Log>(path);
            _ = new SQLiteLogger(conn);

            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(a =>
            ObservableLogger.Instance.Write(GetRandomString(), GetRandomEnum<Splat.LogLevel>()));

            DataGrid1.ItemsSource = conn.Table<Log>().ToArray();
        }
    }
}
