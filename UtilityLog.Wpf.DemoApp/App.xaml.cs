using Splat;
using System.Windows;
using UtilityLog.View.Infrastructure;

namespace UtilityLog.Wpf.DemoApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application   
    {

        public App()
        {
            // Need to include this line to make logging work
            // https://reactiveui.net/docs/handbook/logging/
            Locator.CurrentMutable.RegisterConstant(ObservableLogger.Instance, typeof(ILogger));

            _ = new GlobalExceptionHandler();
        }
    }
}

