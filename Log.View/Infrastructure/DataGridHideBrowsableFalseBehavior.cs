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
    public class DataGridCustomDataTemplateBehavior : Behavior<DataGrid>
    {

        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn += AssociatedObject_AutoGeneratingColumn;
            base.OnAttached();
        }

        private void AssociatedObject_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyDescriptor is PropertyDescriptor propertyDescriptor)
            {
                if (propertyDescriptor.IsBrowsable == false)
                    e.Cancel = true;

            }
        }
    }
}

