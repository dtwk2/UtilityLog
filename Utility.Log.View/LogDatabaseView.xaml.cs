using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ReactiveUI;
using Splat;
using SQLite;
using Utility.Log.Model;
using Utility.Log.View.Infrastructure;
using Utility.View.Infrastructure;
using Constants = Utility.Log.Infrastructure.Constants;

namespace Utility.Log.View {

    /// <summary>
    /// Interaction logic for LogDatabaseUserControl.xaml
    /// </summary>
    public partial class LogDatabaseView : UserControl {
        private const string Last50 = "Select * from Log limit 50";

        public static readonly DependencyProperty ConnectionProperty = DependencyProperty.Register(nameof(Connection), typeof(SQLiteConnection), typeof(LogDatabaseView), new PropertyMetadata(null));

        public static readonly DependencyProperty ConnectionDirectoryProperty = DependencyProperty.Register("ConnectionDirectory", typeof(string), typeof(LogDatabaseView), new PropertyMetadata(null));

        public LogDatabaseView() {

            InitializeComponent();

            _ = ExpandCollapseToggle.Events().Checked.Select(a => false)
                .Merge(ExpandCollapseToggle.Events().Unchecked.Select(a => true))
                .InvokeCommand(MainLogGrid.ExpandAllGroups);

            ConnectionDirectory ??= Locator.Current.GetService<DirectoryInfo>(Constants.LogDirectory)?.FullName;


            this.LayoutUpdated += (o, e) => {
                if (!IsLoaded && (this.ActualHeight > 0 || this.ActualWidth > 0)) {
                    this.WhenAnyValue(a => a.Connection)
                        .StartWith(Locator.Current.GetService(typeof(SQLiteConnection), nameof(Constants.LogConnection)) as SQLiteConnection)
                        .Where(a => a != null)
                        .Merge(GetConnections())
                        .DistinctUntilChanged(a => a.DatabasePath)
                        .CombineLatest(SelectQueries())
                        .SelectMany(c => Task.Run(() => c.First?.Query<Model.Log>(c.Second).ToArray()).ToObservable())
                        .Select(a => a.GroupBy(log => log.Key).Select(grouping => new LogGroup(grouping.Key, grouping.ToArray())))
                        .ObserveOnDispatcher()
                        .Subscribe(a => this.MainLogGrid.ItemsSource = a);

                }
            };




            IObservable<string> SelectQueries()
            {
                var observable = this.ErrorButton.SelectCheckedChanges(true)
                    .Select(a => $"Select * from Log where Level={(byte) LogLevel.Error}");
                var obsWarn = this.WarnButton.SelectCheckedChanges(true)
                    .Select(a => $"Select * from Log where Level={(byte)LogLevel.Warn}");
                var obsAll = this.AllButton.SelectCheckedChanges(true).Select(a => "Select * from Log");
                var obsLastDay = this.LastDayButton.SelectCheckedChanges(true).Select(a =>
                    "select * from Log where  datetime((Date / 10000000) - 62135553600, 'unixepoch') BETWEEN datetime('now', 'start of day') AND datetime('now', 'localtime');");
                var obsLastRun = this.LastRunButton.SelectCheckedChanges(true)
                    .Select(a => "select * from Log where  RunCount=(select Max(RunCount) from Log)");
                return observable.Merge(obsWarn).Merge(obsAll).Merge(obsLastDay).Merge(obsLastRun);
            }

            IObservable<SQLiteConnection> GetConnections() {
                var connections = LogsComboBox.FileChanges()
                    .Select(a => new SQLiteConnection(a.FullName, true));
                return connections;
            }
        }


        public SQLiteConnection Connection {
            get => (SQLiteConnection)GetValue(ConnectionProperty);
            set => SetValue(ConnectionProperty, value);
        }

        public string ConnectionDirectory {
            get => (string)GetValue(ConnectionDirectoryProperty);
            set => SetValue(ConnectionDirectoryProperty, value);
        }
    }
}