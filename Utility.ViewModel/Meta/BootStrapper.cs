using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Pcs.Hfrr.Log.ViewModel.Meta {
   public class BootStrapper {

      public BootStrapper(ContainerBuilder builder)
      {
         builder.RegisterType<ExportViewModel>().AsSelf().AsImplementedInterfaces().SingleInstance();
      }
   }
}
