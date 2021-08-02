namespace Pcs.Hfrr.Log.View.Infrastructure
{
    using System.Windows.Input;

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