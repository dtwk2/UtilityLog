using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UtilityLog.View.Infrastructure;

namespace UtilityLog.View
{
    public class LogHost : ContentControl, Infrastructure.IShowExceptionDialog
    {

        private ReplaySubject<Visibility> visibilityChanges = new ReplaySubject<Visibility>();
        private LogView logView1;
        private MenuItem menuItem1;

        public static readonly DependencyProperty LogVisibilityProperty = DependencyProperty.Register("LogVisibility", typeof(Visibility), typeof(LogHost), new PropertyMetadata(Visibility.Visible, Changed));

        static LogHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LogHost), new FrameworkPropertyMetadata(typeof(LogHost)));
        }

        public LogHost()
        {
        
            this.CommandBindings.Add(new CommandBinding(RoutedCommands.ShowLogPanel, ExecutedCustomCommand, CanExecuteCustomCommand));
        }

        public override void OnApplyTemplate()
        {
            Services.Tracker.Track(this);
            logView1 = this.GetTemplateChild("LogView1") as LogView;
            menuItem1 = this.GetTemplateChild("MenuItem1") as MenuItem;
            var border1 = this.GetTemplateChild("Border1") as Border;
            menuItem1.IsChecked = LogVisibility == Visibility.Visible;
            visibilityChanges.Subscribe(a =>
            logView1.Visibility = a);
            MessageBus.Current.ListenIncludeLatest<Validity>()
                .ObserveOnDispatcher()
                .Subscribe(a =>
            {
                VisualStateManager.GoToState(this, a.ToString(), true);
            });

            base.OnApplyTemplate();
        }



        public Visibility LogVisibility
        {
            get { return (Visibility)GetValue(LogVisibilityProperty); }
            set { SetValue(LogVisibilityProperty, value); }
        }


        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LogHost logHost)
            {
                logHost.visibilityChanges.OnNext((Visibility)e.NewValue);
                //Services.Tracker.Persist(logHost);
            }

        }

        private void ExecutedCustomCommand(object sender, ExecutedRoutedEventArgs e)
        {
            LogVisibility = LogVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            menuItem1.IsChecked = LogVisibility == Visibility.Visible;
        }

        private void CanExecuteCustomCommand(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = e.Source is Control;


        public async System.Threading.Tasks.Task<bool> ShowExceptionDialog(Exception exception)
        {
            string message = "Close Application (or leave in unstable state)?";

            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(new View.ExceptionHost(exception, message));
            return (bool)result;
        }

    }
}
