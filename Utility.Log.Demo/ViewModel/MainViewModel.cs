//using AspectInjector.Broker;

//using Pcs.Hfrr.Log.Advice;
using ReactiveUI;
using Splat;
using System;
using System.Reactive.Linq;

namespace Pcs.Hfrr.Log.Demo {
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainViewModel : IEnableLogger {
      private static readonly Random random = new Random();
      public readonly ReactiveCommand<object, object> command;
      public readonly ReactiveCommand<object, object> command2;

      public MainViewModel() {
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

      //[LogAdvice]
      private static double RandomMethod() {
         return random.NextDouble();
      }

      //[NullOutputAdvice]
      private static double? RandomNullMethod() {
         var d = random.NextDouble();
         return d > 0.5 ? (double?)null : d;
      }

      //[ExceptionAdvice]
      private void ThrowException(object l) {

         try {
            string message = "Exception thrown delibrately";
            throw new Exception(message);
         }
         catch (Exception e) {
            this.Log().Error(new Exception("Outer exception", e), $"Error from {nameof(MainViewModel)}");
         }
         // var result = await DialogHost.Show(new ConfirmationHost(message));
      }

      private static void ThrowUnhandledException(object l) {
         throw new Exception("no exception - this exception won't be caught straight-away");
      }

      private static long ThrowExceptionOnEvenNumber(long l) {
         if ((l % 2) == 1)
            throw new Exception("no exception");
         return 0;
      }

      //[LogCall]
      //public static void TestAspectInjectorLibrary(int x) {
      //   //Bar(x * 2);
      //}
   }

   //// AspectInjector
   //[Aspect(Scope.Global)]
   //[Injection(typeof(LogCall))]
   //public class LogCall : Attribute {
   //   [AspectInjector.Broker.Advice(Kind.Before)] // you can have also After (async-aware), and Around(Wrap/Instead) kinds
   //   public void LogEnter([Argument(Source.Name)] string name) {
   //      Console.WriteLine($"Calling '{name}' method...");   //you can debug it
   //   }
   //}
}