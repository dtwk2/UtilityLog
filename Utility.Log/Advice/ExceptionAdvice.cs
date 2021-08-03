//using ArxOne.MrAdvice.Advice;
//using Splat;
//using System;

//namespace Pcs.Hfrr.Log.Advice
//{
//    /// <summary>
//    /// Aspect that, when applied on a method, catches all its exceptions, and logs details,
//    /// </summary>
//#warning "Need to include MrAdvice library in main project for 'Advise' method invocation."

//    public class ExceptionAdvice : Attribute, IMethodAdvice, IEnableLogger
//    {
//        /// <summary>
//        /// Method invoked upon failure of the method to which the current
//        /// aspect is applied.
//        /// </summary>
//        /// <param name="args">Information about the method being executed.</param>
//        public void Advise(MethodAdviceContext context)
//        {
//            try
//            {
//                context.Proceed();
//            }
//            catch (Exception e)
//            {
//                this.Log().Error(e, $"Error from {nameof(ExceptionAdvice)}");
//            }
//        }
//    }
//}