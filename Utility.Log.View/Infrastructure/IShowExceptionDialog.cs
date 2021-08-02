using System;

namespace Pcs.Hfrr.Log.View.Infrastructure
{
    public interface IShowExceptionDialog
    {
        System.Threading.Tasks.Task<bool> ShowExceptionDialog(Exception exception);
    }
}