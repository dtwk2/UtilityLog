//using System;
//using System.Globalization;
//using System.Linq;
//using System.Windows.Data;
//using Newtonsoft.Json.Linq;

//namespace UtilityLog.View.Json.ValueConverters
//{
//    public sealed class JArrayLengthConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (!(value is JToken jToken))
//                throw new Exception("Wrong type for this converter");

//            return jToken.Type switch
//            {
//                JTokenType.Array => $"[{jToken.Children().Count()}]",
//                JTokenType.Property => $"[ { jToken.Children().FirstOrDefault().Children().Count()} ]",
//                _ => throw new Exception("Type should be JProperty or JArray"),
//            };
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
//        }
//    }
//}
