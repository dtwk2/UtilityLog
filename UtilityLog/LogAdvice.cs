using System;
using System.Linq;
using ArxOne.MrAdvice.Advice;
using Newtonsoft.Json;
using Splat;

namespace UtilityLog
{
   public class LogAdvice : Attribute, IMethodAdvice, IEnableLogger
   {
      public void Advise(MethodAdviceContext context)
      {
         // do things you want here
         this.Log().Info(
               "Method Name : " + context.TargetName + Environment.NewLine +
                    "Target Type : " + context.TargetType.FullName + Environment.NewLine +
                     "Arguments : " + string.Join(", ", context.Arguments.Select(JsonConvert.SerializeObject)));

         context.Proceed(); // this calls the original method
                            // do other things here
         if (context.HasReturnValue)
            this.Log().Info("Method Name : " + context.TargetName + Environment.NewLine +
                            "Return Value : " + context.ReturnValue);
      }
   }
}
