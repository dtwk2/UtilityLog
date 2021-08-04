using System.Windows.Input;

namespace Utility.Log.View.Infrastructure
{
   public static class RoutedCommands
    {
        public static readonly RoutedUICommand ShowLogPanel = new RoutedUICommand
        (
            "Show Log",
            nameof(ShowLogPanel),
            typeof(RoutedCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Alt)
            }
        );
    }
}