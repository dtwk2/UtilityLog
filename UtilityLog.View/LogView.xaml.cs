using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Controls;
using Splat;
using System.Windows.Input;
using System.Windows;
using System.Reactive;

namespace UtilityLog.View
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl, IEnableLogger
    {

        public static readonly DependencyProperty ScrollCommandProperty = DependencyProperty.Register("ScrollCommand", typeof(ICommand), typeof(LogView), new PropertyMetadata(null));


        public static readonly DependencyProperty ClearAllCommandProperty = DependencyProperty.Register("ClearAllCommand", typeof(ICommand), typeof(LogView), new PropertyMetadata(null));

        static LogView()
        {
        }

        public LogView()
        {
            InitializeComponent();
            var scrollCommand = ReactiveUI.ReactiveCommand.Create<bool?, bool>(a => !(a ?? false));
            var clearAllCommand = ReactiveUI.ReactiveCommand.Create<Unit, Guid>(a => Guid.NewGuid());
            var lines = Locator.Current.GetService<IObservableLogger>()
         .Messages
         .SelectMany(Selector)
         .Pace(TimeSpan.FromSeconds(0.5))
         .WithLatestFrom(scrollCommand.StartWith((PlayPause1.IsChecked == false)), (a, b) => (a, b))
         .CombineLatest(clearAllCommand.StartWith(default(Guid)), (a, c) => (a.a, a.b, c))
         .ScanChanges(a => a.c)
         .Subscribe(d =>
         {
             var ((newContent, scoll, _), clear) = d;
             this.Dispatcher.InvokeAsync(() =>
             {
                 try
                 {
                     if (clear)
                         logOutputTextBox.Clear();
                     else
                     {
                         logOutputTextBox.AppendText(newContent);
                         if (scoll)
                             logOutputTextBox.ScrollToEnd();
                         if (newContent.ToLower().Contains("[") == false && newContent.ToLower().Contains("]") == false)
                             logOutputTextBox.AppendText("\n\r");
                     }
                 }
                 catch (Exception ex)
                 {

                 }
             });

         });

            this
                .Log()
                .Info($"{nameof(LogView)} Initialized.");

            static string ConvertToString(object message) =>
                message is string ? message.ToString() : Newtonsoft.Json.JsonConvert.SerializeObject(message);

            static IEnumerable<string> Selector((LogLevel level, object message) next) =>
                  (new string[] { "[" + next.level.ToString() + "]" })
                  .Concat(ConvertToString(next.message)
                        //.Replace("{", "")
                        //.Replace("}", "")
                        .Split(new[] { '\n', '\r' })
                        .Where(c => string.IsNullOrEmpty(c) == false));


            ScrollCommand = scrollCommand;
            ClearAllCommand = clearAllCommand;
        }


        public ICommand ScrollCommand
        {
            get { return (ICommand)GetValue(ScrollCommandProperty); }
            set { SetValue(ScrollCommandProperty, value); }
        }

        public ICommand ClearAllCommand
        {
            get { return (ICommand)GetValue(ClearAllCommandProperty); }
            set { SetValue(ClearAllCommandProperty, value); }
        }
    }

    static class Helper
    {

        // James World
        //http://www.zerobugbuild.com/?p=323
        ///The events should be output at a maximum rate specified by a TimeSpan, but otherwise as soon as possible.
        public static IObservable<T> Pace<T>(this IObservable<T> source, TimeSpan rate)
        {
            var paced = source.Select(i => Observable.Empty<T>()

                                      .Delay(rate)
                                      .StartWith(i)).Concat();

            return paced;
        }

        public static IObservable<(T, bool)> ScanChanges<T, TR>(this IObservable<T> source, Func<T, TR> property) where TR : IEquatable<TR>
        {
            return source
          .Scan((default(T), false), (a, b) => (b, property(a.Item1).Equals(property(b)) == false))
          .Skip(1);
        }
    }
}
