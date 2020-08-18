using Jot;
using Jot.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UtilityLog.View.Infrastructure
{

    // Expose services as static class to keep the example simple 
    static class Services
    {
        // expose the tracker instance
        public static Tracker Tracker = new Tracker(new JsonFileStore("../../../Data"));

        static Services()
        {
            var mainWindow = Application.Current.MainWindow;

            // tell Jot how to track Window objects
            Tracker.Configure<LogHost>()
                .Id(w => w.Name)
                .Properties(w => new { w.LogVisibility })
                .PersistOn(nameof(Window.Closed), mainWindow);

            }
    }

}
