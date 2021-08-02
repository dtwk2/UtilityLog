using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ReactiveUI;
//using Telerik.Windows.Controls;
//using Telerik.Windows.Controls.GridView;
//using Telerik.Windows.Data;

namespace Pcs.Hfrr.Log.View.Infrastructure {
    internal static class ObservableHelper {
        // James World
        //http://www.zerobugbuild.com/?p=323
        ///The events should be output at a maximum rate specified by a TimeSpan, but otherwise as soon as possible.
        public static IObservable<T> Pace<T>(this IObservable<T> source, TimeSpan rate) {
            var paced = source.Select(i => Observable.Empty<T>()

                .Delay(rate)
                .StartWith(i)).Concat();

            return paced;
        }

        ///// James World
        /////http://www.zerobugbuild.com/?p=323/
        /////The events should be output at a maximum rate specified by a TimeSpan, but otherwise as soon as possible.
        //public static IObservable<T> Pace<T>(this IObservable<T> source, IObservable<TimeSpan> rate) {
        //   var paced =
        //      rate.Select(rt =>
        //      source.Select(i => Observable.Empty<T>()
        //         .Delay(rt)
        //      .StartWith(i)).Concat()).Switch();

        //   return paced;
        //}
        /// James World
        ///http://www.zerobugbuild.com/?p=323/
        ///The events should be output at a maximum rate specified by a TimeSpan, but otherwise as soon as possible.
        public static IObservable<T> Pace<T>(this IObservable<T> source, IObservable<TimeSpan> rate) {
            return source
               .WithLatestFrom(rate, (a, b) => (a, b))
               .Select(a => {
                   return Observable.Empty<T>()

                   .Delay(a.b)
                   .StartWith(a.a);
               }).Concat();
        }


        public static IObservable<(T, bool)> ScanChanges<T, TR>(this IObservable<T> source, Func<T, TR> property) where TR : IEquatable<TR> {
            return source
                .Scan((default(T), false), (a, b) => (b, property(a.Item1).Equals(property(b)) == false))
                .Skip(1);
        }



        public static IObservable<double> ObserveSliderValueChanges(this Slider source) {
            return Observable
               .FromEventPattern<RoutedPropertyChangedEventHandler<double>,
                  RoutedPropertyChangedEventArgs<double>>(
                  h => source.ValueChanged += h,
                  h => source.ValueChanged -= h)
               .Select(a => a.EventArgs.NewValue)
               .Buffer(TimeSpan.FromSeconds(1))
               .Select(a => a.LastOrDefault())
               .Where(a => a != default)
               .StartWith(source.Value);
        }

        public static IObservable<bool?> SelectCheckedChanges(this ToggleButton source, bool filter = false, bool initialValue = false) {
            var xx = Observable
               .FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                  h => source.Checked += h,
                  h => source.Checked -= h)
               .Select(a => source.IsChecked)
               .Where(a => filter);

            return initialValue ? xx.StartWith(source.IsChecked) : xx;
        }

        //a.GroupRowIsExpandedChanged += (s, e) => A_GroupRowIsExpandedChanged(a, e);
        //a.Items.GroupCollectionChanged += (s, e) => Items_GroupCollectionChanged(a, e);
    }
}