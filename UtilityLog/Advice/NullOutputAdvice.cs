using System;
using System.ComponentModel;
using ArxOne.MrAdvice.Advice;
using Newtonsoft.Json.Linq;
using Splat;

namespace UtilityLog
{
    /// Aspect that, when applied on a method, catches all its null return values, and logs details,
    /// </summary>
#warning "Need to include MrAdvice library in main project for 'Advise' method invocation."
    public class NullOutputAdvice : Attribute, IMethodAdvice, IEnableLogger
    {
        public void Advise(MethodAdviceContext context)
        {
            context.Proceed(); // this calls the original method
                               // do other things here
            if (context.HasReturnValue && context.ReturnValue == null)
            {
                var enter = JObject.FromObject(new
                {
                    MethodName = context.TargetName,
                    Information = "Method returned null",
                    TargetType = context.TargetType.FullName,
                    Arguments = new JArray(context.Arguments)
                });

                this.Log().Info(enter.ToString());
            }
        }
    }
}
