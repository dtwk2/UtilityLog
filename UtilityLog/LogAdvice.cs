using System;
using System.ComponentModel;
using ArxOne.MrAdvice.Advice;
using Newtonsoft.Json.Linq;
using Splat;

namespace UtilityLog
{
    [Description("Need to inclide MrAdvice in main project for this to work.")]
    public class LogAdvice : Attribute, IMethodAdvice, IEnableLogger
    {
        public void Advise(MethodAdviceContext context)
        {
            var enter = JObject.FromObject(new
            {
                MethodName = context.TargetName,
                TargetType = context.TargetType.FullName,
                Arguments = new JArray(context.Arguments)
            });

            // do things you want here
            this.Log().Info(enter.ToString());


            context.Proceed(); // this calls the original method
                               // do other things here
            if (context.HasReturnValue)
            {
                var retrn = JObject.FromObject(new
                {
                    MethodName = context.TargetName,
                    context.ReturnValue,

                });
                this.Log().Info(retrn.ToString());
            }

        }
    }
}
