using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors.Core;

namespace TestApp {
   public class TimedProgressBar : ProgressBar {

      private Storyboard storyBoard;
      private Grid grid;

      public static readonly DependencyProperty DurationProperty =
          DependencyProperty.Register("Duration", typeof(Duration), typeof(TimedProgressBar), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(1)), Changed));

      public static readonly DependencyProperty SecondsProperty =
          DependencyProperty.Register("Seconds", typeof(int), typeof(TimedProgressBar), new PropertyMetadata(1, Changed));

      public static readonly DependencyProperty FlashProperty =
         DependencyProperty.Register("Flash", typeof(ICommand), typeof(TimedProgressBar), new PropertyMetadata(default));

      public static readonly DependencyProperty RepeatProperty =
         DependencyProperty.Register("Repeat", typeof(bool), typeof(TimedProgressBar), new PropertyMetadata(true, RepeatChanged));


      static TimedProgressBar() {
        // DefaultStyleKeyProperty.OverrideMetadata(typeof(TimedProgressBar), new FrameworkPropertyMetadata(typeof(TimedProgressBar)));
      }

      public bool Repeat {
         get { return (bool)GetValue(RepeatProperty); }
         set { SetValue(RepeatProperty, value); }
      }

      public ICommand Flash {
         get { return (ICommand)GetValue(FlashProperty); }
         set { SetValue(FlashProperty, value); }
      }

      public int Seconds {
         get { return (int)((Duration)GetValue(DurationProperty)).TimeSpan.TotalSeconds; }
         set { SetValue(DurationProperty, new Duration(TimeSpan.FromSeconds(value))); }
      }

      public Duration Duration {
         get { return (Duration)GetValue(DurationProperty); }
         set { SetValue(DurationProperty, value); }
      }


      private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {
         Duration newDuration, oldDuration;
         if (d is TimedProgressBar flashView)
            if (!(e.NewValue is int newValue))
               if ((e.NewValue is Duration durationValue))
               {
                  newDuration = durationValue;
                  oldDuration = (Duration)e.OldValue;
               }
               else
                  return;
            else
            {
               newDuration = new Duration(TimeSpan.FromSeconds(newValue));
               oldDuration = new Duration(TimeSpan.FromSeconds((int) e.OldValue));
            }
         else
            return;

         flashView.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
            //var clone = flashView.storyBoard.Clone();
            //flashView.storyBoard.Stop();
            //clone.Duration = duration;
            //foreach (var child in clone.Children) {

            //   child.Duration = flashView.Duration;
            //}
            //clone.Begin(flashView.grid);

            var anim = new DoubleAnimation(0, 100, newDuration.TimeSpan){RepeatBehavior = RepeatBehavior.Forever};
            flashView.BeginAnimation(ProgressBar.ValueProperty, anim, HandoffBehavior.Compose);
         }));
      }
      private static void RepeatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {

         //if (!(d is TimedProgressBar flashView && e.NewValue is bool newValue))
         //   return;

         //flashView.storyBoard.Stop();
         //flashView.storyBoard.RepeatBehavior = newValue ? RepeatBehavior.Forever : new RepeatBehavior(1);

         //var clone = flashView.storyBoard.Clone();
         //flashView.storyBoard.Stop();
         //clone.RepeatBehavior = newValue ? RepeatBehavior.Forever : new RepeatBehavior(1);
         //clone.Begin(flashView.grid);
      }

      public TimedProgressBar() {

         Flash = new ActionCommand(a => {
            Repeat = false;

         });


         this.Loaded += FlashView_Loaded;
      }

  
      public override void OnApplyTemplate() {
         //grid = this.GetTemplateChild("thumbGrid") as Grid;
         //; this.storyBoard = this.Template.Resources["MainStoryboard"] as Storyboard;
         //this.ellipse = this.GetTemplateChild("focusedHalo") as Ellipse;
         //Storyboard.SetTargetName(storyBoard, "focusedHalo");
         base.OnApplyTemplate();
      }

      private void FlashView_Loaded(object sender, RoutedEventArgs e) {

      }


   }
}
