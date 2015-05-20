using System;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;

namespace Redmine.Net.Api
{
    public class RedmineAsyncWebClient
    {
        public string BaseAddress { get; set; }
        public RequestCachePolicy CachePolicy { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public ICredentials Credentials { get; set; }
        public WebHeaderCollection Headers { get; set; }
        public IWebProxy Proxy { get; set; }

        public RedmineAsyncWebClient()
        {
            //var defaultClient = new WebClient();
            //BaseAddress = defaultClient.BaseAddress;
            //Headers = defaultClient.Headers;
            //Proxy = defaultClient.Proxy;
        }

        public async Task<string> DownloadStringAsync(string uri, CancellationToken cancelToken = default (CancellationToken), IProgress<DownloadProgressChangedEventArgs> progress = null)
        {
            return await Task.Run(() => GetWebClient(cancelToken, progress).DownloadStringTaskAsync(uri)).ConfigureAwait(false);
        }

        public async Task<byte[]> DownloadDataAsync(string uri, CancellationToken cancelToken = default (CancellationToken), IProgress<DownloadProgressChangedEventArgs> progress = null)
        {
            return await Task.Run(() => GetWebClient(cancelToken, progress).DownloadDataTaskAsync(uri)).ConfigureAwait(false);
        }

        public async Task DownloadFileAsync(string uri, string fileName, CancellationToken cancelToken = default (CancellationToken), IProgress<DownloadProgressChangedEventArgs> progress = null)
        {
            await Task.Run(() => GetWebClient(cancelToken, progress).DownloadFileTaskAsync(uri, fileName)).ConfigureAwait(false);
        }

        private WebClient GetWebClient(CancellationToken cancelToken, IProgress<DownloadProgressChangedEventArgs> progress)
        {
            var wc = new WebClient
                     {
                         BaseAddress = BaseAddress,
                         CachePolicy = CachePolicy,
                         UseDefaultCredentials = UseDefaultCredentials,
                         Credentials = Credentials,
                         Headers = Headers,
                         Proxy = Proxy
                     };

            if (cancelToken != CancellationToken.None) cancelToken.Register(() => wc.CancelAsync());
            if (progress != null) wc.DownloadProgressChanged += (sender, args) => progress.Report(args);

            return wc;
        }
    }
}
