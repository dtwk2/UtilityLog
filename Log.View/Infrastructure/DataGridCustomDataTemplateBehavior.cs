using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace UtilityLog.View.Infrastructure
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
                    resourceDictionary ??= (ResourceDictionary)Application.LoadComponent(new Uri("/UtilityLog.View;component/Themes/Generic.xaml", UriKind.Relative));
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


        public static void _grid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (!(sender is DataGrid grid)) { return; }

            //AutoGenerateColumnCollection coll = GetColumns(grid);

            //foreach (AutoGenerateColumn col in coll)
            //{
            //    if (e.PropertyName == col.Column)
            //    {
            //        CustomDataGridTemplateColumn templateColumn =
            //                new CustomDataGridTemplateColumn();
            //        templateColumn.Header = e.Column.Header;
            //        if (col.CellTemplate != null)
            //        {
            //            templateColumn.CellTemplate = col.CellTemplate;
            //        }
            //        if (col.CellEditingTemplate != null)
            //        {
            //            templateColumn.CellEditingTemplate = col.CellEditingTemplate;
            //        }
            //        if (col.Binding != null)
            //        {
            //            templateColumn.Binding = col.Binding;
            //        }

            //        templateColumn.SortMemberPath = e.Column.SortMemberPath;
            //        e.Column = templateColumn;
            //        return;
            //    }
            //}

            return;
        }
    }

    public class AutoGenerateColumn
    {
        public string Column
        {
            get; set;
        }

        public DataTemplate CellTemplate
        {
            get; set;
        }

        public DataTemplate CellEditingTemplate
        {
            get; set;
        }

        public System.Windows.Data.BindingBase Binding
        {
            get; set;
        }
    }
}

