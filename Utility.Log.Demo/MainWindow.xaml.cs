//using ReactiveUI;
//using Splat;
//using System;
//using System.Reactive.Linq;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using Splat;
using Utility.Controls;
using Utility.Log.Demo.ViewModel;
using Utility.Log.Model;
using Utility.ViewModel;

namespace Utility.Log.Demo {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : IEnableLogger {
      public MainWindow() {
         InitializeComponent();
         this.Log().Info("MainWindow Initialized");
         var viewModel = new MainViewModel();
         SendException.Command = viewModel.command;
         SendUnhandledException.Command = viewModel.command2;
         TestDialogHost.Events().Click.Subscribe(ShowLoadingDialogWithProgress);
      }

      private async void ShowLoadingDialogWithProgress(RoutedEventArgs _) {
         const int arbitraryDelayInSeconds = 2;
         var dialogContent = new TextBlock { Text = $"Loading in {arbitraryDelayInSeconds}s .... Please wait.", Margin = new Thickness(20) };
         this.Log().Info(nameof(ShowLoadingDialogWithProgress));
         MessageBus.Current.SendMessage(new Progress(nameof(MainWindow), 50, true));

         await DialogHost.Show(dialogContent, async (object _, DialogOpenedEventArgs args) => {
            await Task.Delay(TimeSpan.FromSeconds(arbitraryDelayInSeconds));
            args.Session.Close();
         });

         MessageBus.Current.SendMessage(new Progress(nameof(MainWindow), 0, false));

         ExceptionDialog.Content = new ExceptionDialog();
      }
   }
}