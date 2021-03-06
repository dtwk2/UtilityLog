using System;
using System.Reactive.Disposables;
using System.Windows.Threading;

namespace UtilityLog.View.Infrastructure
{
    using Splat;

    public class UIFreezeObserver : IEnableLogger
    {
        public IDisposable Observe()
        {
            var timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = Constants.UiFreezeTimer
            };

            var previous = DateTime.Now;

            timer.Tick += (sender, args) =>
            {
                var current = DateTime.Now;
                var delta = current - previous;
                previous = current;

                if (delta > Constants.UiFreeze)
                {
                    this.Log().Warn(new UIFreeze(delta));
                }
            };

            timer.Start();
            return Disposable.Create(timer.Stop);
        }
    }

    class UIFreeze
    {
        public UIFreeze(TimeSpan duration)
        {
            Duration = duration;
        }

        public TimeSpan Duration { get; set; }
    }
}
