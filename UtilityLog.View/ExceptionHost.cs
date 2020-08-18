using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UtilityLog.View
{
    public class ExceptionHost : ConfirmationHost
    {
        public ExceptionHost(Exception exception, string message = null)
        {
            this.Content = new ObjectView(message, exception);
        }
    }
}
