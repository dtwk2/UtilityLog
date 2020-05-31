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
        static Random random = new Random();
        public MainWindow()
        {
            InitializeComponent();

            var command = ReactiveCommand.Create<object, object>(a => a);
            _ = command.Subscribe(ThrowException);
            SendException.Command = command;

            var command2 = ReactiveCommand.Create<object, object>(a => a);
            _ = command2.Subscribe(ThrowUnhandledException);
            command2.ThrownExceptions.Subscribe(a =>
            {
                command2 = ReactiveCommand.Create<object, object>(a => a);
                command2.Subscribe(ThrowUnhandledException);
            });

            SendUnhandledException.Command = command2;

            this.Log().Info("MainWindow Initialized");



            _ = Observable.Interval(TimeSpan.FromSeconds(3)).StartWith(0).Select(ThrowExceptionOnEvenNumber)
                .LoggedCatch(this, Observable.Return(0L)).Subscribe();

            _ = Observable.Interval(TimeSpan.FromSeconds(5)).StartWith(0)
            .Subscribe(a => RandomMethod());

        }


        [LogAdvice]
        static double RandomMethod()
        {
            return random.NextDouble();
        }

        [ExceptionAdvice]
        static async void ThrowException(object l)
        {
            string message = "Exception thrown delibrately";

            var result = await DialogHost.Show(new View.ConfirmationHost(message));

        }

        static void ThrowUnhandledException(object l)
        {
            throw new Exception("no exception - this exception won't be caught straight-away");
        }


        static long ThrowExceptionOnEvenNumber(long l)
        {
            if ((l % 2) == 1)
                throw new Exception("no exception");
            return 0;
        }

        [LogCall]
        public static void TestAspectInjectorLibrary(int x)
        {
            //Bar(x * 2);
        }
    }

    // AspectInjector
    [Aspect(Scope.Global)]
    [Injection(typeof(LogCall))]
    public class LogCall : Attribute
    {
        [Advice(Kind.Before)] // you can have also After (async-aware), and Around(Wrap/Instead) kinds
        public void LogEnter([Argument(Source.Name)] string name)
        {
            Console.WriteLine($"Calling '{name}' method...");   //you can debug it	
        }
    }

}
