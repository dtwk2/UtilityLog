//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Windows.Data;
//using Newtonsoft.Json.Linq;

//namespace UtilityLog.View.Json.ValueConverters
//{
//    // This converter is only used by JProperty tokens whose Value is Array/Object
//    class ComplexPropertyMethodToValueConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (!(parameter is string methodName &&
//                value?.GetType().GetMethod(methodName, new Type[0]) is System.Reflection.MethodInfo methodInfo))
//                return null;

//            return ((IEnumerable<JToken>)methodInfo.Invoke(value, new object[0])).First().Children();
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
//        }
//    }
//}
