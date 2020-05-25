using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityLog.View.Infrastructure
{

    using System.Windows.Input;


    public static class RoutedCommands
    {

        public static readonly RoutedUICommand ShowToolsPanel = new RoutedUICommand
        (
            "Show Log",
            nameof(ShowToolsPanel),
            typeof(RoutedCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Alt)
            }
        );
    }
}


