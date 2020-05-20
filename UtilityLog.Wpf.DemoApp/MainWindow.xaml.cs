using AspectInjector.Broker;
using ReactiveUI;
using Splat;
using System;
using System.Reactive.Linq;
using System.Windows;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace UtilityLog.Wpf.DemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, Splat.IEnableLogger
    {
        const string path = "../../../Data/Log.sqlite";

        public MainWindow()
        {
            InitializeComponent();
            SQLitePCL.Batteries.Init();
            var conn = UtilityDAL.Sqlite.ConnectionFactory.Create<Log>(path);

            _ = new SQLiteLogger(conn);

            LogDbView1.Connection = conn;

            //this.Log().Info("Dfssfdd");


            //_ = Observable.Interval(TimeSpan.FromSeconds(3)).StartWith(0).Select(ThrowExceptionOnEvenNumber).LoggedCatch(this, Observable.Return(0L)).Subscribe();

            //_ = Observable.Interval(TimeSpan.FromSeconds(5)).StartWith(0).Subscribe(a => RandomMethod(GetRandomEmail(), GetRandomFileName(), GetRandomInt()));

            _ = Observable.Interval(TimeSpan.FromSeconds(7)).StartWith(0).Subscribe(ThrowException);

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
        static void ThrowException(long l)
        {
            throw new Exception("no exception - Method has LogAdviceAttribute");
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
