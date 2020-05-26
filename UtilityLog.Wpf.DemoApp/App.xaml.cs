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
            // Need to include this line to make logging work
            // https://reactiveui.net/docs/handbook/logging/

            Locator.CurrentMutable.RegisterConstant(ObservableLogger.Instance, typeof(ILogger));

            new UIFreezeObserver().Observe();


        }


        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            _ = new GlobalExceptionHandler(window);

        }




        //public static void ShowError(Exception exception, string message, string caption = "")
        //{
        //    var exceptionViewer = new object();// new ExceptionViewer(message, exception);
        //    var baseDialogWindow = new DialogWindow
        //    {
        //        Title = string.IsNullOrEmpty(caption) ? "Error" : caption,
        //        Content = exceptionViewer,
        //        WindowStartupLocation = WindowStartupLocation.CenterScreen,
        //        ResizeMode = ResizeMode.CanResizeWithGrip,
        //        MinHeight = 400,
        //        MinWidth = 500,
        //        ShowMinButton = false,
        //        ShowMaxRestoreButton = false,
        //        ShowInTaskbar = false
        //    };
        //    baseDialogWindow.ShowDialog();

        //    // MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        //}
    }
}

