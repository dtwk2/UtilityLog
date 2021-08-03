//using ArxOne.MrAdvice.Advice;
//using Newtonsoft.Json.Linq;
//using Splat;
//using System;

//namespace Pcs.Hfrr.Log.Advice
//{
//    /// <summary>
//    /// Aspect that, when applied on a method, logs details,
//    /// </summary>
//#warning "Need to include MrAdvice library in main project for 'Advise' method invocation."

//    public class LogAdvice : Attribute, IMethodAdvice, IEnableLogger
//    {
//        public void Advise(MethodAdviceContext context)
//        {
//            var enter = JObject.FromObject(new
//            {
//                MethodName = context.TargetName,
//                TargetType = context.TargetType.FullName,
//                Arguments = new JArray(context.Arguments)
//            });

//            // do things you want here
//            this.Log().Info(enter.ToString());

//            context.Proceed(); // this calls the original method
//                               // do other things here
//            if (context.HasReturnValue)
//            {
//                var retrn = JObject.FromObject(new
//                {
//                    MethodName = context.TargetName,
//                    context.ReturnValue,
//                });
//                this.Log().Info(retrn.ToString());
//            }
//        }
//    }
//}