using System;
using System.Net;
using System.Net.Cache;
using System.Threading.Tasks;
using System.Threading;

namespace Redmine.Net.Api
{
	public class RedmineWebClientAsync
	{
		public string BaseAddress { get; set; }
		public RequestCachePolicy CachePolicy { get; set; }
		public bool UseDefaultCredentials { get; set; }
		public ICredentials Credentials { get; set; }
		public WebHeaderCollection Headers { get; set; }
		public IWebProxy Proxy { get; set; }

//		public RedmineAsyncWebClient()
//		{
//			//var defaultClient = new WebClient();
//			//BaseAddress = defaultClient.BaseAddress;
//			//Headers = defaultClient.Headers;
//			//Proxy = defaultClient.Proxy;
//		}
//
//		public RedmineAsyncWebClient(string apiKey)
//		{
//			ApiKey = apiKey;
//		}

//		public Task<string> DownloadStringAsync(string uri, CancellationToken cancelToken = default (CancellationToken))
//		{
//			return  Task.Factory.StartNew(() =>  GetWebClient(cancelToken).DownloadStringAsync(new Uri(uri)));
//		}
//
//		public Task<byte[]> DownloadDataAsync(string uri, CancellationToken cancelToken = default (CancellationToken))
//		{
//			return  Task.Factory.StartNew(() => GetWebClient(cancelToken).DownloadDataAsync(new Uri(uri)));
//		}
//
//		public Task DownloadFileAsync(string uri, string fileName, CancellationToken cancelToken = default (CancellationToken))
//		{
//			Task.Factory.StartNew(() => GetWebClient(cancelToken).DownloadFileAsync(new Uri(uri), fileName));
//		}
//
//		private WebClient GetWebClient(CancellationToken cancelToken)
//		{
//			var wc = new WebClient
//			{
//				BaseAddress = BaseAddress,
//				CachePolicy = CachePolicy,
//				UseDefaultCredentials = UseDefaultCredentials,
//				Credentials = Credentials,
//				Headers = Headers,
//				Proxy = Proxy
//			};
//
//			if (!string.IsNullOrEmpty(ApiKey)) wc.QueryString["key"] = ApiKey;
//
//			if (cancelToken != CancellationToken.None) cancelToken.Register(() => wc.CancelAsync());
//			//if (progress != null) wc.DownloadProgressChanged += (sender, args) => progress.Report(args);
//
//			return wc;
//		}
	}
}

