using System;
using System.Collections.Specialized;
using System.Threading;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Async
{
    public delegate TRes Task<out TRes>();

    public static class RedmineManagerAsync
    {
        public static Task<TRes> Task<TRes>(Task<TRes> task)
        {
            TRes result = default(TRes);
            bool completed = false;

            object sync = new object();
            IAsyncResult asyncResult = task.BeginInvoke(iac =>
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

        public static Task<User> GetCurrentUserAsync(this RedmineManager redmineManager, NameValueCollection parameters = null)
        {
            Task<User> task = delegate
            {
                using (var wc = redmineManager.CreateWebClient(parameters))
                {
                    var uri = UrlHelper.GetCurrentUserUrl(redmineManager);
                    var response = wc.DownloadString(new Uri(uri));
                    return RedmineSerializer.Deserialize<User>(response,redmineManager.MimeFormat);
                }
            };
            return task;
        }
    }
}