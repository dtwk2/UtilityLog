//using System.Windows;
//using System.Windows.Controls;
//using Newtonsoft.Json.Linq;

//namespace UtilityLog.View.Json.TemplateSelectors
//{
//    public sealed class JPropertyDataTemplateSelector : DataTemplateSelector
//    {
//        public DataTemplate PrimitivePropertyTemplate { get; set; }
//        public DataTemplate ComplexPropertyTemplate { get; set; }
//        public DataTemplate ArrayPropertyTemplate { get; set; }
//        public DataTemplate ObjectPropertyTemplate { get; set; }

//        public override DataTemplate SelectTemplate(object item, DependencyObject container)
//        {
//            if (!(item != null && container is FrameworkElement frameworkElement))
//                return null;

//            if (!(item is JProperty jProperty))
//            {
//                var key = new DataTemplateKey(item.GetType());
//                return frameworkElement.FindResource(key) as DataTemplate;
//            }


//            return jProperty.Value.Type switch
//            {
//                JTokenType.Object => frameworkElement.FindResource("ObjectPropertyTemplate"),
//                JTokenType.Array => frameworkElement.FindResource("ArrayPropertyTemplate"),
//                _ => frameworkElement.FindResource("PrimitivePropertyTemplate"),
//            } as DataTemplate;
//        }
//    }
//}
