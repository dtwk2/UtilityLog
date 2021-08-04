using System;

namespace Utility.Log.View.Infrastructure
{
    public interface IShowExceptionDialog
    {
        System.Threading.Tasks.Task<bool> ShowExceptionDialog(Exception exception);
    }
}