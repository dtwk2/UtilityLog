using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Pcs.Hfrr.Log.Infrastructure;
using Pcs.Hfrr.Log.Model;
using ReactiveUI;
using Utility.Infrastructure;

namespace Pcs.Hfrr.Log.ViewModel {

   public class ExportViewModel : ReactiveObject, IObservable<ExportRequest> {

      private readonly ReactiveCommand<Unit, Unit> export;
      private readonly ReactiveCommand<int, int> exportQuantity;
      private readonly ReactiveCommand<string, string> exportDirectory;
      private readonly ReplaySubject<ExportRequest> exportRequest = new ReplaySubject<ExportRequest>();

      private readonly ObservableAsPropertyHelper<double> progress;
      private readonly ObservableAsPropertyHelper<bool> isEnabled;

      private static readonly string FileName = $"LogArchive_{DateTime.Now:yyyy-MM-ddTHH-mm-ss}.zip";
      private const string FilePattern = "Log_*-*-*.sqlite";
      private const string ArchiveReportName = "ArchiveReport.txt";

      public ExportViewModel(IObservable<Progress> progressObservable) {

         export = ReactiveCommand.Create(() => { });

         exportQuantity = ReactiveCommand.Create<int, int>(a => a);

         exportDirectory = ReactiveCommand.Create<string, string>(a => a);

         progress = progressObservable.Select(a => a.Value).ToProperty(this, a => a.Progress);
         isEnabled = progressObservable.Select(a => a.Value >= 100 || a.Value <= 0).ToProperty(this, a => a.IsEnabled);

         _ = export
                  .CombineLatest(exportQuantity, exportDirectory, (a, b, c) => (b, c))
                  .Subscribe(d => {

                     var (quantity, exportDirectory) = d;

                     var source = BootStrapper.ConnectionDirectory.FullName;

                     var fileInfos = ExportHelper.SelectFileInfos(source, FilePattern, quantity);
                     exportRequest.OnNext(new ExportRequest(Guid.NewGuid().ToString().Remove(6), fileInfos,
                        exportDirectory, FileName, ArchiveReportName));

                  }, e => { });
      }

      public ICommand Export => export;

      public ICommand ExportQuantity => exportQuantity;

      public ICommand ExportDirectory => exportDirectory;

      public bool IsEnabled => isEnabled.Value;

      public double Progress => progress.Value;

      public IDisposable Subscribe(IObserver<ExportRequest> observer)
      {
         return exportRequest.Subscribe(observer);
      }
   }


}

