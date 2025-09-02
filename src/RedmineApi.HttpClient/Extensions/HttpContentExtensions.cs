using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Padi.RedmineApi.HttpClient.Extensions;

internal static class HttpContentExtensions
{
#if NET40_OR_GREATER
    public static Task<string> ReadAsStringAsync(this HttpContent content, CancellationToken cancellationToken = default)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (!cancellationToken.CanBeCanceled)
        {
            return content.ReadAsStringAsync();
        }

        if (cancellationToken.IsCancellationRequested)
        {
            var tcsCanceled = new TaskCompletionSource<string>();
            tcsCanceled.SetCanceled();
            return tcsCanceled.Task;
        }

        var tcs = new TaskCompletionSource<string>();
        var reg = cancellationToken.Register(() => tcs.TrySetCanceled());

        content
            .ReadAsStringAsync()
            .ContinueWith(t =>
        {
            reg.Dispose();
            if (t.IsCanceled) 
            {
                tcs.TrySetCanceled();
            }
            else
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception.Flatten().InnerExceptions);
                }
                else
                {
                    tcs.TrySetResult(t.Result);
                }
            }
        }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        return tcs.Task;
    }

    public static Task<Stream> ReadAsStreamAsync(this HttpContent content,  CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
#endif
}