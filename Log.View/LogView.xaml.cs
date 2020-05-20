using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Splat;
using UtilityLog;

namespace UtilityLog.View
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl, IEnableLogger
    {
        static LogView()
        {
        }

        public LogView()
        {
            InitializeComponent();

            var dis = ObservableLogger
                .Instance
                .Messages
                .Scan(new StringBuilder(), (sb, next) => sb.Append("[").Append(next.Item1.ToString()).Append("] ").AppendLine(next.message.ToString()))
                //.CombineLatest(ControlChanges.Select(a => a as TextBox).Where(a => a != null), (a, b) => (a, b))
                .Subscribe(c =>
                {
                    this.Dispatcher.InvokeAsync(() =>
                    {
                        try
                        {
                            logOutputTextBox.Text = c.ToString();
                        }
                        catch (Exception ex)
                        {

                        }
                    });
                });

            this
                .Log()
                .Info($"{nameof(LogView)} Initialized.");
        }
    }
}
