using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

    protected FakeHttpWebResponse(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public static FakeHttpWebResponse Create(HttpStatusCode statusCode, string content)
    {
        var response = (FakeHttpWebResponse)FormatterServices.GetUninitializedObject(typeof(FakeHttpWebResponse));
        response._statusCode = statusCode;
        response._content = Encoding.UTF8.GetBytes(content ?? string.Empty);
        return response;
    }

    public override HttpStatusCode StatusCode => _statusCode;
    public override Stream GetResponseStream() => new MemoryStream(_content);
}
