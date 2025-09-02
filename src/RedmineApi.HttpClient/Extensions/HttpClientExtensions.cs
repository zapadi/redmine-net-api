using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Padi.RedmineApi.HttpClient.Extensions;

/// <summary>
/// 
/// </summary>
internal static class HttpClientExtensions
{
    /// <summary>
    /// Downloads a <see cref="Stream"/> from the given URL, and reports the download progress using the input callback
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> instance to use to download the data</param>
    /// <param name="url">The URL to download</param>
    /// <param name="callback">The optional progress callback</param>
    /// <param name="token">The optional token for the download operation</param>
    public static async Task<Stream?> DownloadAsync(
        this System.Net.Http.HttpClient client,
        string url,
        IProgress<HttpProgress>? callback,
        CancellationToken token = default)
    {
        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);
        if (!response.IsSuccessStatusCode || token.IsCancellationRequested)
        {
            return null;
        }

        using var source = await response.Content.ReadAsStreamAsync();
        Stream result = new MemoryStream();
        long
            totalRead = 0L,
            totalReads = 0L,
            length = response.Content.Headers.ContentLength ?? 0;

        var buffer = new byte[8192];
        var isMoreToRead = true;

        do
        {
            var read = await source.ReadAsync(buffer, 0, buffer.Length, token);
            if (read == 0)
            {
                isMoreToRead = false;
            }
            else
            {
                await result.WriteAsync(buffer, 0, read, token);
                totalRead += read;
                if (totalReads++ % 2000 == 0)
                {
                    callback?.Report(new HttpProgress(totalRead, length > 0 ? (int)(totalRead * 100 / length) : 0));
                }
            }
        } while (isMoreToRead && !token.IsCancellationRequested);

        if (token.IsCancellationRequested)
        {
            result.Dispose();
            return null;
        }

        result.Seek(0, SeekOrigin.Begin);
        return result;
    }
}