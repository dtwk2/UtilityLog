using Forge.Forms;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace UtilityLog.View.Infrastructure
{
    public class GlobalExceptionHandler : IEnableLogger
    {

        public GlobalExceptionHandler()
        {

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnCurrentDomainUnhandledException);

            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            new UIFreezeObserver().Observe();

            //do something with the file contents
        }


        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            this.Log().Error(e);

        }


        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.Log().Error((Exception)e.ExceptionObject, "Unhandled exception: ");

            var message = "Unhandled exception occured.\n";

            if (e.IsTerminating)
            {
                message += "\nApplication will shutdown.\n";
                Process.GetCurrentProcess().Kill();
            }
        }


        private async void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.Log().Error(e.Exception, "An unhandled exception occurred");

            var message = "Unhandled exception occured.\n";

            var x  = await Show.Dialog().For(new Alert("Hello world!"));
            e.Handled = x.Model.Confirmed;


            if (!e.Handled)
            {
                message += "\nApplication will shutdown.\n";
            }

          
        }
    }
}
