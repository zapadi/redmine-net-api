#if !NET20
/*
   Copyright 2011 - 2025 Adrian Popescu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http.Constants;
using Redmine.Net.Api.Http.Helpers;
using Redmine.Net.Api.Http.Messages;

namespace Redmine.Net.Api.Http.Clients.HttpClient;

internal sealed partial class InternalRedmineApiHttpClient
{
    protected override async Task<RedmineApiResponse> HandleRequestAsync(string address, string verb, RequestOptions requestOptions = null,
        object content = null, IProgress<int> progress = null, CancellationToken cancellationToken = default)
    {
        var httpMethod = GetHttpMethod(verb);
        using var requestMessage = CreateRequestMessage(address, httpMethod, requestOptions, content as HttpContent);
        var response = await SendAsync(requestMessage, progress: progress, cancellationToken: cancellationToken).ConfigureAwait(false);
        return response;
    }

    private async Task<RedmineApiResponse> SendAsync(HttpRequestMessage requestMessage, IProgress<int> progress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            using (var httpResponseMessage = await _httpClient
                       .SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                       .ConfigureAwait(false))
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
                    {
                        return CreateApiResponseMessage(httpResponseMessage.Headers, HttpStatusCode.NoContent, []);
                    }

                    byte[] data;
                    
                    if (requestMessage.Method == HttpMethod.Get && progress != null)
                    {
                        data = await DownloadWithProgressAsync(httpResponseMessage.Content, progress, cancellationToken)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        data = await httpResponseMessage.Content.ReadAsByteArrayAsync(cancellationToken)
                            .ConfigureAwait(false);
                    }

                    return CreateApiResponseMessage(httpResponseMessage.Headers, httpResponseMessage.StatusCode, data);
                }

                var statusCode = (int)httpResponseMessage.StatusCode;
                using (var stream = await httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken)
                           .ConfigureAwait(false))
                {
                    var url = requestMessage.RequestUri?.ToString();
                    var message = httpResponseMessage.ReasonPhrase;
                    
                    throw statusCode switch
                    {
                        HttpConstants.StatusCodes.NotFound => new RedmineNotFoundException(message, url),
                        HttpConstants.StatusCodes.Unauthorized => new RedmineUnauthorizedException(message, url),
                        HttpConstants.StatusCodes.Forbidden => new RedmineForbiddenException(message, url),
                        HttpConstants.StatusCodes.UnprocessableEntity => RedmineExceptionHelper.CreateUnprocessableEntityException(url, stream, null, Serializer),
                        HttpConstants.StatusCodes.NotAcceptable => new RedmineNotAcceptableException(message),
                        _ => new RedmineApiException(message, url, statusCode),
                    };
                }
            }
        }
        catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
        {
            throw new RedmineOperationCanceledException(ex.Message, requestMessage.RequestUri, ex);
        }
        catch (OperationCanceledException ex) when (ex.InnerException is TimeoutException tex)
        {
            throw new RedmineTimeoutException(tex.Message, requestMessage.RequestUri, tex);
        }
        catch (TaskCanceledException tcex) when (cancellationToken.IsCancellationRequested)
        {
            throw new RedmineOperationCanceledException(tcex.Message, requestMessage.RequestUri, tcex);
        }
        catch (TaskCanceledException tce)
        {
            throw new RedmineTimeoutException(tce.Message, requestMessage.RequestUri, tce);
        }
        catch (HttpRequestException ex)
        {
            throw new RedmineApiException(ex.Message, requestMessage.RequestUri, HttpConstants.StatusCodes.Unknown, ex);
        }
        catch (Exception ex) when (ex is not RedmineException)
        {
            throw new RedmineApiException(ex.Message, requestMessage.RequestUri, HttpConstants.StatusCodes.Unknown, ex);
        }
    }
    
    private static async Task<byte[]> DownloadWithProgressAsync(HttpContent httpContent, IProgress<int> progress = null, CancellationToken cancellationToken = default)
    {
        var contentLength = httpContent.Headers.ContentLength ?? -1;
        byte[] data;
                    
        if (contentLength > 0)
        {
            using (var stream = await httpContent.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))
            {
                data = new byte[contentLength];
                int bytesRead;
                var totalBytesRead = 0;
                var buffer = new byte[8192];
                            
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    
                    Buffer.BlockCopy(buffer, 0, data, totalBytesRead, bytesRead);
                    totalBytesRead += bytesRead;
                                
                    var progressPercentage = (int)(totalBytesRead * 100 / contentLength);
                    progress?.Report(progressPercentage);
                    ClientHelper.ReportProgress(progress, contentLength, totalBytesRead);
                }
            }
        }
        else
        {
            data = await httpContent.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            progress?.Report(100);
        }
        
        return data;
    }
}
#endif