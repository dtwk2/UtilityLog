using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Utility.Log.Infrastructure;

namespace Utility.ViewModel.Infrastructure {
   public class ExportHelper {


      public static FileInfo[] SelectFileInfos(string sourceDirectory, string filePattern, int takeLast) {
         var files = Directory.GetFiles(sourceDirectory, filePattern)
            .Select(a => new FileInfo(a))
            .OrderByDescending(a => a.CreationTime)
            .Take(takeLast)
            .ToArray();
         return files;
      }


      public static IObservable<Progress> Export(string key, FileInfo[] sourceFiles,
         string destinationDirectory,
         string destinationFileName,
         string destinationReportName) {

         var progress = new Progress<double>();
         string exportFile = Path.Combine(destinationDirectory, destinationFileName);
         var progressChanges = progress.SelectProgress().Select(a => new Progress(key, a));

         string reportContents = null;

         _ = Task.Run(() => {
            var array = ZipHelper.ArchiveToFile(sourceFiles, exportFile, progress).ToArray();
            return array;
         })
             .ToObservable()
             .Subscribe(a => {
                reportContents = CreateReportContents(a, exportFile);
                File.WriteAllText(Path.Combine(destinationDirectory, destinationReportName), reportContents);
             }, exception => {
                reportContents = CreateReportExceptionContents(exception, exportFile);
                File.WriteAllText(Path.Combine(destinationDirectory, destinationReportName), reportContents);
             });

         return progressChanges;
      }

      private static string CreateReportContents((bool sucess, FileInfo fileInfo, Exception exception)[] output, string exportFile) {
         var stringBuilder = new StringBuilder();
         stringBuilder.AppendLine($"Archive Report for {exportFile}");

         stringBuilder.AppendLine("success, file, exception");
         foreach (var (success, fileInfo, exception) in output) {
            stringBuilder.AppendLine($"{success}, {fileInfo.FullName}, {(success ? "Null" : $"{exception.Message}")}");
         }

         return stringBuilder.ToString();
      }

      private static string CreateReportExceptionContents(Exception exception, string exportFile) {
         var stringBuilder = new StringBuilder();
         stringBuilder.AppendLine($"Archive Report for {exportFile}");

         stringBuilder.AppendLine(exception.Message);
         stringBuilder.AppendLine();
         stringBuilder.AppendLine(exception.StackTrace);

         return stringBuilder.ToString();
      }
   }
}
