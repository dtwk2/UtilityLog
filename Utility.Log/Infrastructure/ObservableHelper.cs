using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Utility.Log.Infrastructure
{
    public static class ObservableHelper
    {
        public static IObservable<T> SelectProgress<T>(this Progress<T> progress)
        {
            var progressChanges =
                Observable.FromEventPattern<T>(a => progress.ProgressChanged += a,
                           a => progress.ProgressChanged -= a)
                    .Select(a => a.EventArgs);

            return progressChanges;
        }


        public static IObservable<PropertyChangedEventArgs> ObservePropertyChanges(this INotifyPropertyChanged source) {
           return Observable
              .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                 h => source.PropertyChanged += h,
                 h => source.PropertyChanged -= h)
              .Select(a => a.EventArgs);
        }
    }
}