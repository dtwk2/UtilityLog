using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UtilityLog.View
{
    public class ConfirmationHost: ContentControl
    {
        static ConfirmationHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConfirmationHost), new FrameworkPropertyMetadata(typeof(ConfirmationHost)));
        }

        public ConfirmationHost(object obj=null)
        {
            Content = obj;
        }
    }
}
