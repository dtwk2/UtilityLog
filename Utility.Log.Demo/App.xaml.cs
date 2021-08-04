//using Pcs.Hfrr.Log.View.Infrastructure;
//using Splat;

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Splat;

namespace Utility.Log.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application , IEnableLogger
    {
      public App() {
         SQLitePCL.Batteries.Init();

         Utility.Log.Infrastructure.BootStrapper.Register();
         this.Log().Info($"Log level is {this.Log().Level}");
     
      }

      protected override void OnStartup(StartupEventArgs e) {
         MainWindow window = new MainWindow();
         window.Show();
         //_ = new GlobalExceptionHandler(window);
      }
   }

    internal class BooleanAllConverter : IMultiValueConverter {
       public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
          => values.OfType<bool>().All(b => b);

       public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
          => throw new NotImplementedException();
    }


}