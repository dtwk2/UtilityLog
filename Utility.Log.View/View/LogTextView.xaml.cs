using Pcs.Hfrr.Log.View.Infrastructure;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pcs.Hfrr.Log.View {
   /// <summary>
   /// Interaction logic for LogView.xaml
   /// </summary>
   public partial class LogView : UserControl, IEnableLogger {
      public static readonly DependencyProperty ScrollCommandProperty = DependencyProperty.Register("ScrollCommand", typeof(ICommand), typeof(LogView), new PropertyMetadata(null));

      public static readonly DependencyProperty ClearAllCommandProperty = DependencyProperty.Register("ClearAllCommand", typeof(ICommand), typeof(LogView), new PropertyMetadata(null));

      static LogView() {
      }

      public LogView() {
         InitializeComponent();
         var scrollCommand = ReactiveUI.ReactiveCommand.Create<bool?, bool>(a => !(a ?? false));
         var clearAllCommand = ReactiveUI.ReactiveCommand.Create<Unit, Guid>(a => Guid.NewGuid());

         var ac =
             Locator.Current.GetServices<IObservableLogger>().ToObservable()
                 .SelectMany(obs => obs
                     .Messages
                     .SelectMany(Selector));

         _ = ac
             .Pace(TimeSpan.FromSeconds(0.5))
             .WithLatestFrom(scrollCommand.StartWith((PlayPause1.IsChecked == false)), (a, b) => (a, b))
             .CombineLatest(clearAllCommand.StartWith(default(Guid)), (a, c) => (a.a, a.b, c))
             .ScanChanges(a => a.c)
             .Subscribe(d => {
                var ((newContent, scroll, _), clear) = d;
                this.Dispatcher.InvokeAsync(() => {
                   try {
                      if (clear)
                         logOutputTextBox.Clear();
                      else {
                         logOutputTextBox.AppendText(newContent);
                         if (scroll)
                            logOutputTextBox.ScrollToEnd();
                         if (newContent.ToLower().Contains("(") == false &&
                                newContent.ToLower().Contains(")") == false &&
                                newContent.ToLower().Contains("[") == false &&
                                newContent.ToLower().Contains("]") == false)
                            logOutputTextBox.AppendText("\n\r");
                      }
                   }
                   catch (Exception ex) {
                   }
                });
             });

         this
             .Log()
             .Info($"{nameof(LogView)} Initialized.");

         static string ConvertToString(object message) =>
             message is string ? message.ToString() : Newtonsoft.Json.JsonConvert.SerializeObject(message);

         static IEnumerable<string> Selector((LogLevel level, object message, DateTime date) next) =>
                new[] {
                         "("+ next.date.ToString("") +")",
                         "[" + next.level + "]"
                   }
                   .Concat(Second(next.message));

         static IEnumerable<string> Second(object message) =>
            ConvertToString(message)
               .Split(new[] { '\n', '\r' })
               .Where(c => string.IsNullOrEmpty(c) == false);

         ScrollCommand = scrollCommand;
         ClearAllCommand = clearAllCommand;
      }

      public ICommand ScrollCommand {
         get => (ICommand)GetValue(ScrollCommandProperty);
         set => SetValue(ScrollCommandProperty, value);
      }

      public ICommand ClearAllCommand {
         get => (ICommand)GetValue(ClearAllCommandProperty);
         set => SetValue(ClearAllCommandProperty, value);
      }
   }
}