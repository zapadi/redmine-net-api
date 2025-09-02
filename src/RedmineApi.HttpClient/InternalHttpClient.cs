using System;
using Padi.RedmineApi.Authentication;
using Padi.RedmineApi.Net;

namespace Padi.RedmineApi.HttpClient;

/// <summary>
/// 
/// </summary>
internal sealed partial class InternalHttpClient : RedmineApiClient,IDisposable
{
    private static System.Net.Http.HttpClient? _httpClient;
    
    private readonly IRedmineAuthentication _credentials;
    private readonly bool _disposeClient;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="auth"></param>
    /// <param name="options"></param>
    /// <param name="serializerType"></param>
    public InternalHttpClient(IRedmineAuthentication auth, HttpClientApiOptions options, string serializerType) 
        : base(serializerType)
    {
        _credentials = auth ?? throw new ArgumentNullException(nameof(auth));
        _httpClient = HttpClientProvider.GetOrCreateHttpClient(options);
        _disposeClient = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="auth"></param>
    /// <param name="options"></param>
    /// <param name="serializerType"></param>
    /// <exception cref="ArgumentException"></exception>
    public InternalHttpClient( System.Net.Http.HttpClient httpClient, IRedmineAuthentication auth, HttpClientApiOptions options, string serializerType)
        : base(serializerType)
    {
        _credentials = auth ?? throw new ArgumentNullException(nameof(auth));
        _httpClient = HttpClientProvider.GetOrCreateHttpClient(options, httpClient);
        _disposeClient = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        if (_disposeClient)
        {
            _httpClient?.Dispose();
        }
    }

    protected override ApiResponseMessage HandleRequest(string address, string verb, RequestOptions? requestOptions = null, object? content = null, IProgress<int>? progress = null)
    {
        var requestMessage = CreateRequestMessage(address, verb, requestOptions, content as ApiRequestContent);

        return Send(requestMessage, progress);
    }

    private ApiResponseMessage Send(ApiRequestMessage requestMessage, IProgress<int>? progress = null)
    {
        //TODO: improve it
        return SendAsync(requestMessage, progress).Result;
    }

    protected override object CreateContentFromPayload(string payload, string contentType)
    {
        return ApiRequestContent.CreateString(payload, contentType);
    }

    protected override object CreateContentFromBytes(byte[] data)
    {
        return ApiRequestContent.CreateBinary(data);
    }

    private static ApiRequestMessage CreateRequestMessage(string address, string verb, RequestOptions? requestOptions = null, ApiRequestContent? content = null)
    {
        var req = new ApiRequestMessage
        {
            RequestUri = address,
            Method = verb,
        };

        if (requestOptions != null)
        {
            req.QueryString = requestOptions.QueryString;
            req.ImpersonateUser = requestOptions.ImpersonateUser;
        }

        if (content != null)
        {
            req.Content = content;
        }

        return req;
    }
}