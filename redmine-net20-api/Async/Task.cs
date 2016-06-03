using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Redmine.Net.Api.Async
{

    public delegate void Task();

    public delegate TResult Task<out TResult>();

    public static class TaskHelper
    {
        public static Task<TResult> Task<TResult>(Task<TResult> task)
        {
            var result = default(TResult);
            var completed = false;

            var sync = new object();
            task.BeginInvoke(iac =>
            {
                lock (sync)
                {
                    completed = true;
                    result = task.EndInvoke(iac);
                    Monitor.Pulse(sync);
                }
            }, null);

            return delegate
            {
                lock (sync)
                {
                    if (!completed)
                    {
                        Monitor.Wait(sync);
                    }
                    return result;
                }
            };
        }
    }
}
