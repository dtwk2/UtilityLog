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
    public partial class ViewModel : IEnableLogger
    {
        static readonly Random random = new Random();
        public readonly ReactiveCommand<object, object> command;
        public readonly ReactiveCommand<object, object> command2;

        public ViewModel()
        {
            command = ReactiveCommand.Create<object, object>(a => a);
            _ = command.Subscribe(ThrowException);

            command2 = ReactiveCommand.Create<object, object>(a => a);
            _ = command2.Subscribe(ThrowUnhandledException);


            _ = Observable.Interval(TimeSpan.FromSeconds(10)).StartWith(0).Select(ThrowExceptionOnEvenNumber)
                .LoggedCatch(this, Observable.Return(0L)).Subscribe();

            _ = Observable.Interval(TimeSpan.FromSeconds(20)).StartWith(0)
            .Subscribe(a => RandomMethod());

            _ = Observable.Interval(TimeSpan.FromSeconds(3)).StartWith(0)
                .Subscribe(a => RandomNullMethod());
        }




        [LogAdvice]
        static double RandomMethod()
        {
            return random.NextDouble();
        }


        [NullOutputAdvice]
        static double? RandomNullMethod()
        {
            var d = random.NextDouble();
            return d > 0.5 ? (double?)null : d;
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
