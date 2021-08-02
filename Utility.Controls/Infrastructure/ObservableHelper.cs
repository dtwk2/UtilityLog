using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Controls;

namespace Utility.View.Infrastructure {
    public static class ObservableHelper {
        public static IObservable<FileInfo> FileChanges(this DirectoryComboBox directoryComboBox) => Observable
            .FromEventPattern<DirectoryComboBox.FileChangeEventHandler, DirectoryComboBox.FileChangeEventArgs>(
                a => directoryComboBox.FileChange += a, 
                a => directoryComboBox.FileChange -= a)
            .Select(a => a.EventArgs.FileInfo);
    }
}
