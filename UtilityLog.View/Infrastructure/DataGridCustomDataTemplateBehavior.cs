using Microsoft.Xaml.Behaviors;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;


namespace UtilityLog.View.Infrastructure
{
    public class DataGridCustomDataTemplateBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty DataTemplateProperty =
            DependencyProperty.Register("DataTemplate", typeof(DataTemplate), typeof(DataGridCustomDataTemplateBehavior), new PropertyMetadata(null));

        public DataTemplate DataTemplate
        {
            get { return (DataTemplate)GetValue(DataTemplateProperty); }
            set { SetValue(DataTemplateProperty, value); }
        }


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
                   
                    var column = new DataGridTemplateColumn()
                    {
                        CellTemplate = DataTemplate,
                    };
                    e.Column = column;
                }
                // e.Column.Header = descriptor.DisplayName ?? descriptor.Name;
            }
        }
    }

}

