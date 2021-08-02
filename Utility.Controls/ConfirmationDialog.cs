using System.Windows;
using System.Windows.Controls;

namespace Utility.Controls
{
    public class ConfirmationDialog : ContentControl {

        static ConfirmationDialog() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConfirmationDialog), new FrameworkPropertyMetadata(typeof(ConfirmationDialog)));
        }

        public ConfirmationDialog() {
        }
    }
}