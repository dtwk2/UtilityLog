using System;
using ArxOne.MrAdvice.Advice;
using Splat;

namespace UtilityLog
{
   // <summary>
   // Aspect that, when applied on a method, catches all its exceptions,
   // </summary>
   public class ExceptionAdvice : Attribute, IMethodAdvice, IEnableLogger
    {
        /// <summary>
        /// Method invoked upon failure of the method to which the current
        /// aspect is applied.
        /// </summary>
        /// <param name="args">Information about the method being executed.</param>
        public void Advise(MethodAdviceContext context)
        {
            try
            {
                context.Proceed();
            }
            catch (Exception e)
            {
                // do things you want here
                this.Log().Info(
                         "Message : " + e.Message + Environment.NewLine +
                              " StackTrace : " + e.StackTrace);

                Environment.Exit(1);
            }

        }
    }
}