using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Pcs.Hfrr.Log.View.Infrastructure
{
    public class DataGridHideBrowsableFalseBehavior : Behavior<DataGrid>
    {
        private ResourceDictionary resourceDictionary;

        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn += AssociatedObject_AutoGeneratingColumn;
            base.OnAttached();
        }

        private void AssociatedObject_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyDescriptor is PropertyDescriptor propertyDescriptor)
            {
                if (propertyDescriptor.PropertyType.IsEnum)
                {
                    resourceDictionary ??= (ResourceDictionary)Application.LoadComponent(new Uri("/Utility.Log.View;component/Themes/Generic.xaml", UriKind.Relative));
                    var template = resourceDictionary.Values.OfType<DataTemplate>().Single(a => a.DataType.Equals(typeof(Splat.LogLevel)));
                    var column = new DataGridTemplateColumn()
                    {
                        CellTemplate = template,
                    };
                    e.Column = column;
                }
                // e.Column.Header = descriptor.DisplayName ?? descriptor.Name;
            }
        }
    }
}