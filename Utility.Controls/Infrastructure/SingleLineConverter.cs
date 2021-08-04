using System;
using System.Windows;
using System.Windows.Data;

namespace Utility.Controls.Infrastructure
{
    public class SingleLineConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string str)
            {
                if (str.Split('\r') is string[] split)
                    if (split.Length > 1)
                        return split[0] + " ...";
                    else
                        return split[0];
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter Members
    }
}