using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Controls;
using Splat;

namespace UtilityLog.View
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl, IEnableLogger
    {
        static LogView()
        {
        }

        public LogView()
        {
            InitializeComponent();

                   var lines = ObservableLogger
                .Instance
                .Messages
                .SelectMany(Selector)
                .Pace(TimeSpan.FromSeconds(0.5))
                .Subscribe(c =>
                {
                    this.Dispatcher.InvokeAsync(() =>
                    {
                        try
                        {
                            logOutputTextBox.AppendText(c);
                            //if (c.ToLower().Contains("[error]"))
                            //    logOutputTextBox.BorderBrush = Brushes.Red;
                            logOutputTextBox.ScrollToEnd();
                            if (c.ToLower().Contains("[") == false && c.ToLower().Contains("]") == false)
                                logOutputTextBox.AppendText("\n\r");
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
    }
}
