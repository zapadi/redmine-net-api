#if NET40_OR_GREATER || NET
using System;
using System.Threading;
using System.Threading.Tasks;
using Padi.RedmineApi.Internals;

namespace Padi.RedmineApi.Net;

/// <inheritdoc />
public abstract partial class RedmineApiClient
{
    /// <inheritdoc />
    public async Task<ApiResponseMessage> GetAsync(string address, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.GET, requestOptions, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
    /// <inheritdoc />
    public async Task<ApiResponseMessage> GetPagedAsync(string address, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync(address, requestOptions, cancellationToken).ConfigureAwait(false);
    }
    /// <inheritdoc />
    public async Task<ApiResponseMessage> CreateAsync(string address, string payload,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.POST, requestOptions, CreateContentFromPayload(payload, _serializer),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    /// <inheritdoc />
    public async Task<ApiResponseMessage> UpdateAsync(string address, string payload,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.PUT, requestOptions, CreateContentFromPayload(payload, _serializer),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    /// <inheritdoc />
    public async Task<ApiResponseMessage> UploadFileAsync(string address, byte[] data,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.POST, requestOptions, CreateContentFromBytes(data),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    /// <inheritdoc />
    public async Task<ApiResponseMessage> PatchAsync(string address, string payload,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.PATCH, requestOptions, CreateContentFromPayload(payload, _serializer),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    /// <inheritdoc />
    public async Task<ApiResponseMessage> DeleteAsync(string address, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.DELETE, requestOptions, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
    /// <inheritdoc />
    public async Task<ApiResponseMessage> DownloadAsync(string address, RequestOptions requestOptions = null,
        IProgress<int> progress = null, CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.DOWNLOAD, requestOptions, progress: progress,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <param name="verb"></param>
    /// <param name="requestOptions"></param>
    /// <param name="content"></param>
    /// <param name="progress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected abstract Task<ApiResponseMessage> HandleRequestAsync(
        string address,
        string verb,
        RequestOptions requestOptions = null,
        object content = null,
        IProgress<int> progress = null,
        CancellationToken cancellationToken = default);
}
#endif