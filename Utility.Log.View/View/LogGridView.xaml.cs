﻿using DynamicData;
using Pcs.Hfrr.Log.View.Infrastructure;
using Splat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace Pcs.Hfrr.Log.View {
   /// <summary>
   /// Interaction logic for LogGridUserControl.xaml
   /// </summary>
   public partial class LogGridView : UserControl {
      public LogGridView() {
         InitializeComponent();
         this.Loaded += LogGridUserControl_Loaded;


      }

      private void LogGridUserControl_Loaded(object sender, System.Windows.RoutedEventArgs e) {

         int runCount = 0;
         DateTime date = DateTime.Now;

         var logs = SelectGuidAndLogs()
            .ToObservableChangeSet(a => a.guid)
             .Transform(a => new LogGroup(a.guid, a.logs));

         _ = logs.Filter(a => a.Date < date)
            .Merge(logs.Filter(a => a.Date > date).Pace(MainSlider
           .ObserveSliderValueChanges()
             .Select(TimeSpan.FromSeconds)))
            .ObserveOnDispatcher()
           .Bind(out var collection)
           .Subscribe(a => { },
             ex => {

             });

         MainLogGrid.ItemsSource = collection;



         IObservable<(Guid guid, Log[] logs)> SelectGuidAndLogs() {

            return Observable.Create<(Guid guid, Log[] logs)>(observer => {
               return Locator.Current.GetServices<IObservableLogger>().ToObservable()
                  .Distinct(a => a.GetType().Name)
                  .Subscribe(obs => {
                     obs
                        .Messages
                        //.Pace(TimeSpan.FromSeconds(1))
                        .ObserveOnDispatcher()
                        .Select(c => {
                           var guid = Guid.NewGuid();
                           var (level, message,date) = c;
                           var logs = SelectLogs(level, message, guid, date).ToArray();
                           return (guid, logs);
                        }).Subscribe(observer.OnNext);
                  });
            });

            IEnumerable<Log> SelectLogs(LogLevel level, object message, Guid guid, DateTime date) {
               switch (message) {
                  case string msg:
                     yield return new Log(msg, level, runCount) { Key = guid, Date = date };
                     break;

                  case Exception exception: {
                        while (exception != null) {
                           yield return new Log(exception, level, runCount) { Key = guid, Date = date };
                           exception = exception.InnerException;
                        }

                        break;
                     }
               }
            }
            //return Locator.Current.GetServices<IObservableLogger>().ToObservable()
            //   .Distinct(a => a.GetType().Name)
            //   .SelectMany(obs =>


            //   .Timestamp()
            //   .Do(a => Debug.WriteLine(DateTime.Now))
            //   .Select(a => a.Value);
         }
      }
   }
}