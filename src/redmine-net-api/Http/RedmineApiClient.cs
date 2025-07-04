using System;
using System.Net;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http.Constants;
using Redmine.Net.Api.Http.Extensions;
using Redmine.Net.Api.Http.Messages;
using Redmine.Net.Api.Logging;
using Redmine.Net.Api.Options;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Http;

internal abstract partial class RedmineApiClient : IRedmineApiClient
{
    protected readonly IRedmineAuthentication Credentials;
    protected readonly IRedmineSerializer Serializer;
    protected readonly RedmineManagerOptions Options;

    protected RedmineApiClient(RedmineManagerOptions redmineManagerOptions)
    {
        Credentials = redmineManagerOptions.Authentication;
        Serializer = redmineManagerOptions.Serializer;
        Options = redmineManagerOptions;
    }

    public RedmineApiResponse Get(string address, RequestOptions requestOptions = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.GET, requestOptions);
    }

    public RedmineApiResponse GetPaged(string address, RequestOptions requestOptions = null)
    {
        return Get(address, requestOptions);
    }

    public RedmineApiResponse Create(string address, string payload, RequestOptions requestOptions = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.POST, requestOptions, CreateContentFromPayload(payload));
    }

    public RedmineApiResponse Update(string address, string payload, RequestOptions requestOptions = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.PUT, requestOptions, CreateContentFromPayload(payload));
    }

    public RedmineApiResponse Patch(string address, string payload, RequestOptions requestOptions = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.PATCH, requestOptions, CreateContentFromPayload(payload));
    }

    public RedmineApiResponse Delete(string address, RequestOptions requestOptions = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.DELETE, requestOptions);
    }

    public RedmineApiResponse Download(string address, RequestOptions requestOptions = null,
        IProgress<int> progress = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.DOWNLOAD, requestOptions, progress: progress);
    }

    public RedmineApiResponse Upload(string address, byte[] data, RequestOptions requestOptions = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.POST, requestOptions, CreateContentFromBytes(data));
    }

    protected abstract RedmineApiResponse HandleRequest(
        string address,
        string verb,
        RequestOptions requestOptions = null,
        object content = null,
        IProgress<int> progress = null);
    
    protected abstract object CreateContentFromPayload(string payload);
    
    protected abstract object CreateContentFromBytes(byte[] data);

    protected static bool IsGetOrDownload(string method)
    {
        return method is HttpConstants.HttpVerbs.GET or HttpConstants.HttpVerbs.DOWNLOAD;
    }
    
    protected void LogRequest(string verb, string address, RequestOptions requestOptions)
    {
        if (Options.LoggingOptions?.IncludeHttpDetails != true)
        {
            return;
        }
        
        Options.Logger.Info($"Request HTTP {verb} {address}");
            
        if (requestOptions?.QueryString != null)
        {
            Options.Logger.Info($"Query parameters: {requestOptions.QueryString.ToQueryString()}");
        }
    }
    
    protected void LogResponse(int statusCode)
    {
        if (Options.LoggingOptions?.IncludeHttpDetails == true)
        {
            Options.Logger.Info($"Response status: {statusCode.ToInvariantString()}");
        }
    }
}