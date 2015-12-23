using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Redmine.Net.Api
{
    public class RedmineWebClientAsync
    {
        public Uri BaseAddress { get; set; }
     
        public bool UseDefaultCredentials { get; set; }
        public ICredentials Credentials { get; set; }
        public NameValueCollection Headers { get; set; }
        public IWebProxy Proxy { get; set; }
		public bool UseProxy { get; set; }
		public bool UseCookies{ get; set;}

        public TimeSpan Timeout { get; set; }
        public CookieContainer CookieContainer { get; set; }
        public bool PreAuthenticate { get; set; }
        public ClientCertificateOption ClientCertificateOptions { get; set; }

        private string ApiKey { get; set; }

		        public RedmineWebClientAsync()
        {
           
        }

		        public RedmineWebClientAsync(string apiKey)
        {
            ApiKey = apiKey;
        }
//
		public async Task<HttpResponseMessage> DownloadStringAsync(string uri, CancellationToken cancelToken = default (CancellationToken), IProgress<DownloadProgressChangedEventArgs> progress = null)
        {
			return await Task.Run(() => GetWebClient(cancelToken, progress).GetAsync(uri)).ConfigureAwait(false);
        }

        public async Task<byte[]> DownloadDataAsync(string uri, CancellationToken cancelToken = default (CancellationToken), IProgress<DownloadProgressChangedEventArgs> progress = null)
        {
			return await Task.Run(() => GetWebClient(cancelToken, progress).GetByteArrayAsync(uri)).ConfigureAwait(false);
        }

//        public async Task DownloadFileAsync(string uri, string fileName, CancellationToken cancelToken = default (CancellationToken), IProgress<DownloadProgressChangedEventArgs> progress = null)
//        {
//			await Task.Run(() => GetWebClient(cancelToken, progress).GetStreamAsync(uri, fileName)).ConfigureAwait(false);
//        }

		public async Task<HttpResponseMessage> Delete(string uri, CancellationToken cancelToken = default (CancellationToken)){
			return await Task.Run (() => GetWebClient (cancelToken, null).DeleteAsync(uri)).ConfigureAwait(false);
		}

		public async Task<HttpResponseMessage> Create(string uri, CancellationToken cancelToken = default (CancellationToken)){
			return await Task.Run (() => GetWebClient (cancelToken, null).PostAsync(uri,null)).ConfigureAwait(false);
		}

		private HttpClient GetWebClient(CancellationToken cancelToken, IProgress<DownloadProgressChangedEventArgs> progress)
        {
			var hch = new HttpClientHandler
                     {
                         UseDefaultCredentials = UseDefaultCredentials,
                         Credentials = Credentials,
                         Proxy = Proxy,
                         UseCookies = UseCookies,
                         UseProxy = UseProxy,
                         CookieContainer = CookieContainer,
						AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None,
                         ClientCertificateOptions  = ClientCertificateOptions,
                         PreAuthenticate = PreAuthenticate
                     };

            var hc = new HttpClient(hch,false)
            {
                BaseAddress = BaseAddress,
                Timeout = Timeout
            };

		    foreach (NameValueHeaderValue header in Headers)
		    {
		        hc.DefaultRequestHeaders.Add(header.Name, header.Value);
		    }

            if (!string.IsNullOrEmpty(ApiKey)) hc.DefaultRequestHeaders.Add("key", ApiKey);

            return hc;
        }
    }
}
