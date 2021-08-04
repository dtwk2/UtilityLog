using System;
using System.IO;
using System.Reactive.Linq;
using HandyControl.Controls;
using HandyControl.Data;
using ReactiveUI;
using Observable = System.Reactive.Linq.Observable;

namespace Utility.Controls.View
{
    /// <summary>
    /// Interaction logic for ExportView.xaml
    /// </summary>
    public partial class ExportView 
    {
        public ExportView()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
               ExportDirectoryLabel.Content = Directory.CreateDirectory("../../../Data").FullName;

               _ = MainNumeric
                  .SelectIntegerValues()
                  .StartWith((int) (MainNumeric.Value))
                  .InvokeCommand(this.ViewModel, a => a.ExportQuantity);

               this.BindCommand(this.ViewModel, vm => vm.Export, v => v.Export);
               
               Observable.Return(ExportDirectoryLabel.Content).InvokeCommand(this.ViewModel, a => a.ExportDirectory);

               this.OneWayBind(this.ViewModel, vm => vm.IsEnabled, v => v.Export.IsEnabled);

               this.OneWayBind(this.ViewModel, vm => vm.Progress, v => v.MainProgressBar.Value);

            });
        }

      
    }

    static class TelerikHelper
    {
       public static IObservable<int> SelectIntegerValues(this NumericUpDown upDown) {
          return Observable
             .FromEventPattern<EventHandler<FunctionEventArgs<double>>, FunctionEventArgs<double>>(
                a => upDown.ValueChanged += a, a => upDown.ValueChanged -= a)
             .Select(a => (int)(a.EventArgs.Info));
       }
   }
}