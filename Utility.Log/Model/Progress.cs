
namespace Pcs.Hfrr.Log.Model {

   public readonly struct Progress {

      public Progress(string key, double value, bool isIndeterminate= false)
      {
         Key = key;
         Value = value;
         IsIndeterminate = isIndeterminate;
      }

      public string Key { get; }

      public double Value { get; }

      public bool IsIndeterminate { get; }
   }
}
