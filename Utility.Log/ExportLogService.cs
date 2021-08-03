using Pcs.Hfrr.Log.Common;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Pcs.Hfrr.Log.Infrastructure;
using Pcs.Hfrr.Log.Model;

namespace Pcs.Hfrr.Log {

   public class ExportService :  IObserver<ExportRequest> {
      private readonly ReplaySubject<Progress> progressSubject = new ReplaySubject<Progress>();
      private readonly ReplaySubject<ExportRequest> exportRequestSubject = new ReplaySubject<ExportRequest>();


      public ExportService()
      {
         exportRequestSubject
            .SelectMany(a => ExportHelper.Export(a.Key, a.SourceFiles, a.DestinationDirectory, a.DestinationFileName, a.DestinationReportName))
            .Subscribe(progressSubject);
      }

      public IDisposable Subscribe(IObserver<Progress> observer) {
         return progressSubject.Subscribe(observer);
      }

      public void OnCompleted()
      {
         throw new NotImplementedException();
      }

      public void OnError(Exception error)
      {
         throw new NotImplementedException();
      }

      public IObservable<ExportRequest> ExportRequest
      {
         get => exportRequestSubject;
         set => value.Subscribe(exportRequestSubject);
      }

      public void OnNext(ExportRequest value)
      {
         exportRequestSubject.OnNext(value);
      }
   }

   public class ExportRequest {

      public ExportRequest(string key, FileInfo[] sourceFiles, string destinationDirectory, string destinationFileName, string destinationReportName)
      {
         Key = key;
         SourceFiles = sourceFiles;
         DestinationDirectory = destinationDirectory;
         DestinationFileName = destinationFileName;
         DestinationReportName = destinationReportName;
      }

      public string Key { get; }

      public FileInfo[] SourceFiles { get; }

      public string DestinationDirectory { get; }

      public string DestinationFileName { get; }

      public string DestinationReportName { get; } = "ArchiveReport.txt";
   }
}