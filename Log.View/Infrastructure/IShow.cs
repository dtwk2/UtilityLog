using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityLog.View.Infrastructure
{
    public interface IShowExceptionDialog
    {
        System.Threading.Tasks.Task<bool> ShowExceptionDialog(Exception exception);
    }
}
