using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Padi.DotNet.RedmineAPI.Tests.Infrastructure;

internal sealed class RecordedRequest(string method, string rawUrl, NameValueCollection headers, byte[] body)
{
    public string Method { get; } = method;
    public string RawUrl { get; } = rawUrl;
    public NameValueCollection Headers { get; } = headers;
    public byte[] Body { get; } = body;
}

internal sealed class TestHttpResponse
{
    public int StatusCode { get; init; } = 200;
    public string ContentType { get; init; }
    public byte[] Body { get; init; } = [];
    public Dictionary<string, string> Headers { get; init; } = new();
    public TimeSpan Delay { get; init; }
    public long? ContentLength { get; init; }
}

internal sealed class TestHttpServer : IDisposable
{
    private static readonly TimeSpan MaxHold = TimeSpan.FromSeconds(30);

    private readonly HttpListener _listener;
    private readonly Func<RecordedRequest, TestHttpResponse> _handler;
    private readonly ManualResetEvent _stopped = new(false);
    private readonly List<RecordedRequest> _requests = [];

    public TestHttpServer(Func<RecordedRequest, TestHttpResponse> handler)
    {
        _handler = handler;
        for (var attempt = 1; ; attempt++)
        {
            Url = $"http://localhost:{GetFreePort()}/";
            _listener = new HttpListener();
            _listener.Prefixes.Add(Url);
            try
            {
                _listener.Start();
                break;
            }
            catch (HttpListenerException) when (attempt < 5)
            {
                _listener.Close();
            }
        }

        _ = Task.Run(ListenAsync);
    }

    public string Url { get; }

    public IReadOnlyList<RecordedRequest> Requests
    {
        get
        {
            lock (_requests)
            {
                return _requests.ToArray();
            }
        }
    }

    public static int GetFreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private async Task ListenAsync()
    {
        while (_listener.IsListening)
        {
            HttpListenerContext context;
            try
            {
                context = await _listener.GetContextAsync().ConfigureAwait(false);
            }
            catch
            {
                return;
            }

            _ = Task.Run(() => Handle(context));
        }
    }

    private void Handle(HttpListenerContext context)
    {
        try
        {
            using var body = new MemoryStream();
            context.Request.InputStream.CopyTo(body);
            var request = new RecordedRequest(context.Request.HttpMethod, context.Request.RawUrl, new NameValueCollection(context.Request.Headers), body.ToArray());
            lock (_requests)
            {
                _requests.Add(request);
            }

            var response = _handler(request);
            if (response.Delay > TimeSpan.Zero && _stopped.WaitOne(response.Delay))
            {
                context.Response.Abort();
                return;
            }

            context.Response.StatusCode = response.StatusCode;
            if (response.ContentType != null)
            {
                context.Response.ContentType = response.ContentType;
            }

            foreach (var header in response.Headers)
            {
                context.Response.Headers[header.Key] = header.Value;
            }

            context.Response.ContentLength64 = response.ContentLength ?? response.Body.Length;
            context.Response.OutputStream.Write(response.Body, 0, response.Body.Length);
            if (response.ContentLength.HasValue)
            {
                context.Response.OutputStream.Flush();
                _stopped.WaitOne(MaxHold);
                context.Response.Abort();
                return;
            }

            context.Response.Close();
        }
        catch
        {
            try
            {
                context.Response.Abort();
            }
            catch
            {
            }
        }
    }

    public void Dispose()
    {
        _stopped.Set();
        try
        {
            _listener.Stop();
            _listener.Close();
        }
        catch
        {
        }
    }
}
