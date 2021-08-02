using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors;

namespace TestApp
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
   }

   internal class BooleanAll2Converter : IMultiValueConverter {
      public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
      {
         if(values[1] is Storyboard storyboard && values[0] is double value) {
            storyboard.Stop();
            storyboard.Duration = new Duration(TimeSpan.FromSeconds(value));
            storyboard.Begin();
            return storyboard.Duration;
         }

         return DependencyProperty.UnsetValue;
      }

      public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
         => throw new NotImplementedException();
   }


   internal class BooleanAllConverter : IMultiValueConverter
   {
      public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
         => values.OfType<bool>().All(b => b);

      public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
         => throw new NotImplementedException();
   }

   internal class DurationConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return new Duration(TimeSpan.FromSeconds((double) value));
      }

      public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
         => throw new NotImplementedException();
   }

   internal class StoryboardConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      {
         return new Storyboard();
      }

      public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
         => throw new NotImplementedException();
   }

   class DragBlendBehavior : Microsoft.Xaml.Behaviors.Behavior<Storyboard>
   {
      protected override void OnAttached()
      {
         var aa = this.AssociatedObject;
         base.OnAttached();
      }

      protected override void OnDetaching()
      {
         base.OnDetaching();
      }
   }


   public class StoryboardHelper : DependencyObject {
      public static double GetBeginIf(DependencyObject obj) {
         return (double)obj.GetValue(BeginIfProperty);
      }

      public static void SetBeginIf(DependencyObject obj, double value) {
         obj.SetValue(BeginIfProperty, value);
      }

      public static readonly DependencyProperty BeginIfProperty = DependencyProperty.RegisterAttached("BeginIf", typeof(double), typeof(StoryboardHelper), new PropertyMetadata(0d, BeginIfPropertyChangedCallback));

      private static void BeginIfPropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e) {
         //var storyboard = s as Storyboard;
         //if (storyboard == null)
         //   throw new InvalidOperationException("This attached property only supports Storyboards.");

         //var begin = (bool)e.NewValue;
         //if (begin) storyboard.Begin();
         //else storyboard.Stop();
      }
   }
}
