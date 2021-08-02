//using Forge.Forms;
using ReactiveUI;
using Splat;
using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Pcs.Hfrr.Log.View.Infrastructure
{
    public class GlobalExceptionHandler : IEnableLogger
    {
        private readonly IShowExceptionDialog showExceptionDialog;

        public GlobalExceptionHandler(IShowExceptionDialog showExceptionDialog)
        {
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;

            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            new UIFreezeObserver().Observe();
            this.showExceptionDialog = showExceptionDialog;

            //do something with the file contents
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            this.Log().Error(e.Exception);
        }

        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.Log().Error((Exception)e.ExceptionObject, "Unhandled exception: ");

            var message = "Unhandled exception occured.\n";

            if (e.IsTerminating)
            {
                message += "\nApplication will shutdown.\n";
                this.Log().Info(e.ExceptionObject as Exception, message);
                Process.GetCurrentProcess().Kill();
            }
        }

        private async void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.Log().Error(e.Exception, "An unhandled exception occurred");

            IDisposable disposable = null;
            disposable = showExceptionDialog.ShowExceptionDialog(e.Exception).ToObservable().Subscribe(a =>
            {
                this.Log().Error(e.Exception, "App will " + (a ? string.Empty : "not") + " shutdown");

                if (a)
                {
                    RxApp.MainThreadScheduler.Schedule(() => Application.Current.Shutdown());
                }
                else
                {
                    MessageBus.Current.SendMessage(Validity.Invalid);
                }
                disposable?.Dispose();
            });

            e.Handled = true;

            if (!e.Handled)
            {
                this.Log().Error(e.Exception, "App will shutdown");
            }
        }
    }
}