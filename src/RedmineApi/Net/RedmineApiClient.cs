using System;
using Padi.RedmineApi.Internals;

namespace Padi.RedmineApi.Net;

public abstract partial class RedmineApiClient : IRedmineApiClient
{
    private readonly string _serializer;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serializerType"></param>
    protected RedmineApiClient(string serializerType)
    {
        _serializer = serializerType;
    }
    /// <inheritdoc />
    public ApiResponseMessage Get(string address, RequestOptions requestOptions = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.GET, requestOptions);
    }
    /// <inheritdoc />
    public ApiResponseMessage GetPaged(string address, RequestOptions requestOptions = null)
    {
        return Get(address, requestOptions);
    }
    /// <inheritdoc />
    public ApiResponseMessage Create(string address, string payload, RequestOptions requestOptions = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.POST, requestOptions, CreateContentFromPayload(payload, _serializer));
    }
    /// <inheritdoc />
    public ApiResponseMessage Update(string address, string payload, RequestOptions requestOptions = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.PUT, requestOptions, CreateContentFromPayload(payload, _serializer));
    }
    /// <inheritdoc />
    public ApiResponseMessage Patch(string address, string payload, RequestOptions requestOptions = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.PATCH, requestOptions, CreateContentFromPayload(payload, _serializer));
    }
    /// <inheritdoc />
    public ApiResponseMessage Delete(string address, RequestOptions requestOptions = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.DELETE, requestOptions);
    }
    /// <inheritdoc />
    public ApiResponseMessage Download(string address, RequestOptions requestOptions = null,
        IProgress<int> progress = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.DOWNLOAD, requestOptions, progress: progress);
    }
    /// <inheritdoc />
    public ApiResponseMessage Upload(string address, byte[] data, RequestOptions requestOptions = null)
    {
        return HandleRequest(address, HttpConstants.HttpVerbs.POST, requestOptions, CreateContentFromBytes(data));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <param name="verb"></param>
    /// <param name="requestOptions"></param>
    /// <param name="content"></param>
    /// <param name="progress"></param>
    /// <returns></returns>
    protected abstract ApiResponseMessage HandleRequest(
        string address,
        string verb,
        RequestOptions requestOptions = null,
        object content = null,
        IProgress<int> progress = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    protected abstract object CreateContentFromPayload(string payload, string contentType);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    protected abstract object CreateContentFromBytes(byte[] data);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    protected static bool IsGetOrDownload(string method)
    {
        return method is HttpConstants.HttpVerbs.GET or HttpConstants.HttpVerbs.DOWNLOAD;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="total"></param>
    /// <param name="bytesRead"></param>
    protected static void ReportProgress(IProgress<int> progress, long total, long bytesRead)
    {
        if (progress == null || total <= 0)
        {
            return;
        }

        var percent = (int)(bytesRead * 100L / total);
        progress.Report(percent);
    }
}