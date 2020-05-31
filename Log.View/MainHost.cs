using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UtilityLog.View
{
    public class MainHost : ContentControl, Infrastructure.IShowExceptionDialog
    {
        private LogView logView1;

        static MainHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainHost), new FrameworkPropertyMetadata(typeof(MainHost)));
        }

        public MainHost()
        {
            this.CommandBindings.Add(new CommandBinding(
                Infrastructure.RoutedCommands.ShowToolsPanel,
                ExecutedCustomCommand,
                CanExecuteCustomCommand));
        }

        public override void OnApplyTemplate()
        {
            logView1 = this.GetTemplateChild("LogView1") as LogView;
            base.OnApplyTemplate();
        }

        private void ExecutedCustomCommand(object sender, ExecutedRoutedEventArgs e)
        {
            logView1.Visibility = (bool?)e.Parameter ?? false ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void CanExecuteCustomCommand(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = e.Source is Control;


        public async System.Threading.Tasks.Task<bool> ShowExceptionDialog(Exception exception)
        {
            string message = "Close Application (or leave in unstable state)?";

            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(new View.ExceptionHost(exception, message));
            return (bool)result;
        }

    }
}
