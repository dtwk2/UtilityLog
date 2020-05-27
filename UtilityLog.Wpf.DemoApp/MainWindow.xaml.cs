using AspectInjector.Broker;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using Splat;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace UtilityLog.Wpf.DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, Splat.IEnableLogger, View.Infrastructure.IShowExceptionDialog
    {
        const string path = "../../../Data/Log.sqlite";

        public MainWindow()
        {
            InitializeComponent();

            var command = ReactiveCommand.Create<object, object>(a => a);

            (this.Resources["SendException"] as Button).Command = command;

            var command2 = ReactiveCommand.Create<object, object>(a => a);

            command2.ThrownExceptions.Subscribe(a =>
            {
                command2 = ReactiveCommand.Create<object, object>(a => a);
                command2.Subscribe(ThrowUnhandledException);
            });

            (this.Resources["SendUnhandledException"] as Button).Command = command2;

            SQLitePCL.Batteries.Init();
            var conn = UtilityDAL.Sqlite.ConnectionFactory.Create<Log>(path);

            _ = new SQLiteLogger(conn);

            (this.Resources["LogDbView1"] as View.LogDbView).Connection = conn;

            //this.Log().Info("Dfssfdd");


            //_ = Observable.Interval(TimeSpan.FromSeconds(3)).StartWith(0).Select(ThrowExceptionOnEvenNumber).LoggedCatch(this, Observable.Return(0L)).Subscribe();

            //_ = Observable.Interval(TimeSpan.FromSeconds(5)).StartWith(0).Subscribe(a => RandomMethod(GetRandomEmail(), GetRandomFileName(), GetRandomInt()));

            _ = command.Subscribe(ThrowException);

            _ = command2.Subscribe(ThrowUnhandledException);

            //_ = Observable.Empty<long>().StartWith(0).Subscribe(a=>
            //{
            //    throw new Exception("no exception");
            //});

        }


        [LogAdvice]
        static double RandomMethod(string sdf, string afd, int sd)
        {
            return GetRandomDouble();

        }

        [ExceptionAdvice]
        static async void ThrowException(object l)
        {
            string message = "Close Application (or leave in unstable state)?";

            var result = await DialogHost.Show(new UtilityLog.View.ObjectView("asfdsd", l));

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

        public async System.Threading.Tasks.Task<bool> ShowExceptionDialog(Exception exception)
        {
            string message = "Close Application (or leave in unstable state)?";
        
            var result = await DialogHost.Show(new View.ExceptionView(exception));
            return (bool)result;
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            
        }
    }

    //
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
