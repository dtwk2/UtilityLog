using Splat;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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


            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            //do something with the file contents
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // Log the exception, display it, etc
            //Debug.WriteLine(e.Exception.Message);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.Log().Error(e);
            // Log the exception, display it, etc
           // Debug.WriteLine((e.ExceptionObject as Exception).Message);
        }
    }
}
