using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UtilityLog.View
{
    /// <summary>
    /// Interaction logic for LogDbView.xaml
    /// </summary>
    public partial class LogDbView : UserControl
    {

        public static readonly DependencyProperty ConnectionProperty = DependencyProperty.Register("Connection", typeof(SQLiteConnection), typeof(LogDbView), new PropertyMetadata(null, Changed));

        public LogDbView()
        {
            InitializeComponent();
            this.Loaded += (s,_) => this.SetItemsSource(Connection); ;
        }

        public SQLiteConnection Connection
        {
            get { return (SQLiteConnection)GetValue(ConnectionProperty); }
            set { SetValue(ConnectionProperty, value); }
        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is LogDbView logDbView && e.NewValue is SQLiteConnection conn)
            {
                logDbView.SetItemsSource(conn);
            }
        }

        private void SetItemsSource(SQLiteConnection conn)
        {
            this.DataGrid1.ItemsSource = Connection?.Table<Log>().ToArray();
        }

      
    }
}
