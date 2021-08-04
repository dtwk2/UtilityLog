using Autofac;

namespace Utility.ViewModel.Meta {
   public class BootStrapper {

      public BootStrapper(ContainerBuilder builder)
      {
         builder.RegisterType<ExportViewModel>().AsSelf().AsImplementedInterfaces().SingleInstance();
      }
   }
}
