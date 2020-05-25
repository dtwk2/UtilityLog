//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Windows.Data;
//using System.Windows.Media;

//namespace UtilityLog.View.Json.ValueConverters
//{

//    class JTokenToColorConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (value is Newtonsoft.Json.Linq.JTokenType token)
//                return GetColor((byte)token);
//            return Colors.BlanchedAlmond;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }

//        public static Color GetColor(byte key) => NiceColors.Value[key];


//        private static Lazy<Dictionary<int, Color>> NiceColors = new Lazy<Dictionary<int, Color>>(() =>
//               niceColorsDict
//                .Select((a, i) => Tuple.Create(i, (Color)ColorConverter.ConvertFromString(a.Value)))
//                .ToDictionary(a => a.Item1, a => a.Item2));



//        static Dictionary<string, string> niceColorsDict = new Dictionary<string, string> {
//            { "navy", "#001F3F"} ,
//             { "blue", "#0074D9"} ,
//              { "aqua", "#7FDBFF"} ,
//                                  { "teal", "#39CCCC"} ,
//                                  { "olive", "#3D9970"} ,
//                                  { "green", "#2ECC40"} ,
//                                  { "d", "#d59aea"} ,
//                                  {  "yellow", "#FFDC00"} ,
//                         { "black", "#111111"},
//                                  { "red", "#FF4136"} ,
//               { "fuchsia", "#F012BE"} ,

//             { "purple", "#B10DC9"} ,

//             { "maroon", "#85144B"} ,
//                                  { "gray", "#AAAAAA"} ,
//                                                 { "silver", "#DDDDDD"} ,
//                                           {  "orange", "#FF851B"} ,
//                                  { "a", "#ff035c"},
//                                  { "b", "#9eb4cc"},
//                                  { "c", "#fbead3"},
//                                  };
//    }

//}

