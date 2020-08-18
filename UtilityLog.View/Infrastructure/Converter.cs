using LambdaConverters;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace UtilityLog.View.Infrastructure
{
    internal static class Converter
    {
        public static readonly IValueConverter VisibilityToBoolean =
       ValueConverter.Create<Visibility,bool>(e => e.Value== Visibility.Visible);

        public static readonly IValueConverter VisibleIfTrue =
            ValueConverter.Create<bool, Visibility>(e => e.Value ? Visibility.Visible : Visibility.Collapsed);

        public static readonly IValueConverter VisibleIfNotNull =
            ValueConverter.Create<object, Visibility>(e => e.Value != null ? Visibility.Visible : Visibility.Collapsed);

        public static readonly IValueConverter ToUpperCase =
            ValueConverter.Create<string, string>(e => e.Value.ToUpper());

        public static readonly IValueConverter ModConverter =
    ValueConverter.Create<object, bool>(e => e.Value is Log log && (byte)log.RunCount % 2 == 1);

        public static readonly IValueConverter LogLevelConverter = ValueConverter.Create<Splat.LogLevel, Color>(e =>
        e.Value switch
        {
            Splat.LogLevel.Debug => Colors.Orange,
            Splat.LogLevel.Error => Colors.Red,
            Splat.LogLevel.Fatal => Colors.DarkRed,
            Splat.LogLevel.Info => Colors.Blue,
            Splat.LogLevel.Warn => Colors.Green,
            _ => throw new ArgumentOutOfRangeException(e.Value.ToString())

        });


    }
}
