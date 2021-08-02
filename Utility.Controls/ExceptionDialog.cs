using System;

namespace Utility.Controls {
    public class ExceptionDialog : ConfirmationDialog {
        public ExceptionDialog(Exception exception, string message = null) : this()
        {
            Content = new ObjectControl(message, exception);
        }

        public ExceptionDialog() : base()
        {
            //MaxHeight = 400;
        }
    }
}