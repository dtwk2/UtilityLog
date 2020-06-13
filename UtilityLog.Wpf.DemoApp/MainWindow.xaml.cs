using AspectInjector.Broker;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using Splat;
using System;
using System.Reactive.Linq;
using System.Windows;

namespace UtilityLog.Wpf.DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IEnableLogger
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Log().Info("MainWindow Initialized");

            var viewModel = new ViewModel();

            SendException.Command = viewModel.command;

            SendUnhandledException.Command = viewModel.command2;

        }
    }
}
