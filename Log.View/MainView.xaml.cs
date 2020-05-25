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
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }


        public object MainContent
        {
            get { return (object)GetValue(MainContentProperty); }
            set { SetValue(MainContentProperty, value); }
        }

        public static readonly DependencyProperty MainContentProperty = DependencyProperty.Register("MainContent", typeof(object), typeof(MainView), new PropertyMetadata(null));


        private void ExecutedCustomCommand(object sender,    ExecutedRoutedEventArgs e)
        {
            logView1.Visibility = (bool?)e.Parameter?? false ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CanExecuteCustomCommand(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = e.Source is Control;

    }
}
