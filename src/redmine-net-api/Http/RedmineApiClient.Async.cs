#if !NET20
using System;
using System.Threading;
using System.Threading.Tasks;
using Redmine.Net.Api.Http.Constants;
using Redmine.Net.Api.Http.Messages;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Net.Internal;

namespace Redmine.Net.Api.Http;

internal abstract partial class RedmineApiClient
{
    public async Task<RedmineApiResponse> GetAsync(string address, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.GET, requestOptions, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<RedmineApiResponse> GetPagedAsync(string address, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync(address, requestOptions, cancellationToken).ConfigureAwait(false);
    }

    public async Task<RedmineApiResponse> CreateAsync(string address, string payload,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.POST, requestOptions, CreateContentFromPayload(payload),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<RedmineApiResponse> UpdateAsync(string address, string payload,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.PUT, requestOptions, CreateContentFromPayload(payload),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<RedmineApiResponse> UploadFileAsync(string address, byte[] data,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.POST, requestOptions, CreateContentFromBytes(data),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<RedmineApiResponse> PatchAsync(string address, string payload,
        RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.PATCH, requestOptions, CreateContentFromPayload(payload),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<RedmineApiResponse> DeleteAsync(string address, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.DELETE, requestOptions, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<RedmineApiResponse> DownloadAsync(string address, RequestOptions requestOptions = null,
        IProgress<int> progress = null, CancellationToken cancellationToken = default)
    {
        return await HandleRequestAsync(address, HttpConstants.HttpVerbs.DOWNLOAD, requestOptions, progress: progress,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    protected abstract Task<RedmineApiResponse> HandleRequestAsync(
        string address,
        string verb,
        RequestOptions requestOptions = null,
        object content = null,
        IProgress<int> progress = null,
        CancellationToken cancellationToken = default);
}
#endif