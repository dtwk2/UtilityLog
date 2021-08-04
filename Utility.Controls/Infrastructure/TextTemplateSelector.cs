using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Utility.Controls.Infrastructure
{
    public class TextTemplateSelector : DataTemplateSelector
    {
        private PropertyInfo property;
        public string Property { get; set; }

        public DataTemplate TextBoxTemplate { get; set; }

        public double Width { get; set; }

        public DataTemplate ExpanderTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            property ??= item?.GetType().GetProperty(Property);

            // your logic do determine what template you need goes here
            if (item is Log.Model.Log log && property?.GetValue(log) is string str)
            {
                if (str.Split('\r') is string[] split)
                    if (split.Length > 1 || split.Length > 0 && ContentsBiggerThanTextBox(new TextBlock(),split[0], (int)Width))
                        return ExpanderTemplate;
                    else
                        return TextBoxTemplate;
            }

            return default(DataTemplate);
        }

        public Size MeasureString(string s)
        {

            if (string.IsNullOrEmpty(s))
            {
                return new Size(0, 0);
            }

            var TextBlock = new TextBlock()
            {
                Text = s
            };

            return new Size(TextBlock.DesiredSize.Width, TextBlock.DesiredSize.Height);

        }

        public bool ContentsBiggerThanTextBox(TextBlock textBlock, string text, int width)
        {
            Typeface typeface = new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch);
            FormattedText ft = new FormattedText(text, System.Globalization.CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight, typeface, textBlock.FontSize, Brushes.Black);
            if (ft.Width > width)
                return true;

            return false;
        }
    }
}