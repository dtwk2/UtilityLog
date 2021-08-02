using ReactiveUI;
using Splat;
using SQLite;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Pcs.Hfrr.Log.View
{
    /// <summary>
    /// Interaction logic for LogDbView.xaml
    /// </summary>
    public partial class LogDbView : UserControl
    {
        public static readonly DependencyProperty ConnectionProperty = DependencyProperty.Register(nameof(Connection), typeof(SQLiteConnection), typeof(LogDbView), new PropertyMetadata(null));

        public static readonly DependencyProperty ConnectionDirectoryProperty = DependencyProperty.Register("ConnectionDirectory", typeof(string), typeof(LogDbView), new PropertyMetadata(null));

        public LogDbView()
        {
            InitializeComponent();
            //this.Loaded += (s, _) => this.SetItemsSource(Connection);
            var obs = this.ErrorButton.Events().Click.Select(a => $"Select * from Log where Level={(byte)LogLevel.Error}");
            var obs2 = this.WarnButton.Events().Click.Select(a => $"Select * from Log where Level={(byte)LogLevel.Warn}");
            var obs3 = this.AllButton.Events().Click.Select(a => "Select * from Log");
            var obs4 = this.LastDayButton.Events().Click.Select(a => "select * from Log where  datetime((Date / 10000000) - 62135553600, 'unixepoch') BETWEEN datetime('now', 'start of day') AND datetime('now', 'localtime');");
            var obs5 = this.LastRunButton.Events().Click.Select(a => "select * from Log where  RunCount=(select Max(RunCount) from Log)");

            this.WhenAnyValue(a => a.ConnectionDirectory)
                .Where(a => a != null)
                .Subscribe(a =>
                {
                    this.LogsComboBox.ItemsSource = new DirectoryInfo(a).GetFiles("Log_*.sqlite");
                });

            var connections = LogsComboBox.Events().SelectionChanged
                .SelectMany(a => a.AddedItems.Cast<FileInfo>())
                .Where(a => a != null)
                .Select(a =>
                {
                    return new SQLiteConnection(a.FullName);
                });

            this.WhenAnyValue(a => a.Connection)
                .StartWith(Locator.Current.GetService(typeof(SQLiteConnection), nameof(Constants.LogConnection)) as SQLiteConnection)
                .Where(a => a != null)
                .Merge(connections)
                .CombineLatest(obs.Merge(obs2).Merge(obs3).Merge(obs4).Merge(obs5).StartWith("Select * from Log limit 50"), (a, b) => (a, b))
                .SelectMany(c => Task.Run(() =>
                    c.a.Query<Log>(c.b).ToArray()).ToObservable())
                .ObserveOnDispatcher()
                .Subscribe(a => this.DataGrid1.ItemsSource = a);
        }

        public SQLiteConnection Connection
        {
            get => (SQLiteConnection)GetValue(ConnectionProperty);
            set => SetValue(ConnectionProperty, value);
        }

        public string ConnectionDirectory
        {
            get => (string)GetValue(ConnectionDirectoryProperty);
            set => SetValue(ConnectionDirectoryProperty, value);
        }
    }
}