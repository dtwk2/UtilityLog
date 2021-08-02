using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using Pcs.Hfrr.Log;
using Pcs.Hfrr.Log.Model;
using Pcs.Hfrr.Log.View;
using Pcs.Hfrr.Log.View.Infrastructure;
using ReactiveUI;
using SnazzyWpfBorders.Components;
using SnazzyWpfBorders.Extensions;
using Utility.Controls;

namespace Utility.Log.View.Controls {
    public class Window : CustomWindow.Window, IShowExceptionDialog {

        //private MenuItem? menuItem1;
        private LogWindow? logWindow;
        private System.Windows.Controls.ProgressBar? progressBar;
        private DialogHost? dialogHost;

        public static readonly DependencyProperty LogVisibilityProperty = DependencyProperty.Register("LogVisibility", typeof(Visibility), typeof(Window), new PropertyMetadata(Visibility.Hidden));
        private BorderFade BorderWarningFlash;
        private BorderLoop BorderLoop;
        private ToggleButton button;

        static Window() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));
        }

        private static void Window_SourceInitialized(object sender, EventArgs e) {
            _ = new GlobalExceptionHandler(sender as Window);
            _ = new UIFreezeObserver().Observe();
        }

        public Window() {
            this.CommandBindings.Add(new CommandBinding(RoutedCommands.ShowLogPanel, ExecutedCustomCommand, CanExecuteCustomCommand));
            this.SourceInitialized += Window_SourceInitialized;
        }
        public override void OnApplyTemplate() {
            button = this.GetTemplateChild("PART_AddButton") as ToggleButton;
            button
                .Events()
                .Checked.Merge(button.Events().Unchecked)
                .Subscribe(a => {

                    logWindow ??= CreateLogWindow(this);

                    if (button.IsChecked ?? false) {
                        logWindow.Show();
                        // Add gap between flashes
                        BorderWarningFlash.Storyboard.AddDelayToChildren(80);
                        // Set it to repeat forever
                        BorderWarningFlash.Storyboard.RepeatBehavior = RepeatBehavior.Forever;
                        // Start flashing
                        BorderWarningFlash.Start();
                    }
                    else {
                        logWindow.Hide();
                        BorderLoop.Start();
                        // Stop the flash animation
                        BorderWarningFlash.Storyboard.Stop(BorderWarningFlash);
                    }
                });

            //Services.Tracker.Track(this);
            //menuItem1 = this.GetTemplateChild("MenuItem1") as MenuItem;
            BorderWarningFlash = this.GetTemplateChild("BorderWarningFlash") as BorderFade;
            BorderLoop = this.GetTemplateChild("BorderLoop") as BorderLoop;
            progressBar = this.GetTemplateChild("MainProgressBar") as System.Windows.Controls.ProgressBar;
            dialogHost = this.GetTemplateChild("DialogHost1") as DialogHost;
            dialogHost.DialogOpened += DialogHost_DialogOpened;
            var style = dialogHost.Style;
            if (style == null)
                throw new Exception("DialogHost without Style. Dialogs won't display. Try adding MaterialDesign ResourceDictionary to App.xaml ");



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
            //        VisualStateManager.GoToState(this, a.ToString(), true);
            //    });

            MessageBus.Current
               .ListenIncludeLatest<Progress>()
                .ObserveOnDispatcher()
                .Subscribe(a => {
                    progressBar.Value = a.Value;
                    progressBar.IsIndeterminate = a.IsIndeterminate;
                });

            base.OnApplyTemplate();

            static LogWindow CreateLogWindow(Window logHost) {
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
                    logHost.button.IsChecked = false;
                };

                lWindow
                    .WhenAnyValue(a => a.Visibility)
                    .Subscribe(a => {
                        logHost.LogVisibility = a;
                    });

                return lWindow;
            }
        }

        private void DialogHost_DialogOpened(object sender, DialogOpenedEventArgs eventArgs) {
            BorderLoop.Start();
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

            //var result = await DialogHost.Show(new ExceptionDialog());
            var result = await DialogHost.Show(new ExceptionDialog(exception, message));
           // var result = await DialogHost.Show(new Ellipse { Fill = Brushes.Red, Height = 20, Width = 199 });
            return (bool)result;
            //var result  =  await DialogHost.Show("dsffd", async (object _, DialogOpenedEventArgs args) => {
            //    await Task.Delay(TimeSpan.FromSeconds(6));
            //    args.Session.Close();
            //});
            //return true;

        }
    }
}
