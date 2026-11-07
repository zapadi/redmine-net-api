using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Net.WebClient;

#pragma warning disable SYSLIB0014, SYSLIB0050, SYSLIB0051, CS0618
namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure;

internal sealed class FakeWebClient(Func<FakeWebRequest, WebResponse> responder) : WebClient
{
    public List<FakeWebRequest> Requests { get; } = [];

    protected override WebRequest GetWebRequest(Uri address)
    {
        var webRequest = base.GetWebRequest(address)!;
        var request = new FakeWebRequest(address) { Method = webRequest.Method, Headers = webRequest.Headers };
        Requests.Add(request);
        return request;
    }

    protected override WebResponse GetWebResponse(WebRequest request)
    {
        return responder((FakeWebRequest)request);
    }

    protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
    {
        return responder((FakeWebRequest)request);
    }
}

internal sealed class FakeWebRequest(Uri requestUri) : WebRequest
{
    public override Uri RequestUri { get; } = requestUri;
    public override string Method { get; set; } = "GET";
    public override WebHeaderCollection Headers { get; set; } = new();
    public override string ContentType { get; set; }
    public override long ContentLength { get; set; }
    public override ICredentials Credentials { get; set; }
    public override bool UseDefaultCredentials { get; set; }
    public override bool PreAuthenticate { get; set; }
    public override int Timeout { get; set; }
    public override IWebProxy Proxy { get; set; }
    public override string ConnectionGroupName { get; set; }

    public override Stream GetRequestStream() => new MemoryStream();

    public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state) => Complete<Stream>(new MemoryStream(), callback, state);

    public override Stream EndGetRequestStream(IAsyncResult asyncResult) => ((Task<Stream>)asyncResult).Result;

    public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state) => Complete<WebResponse>(null, callback, state);

    public override WebResponse EndGetResponse(IAsyncResult asyncResult) => ((Task<WebResponse>)asyncResult).Result;

    public override void Abort()
    {
    }

    private static Task<T> Complete<T>(T result, AsyncCallback callback, object state)
    {
        var tcs = new TaskCompletionSource<T>(state);
        tcs.SetResult(result);
        callback?.Invoke(tcs.Task);
        return tcs.Task;
    }
}

internal sealed class FakeWebResponse(string content) : WebResponse
{
    private readonly byte[] _content = Encoding.UTF8.GetBytes(content ?? string.Empty);

    public override long ContentLength { get => _content.Length; set { } }
    public override string ContentType { get; set; } = "application/xml";
    public override WebHeaderCollection Headers { get; } = new();
    public override Stream GetResponseStream() => new MemoryStream(_content);
}

internal class FakeHttpWebResponse : HttpWebResponse
{
    private HttpStatusCode _statusCode;
    private byte[] _content;
    private WebHeaderCollection _headers;

    protected FakeHttpWebResponse(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public static FakeHttpWebResponse Create(HttpStatusCode statusCode, string content, WebHeaderCollection headers = null)
    {
        var response = (FakeHttpWebResponse)FormatterServices.GetUninitializedObject(typeof(FakeHttpWebResponse));
        response._statusCode = statusCode;
        response._content = Encoding.UTF8.GetBytes(content ?? string.Empty);
        response._headers = headers ?? new WebHeaderCollection();
        return response;
    }

    public override HttpStatusCode StatusCode => _statusCode;
    public override WebHeaderCollection Headers => _headers;
    public override Stream GetResponseStream() => new MemoryStream(_content);
}

internal sealed class HangingWebClient : WebClient
{
    protected override WebRequest GetWebRequest(Uri address) => new HangingWebRequest(address);
}

internal sealed class HangingWebRequest(Uri requestUri) : WebRequest
{
    private TaskCompletionSource<WebResponse> _response;
    private AsyncCallback _callback;

    public override Uri RequestUri { get; } = requestUri;
    public override string Method { get; set; } = "GET";
    public override WebHeaderCollection Headers { get; set; } = new();
    public override string ContentType { get; set; }
    public override long ContentLength { get; set; }
    public override ICredentials Credentials { get; set; }
    public override bool UseDefaultCredentials { get; set; }
    public override bool PreAuthenticate { get; set; }
    public override int Timeout { get; set; }
    public override IWebProxy Proxy { get; set; }
    public override string ConnectionGroupName { get; set; }

    public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
    {
        _callback = callback;
        _response = new TaskCompletionSource<WebResponse>(state);
        return _response.Task;
    }

    public override WebResponse EndGetResponse(IAsyncResult asyncResult) => ((Task<WebResponse>)asyncResult).GetAwaiter().GetResult();

    public override void Abort()
    {
        if (_response != null && _response.TrySetException(new WebException("Aborted", WebExceptionStatus.RequestCanceled)))
        {
            _callback?.Invoke(_response.Task);
        }
    }
}

internal sealed class CustomWebClientOptions : IRedmineWebClientOptions
{
    public bool? AutoRedirect { get; set; }
    public CookieContainer CookieContainer { get; set; }
    public DecompressionMethods? DecompressionFormat { get; set; }
    public ICredentials Credentials { get; set; }
    public Dictionary<string, string> DefaultHeaders { get; set; }
    public IWebProxy Proxy { get; set; }
    public bool? KeepAlive { get; set; }
    public int? MaxAutomaticRedirections { get; set; }
    public long? MaxRequestContentBufferSize { get; set; }
    public long? MaxResponseContentBufferSize { get; set; }
    public int? MaxConnectionsPerServer { get; set; }
    public int? MaxResponseHeadersLength { get; set; }
    public bool? PreAuthenticate { get; set; }
    public RequestCachePolicy RequestCachePolicy { get; set; }
    public string Scheme { get; set; }
    public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }
    public TimeSpan? Timeout { get; set; }
    public bool? UnsafeAuthenticatedConnectionSharing { get; set; }
    public string UserAgent { get; set; }
    public bool? UseCookies { get; set; }
    public bool? UseDefaultCredentials { get; set; }
    public bool? UseProxy { get; set; }
    public Version ProtocolVersion { get; set; }
    public bool CheckCertificateRevocationList { get; set; }
    public int? DefaultConnectionLimit { get; set; }
    public int? DnsRefreshTimeout { get; set; }
    public bool? EnableDnsRoundRobin { get; set; }
    public int? MaxServicePoints { get; set; }
    public int? MaxServicePointIdleTime { get; set; }
    public SecurityProtocolType? SecurityProtocolType { get; set; }
    public X509CertificateCollection ClientCertificates { get; set; }
    public bool? ReusePort { get; set; }
}
