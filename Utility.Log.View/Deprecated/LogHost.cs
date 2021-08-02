#nullable enable

using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Pcs.Hfrr.Log;
using Pcs.Hfrr.Log.Model;
using Pcs.Hfrr.Log.View;
using Pcs.Hfrr.Log.View.Infrastructure;
using ReactiveUI;
using Splat;
using Utility.Controls;

namespace Utility.Log.View.Controls {
   public class LogHost : ContentControl, IShowExceptionDialog {
      private MenuItem? menuItem1;
      private LogWindow? logWindow;
      private System.Windows.Controls.ProgressBar? progressBar;
      private DialogHost? dialogHost;

      public static readonly DependencyProperty LogVisibilityProperty = DependencyProperty.Register("LogVisibility", typeof(Visibility), typeof(LogHost), new PropertyMetadata(Visibility.Hidden));

      static LogHost() {
         DefaultStyleKeyProperty.OverrideMetadata(typeof(LogHost), new FrameworkPropertyMetadata(typeof(LogHost)));
      }

      public LogHost() {
         this.CommandBindings.Add(new CommandBinding(RoutedCommands.ShowLogPanel, ExecutedCustomCommand, CanExecuteCustomCommand));
      }

      public override void OnApplyTemplate() {
         //Services.Tracker.Track(this);
         menuItem1 = this.GetTemplateChild("MenuItem1") as MenuItem;
         progressBar = this.GetTemplateChild("MainProgressBar") as System.Windows.Controls.ProgressBar;
         dialogHost = this.GetTemplateChild("DialogHost1") as DialogHost;
         dialogHost.DialogOpened += DialogHost_DialogOpened;
         var style = dialogHost.Style;
         if (style == null)
            throw new Exception("DialogHost without Style. Dialogs won't display. Try adding MaterialDesign ResourceDictionary to App.xaml ");

         this.WhenAnyValue(a => a.LogVisibility)
             .Subscribe(a => {
                logWindow ??= CreateLogWindow(this);

                if (a == Visibility.Visible)
                   logWindow.Show();
                else
                   logWindow.Hide();

                if (menuItem1 != null)
                   menuItem1.IsChecked = LogVisibility == Visibility.Visible;
             });

         MessageBus
            .Current
            .ListenIncludeLatest<Validity>().Scan((a, b) => {
               if (a == Validity.Invalid && b == Validity.Valid) {

               }

               return b;
            });

         //MessageBus
         //   .Current
         //   .ListenIncludeLatest<Validity>()
         //    .ObserveOnDispatcher()
         //    .Subscribe(a => {
         //       VisualStateManager.GoToState(this, a.ToString(), true);
         //    });

         MessageBus.Current
            .ListenIncludeLatest<Progress>()
             .ObserveOnDispatcher()
             .Subscribe(a => {
                progressBar.Value = a.Value;
                progressBar.IsIndeterminate = a.IsIndeterminate;
             });

         base.OnApplyTemplate();

         static LogWindow CreateLogWindow(LogHost logHost) {
            var lWindow = Application.Current.MainWindow is { } mainWindow
                ? new LogWindow {
                   Height = mainWindow.Height,
                   Width = mainWindow.Width / 2,
                   Owner = mainWindow,
                }
                : new LogWindow();

            lWindow.Closing += (s, e) => {
               e.Cancel = true;
               lWindow.Hide();
            };

            lWindow.WhenAnyValue(a => a.Visibility).Subscribe(a => {
               logHost.LogVisibility = a;
            });

            return lWindow;
         }
      }

      private void DialogHost_DialogOpened(object sender, DialogOpenedEventArgs eventArgs) {

      }

      public Visibility LogVisibility {
         get => (Visibility)GetValue(LogVisibilityProperty);
         set => SetValue(LogVisibilityProperty, value);
      }

      private void ExecutedCustomCommand(object sender, ExecutedRoutedEventArgs e) {
         LogVisibility = LogVisibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
      }

      private void CanExecuteCustomCommand(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = e.Source is Control;

      public async System.Threading.Tasks.Task<bool> ShowExceptionDialog(Exception exception) {
         const string message = "Close Application (or leave in unstable state)?";

         var result = await MaterialDesignThemes.Wpf.DialogHost.Show(new ExceptionDialog(exception, message));
         return (bool)result;
      }
   }
}