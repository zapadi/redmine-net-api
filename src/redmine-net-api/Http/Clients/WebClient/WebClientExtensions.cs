using System;
using System.Globalization;
using System.Net;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http.Helpers;
using Redmine.Net.Api.Http.Messages;

namespace Redmine.Net.Api.Http.Clients.WebClient;

internal static class WebClientExtensions
{
    public static void ApplyHeaders(this System.Net.WebClient client, RedmineApiRequest request, IRedmineAuthentication authentication)
    {
        switch (authentication)
        {
            case RedmineApiKeyAuthentication:
                client.Headers.Add(RedmineConstants.API_KEY_AUTHORIZATION_HEADER_KEY, authentication.Token);
                break;
            case RedmineBasicAuthentication:
                client.Headers.Add(RedmineConstants.AUTHORIZATION_HEADER_KEY, authentication.Token);
                break;
        }
        
        client.Headers.Add(RedmineConstants.CONTENT_TYPE_HEADER_KEY, request.ContentType);

        if (!request.UserAgent.IsNullOrWhiteSpace())
        {
            client.Headers.Add(RedmineConstants.USER_AGENT_HEADER_KEY, request.UserAgent);
        }

        if (!request.ImpersonateUser.IsNullOrWhiteSpace())
        {
            client.Headers.Add(RedmineConstants.IMPERSONATE_HEADER_KEY, request.ImpersonateUser);
        }

        if (request.Headers is not { Count: > 0 })
        {
            return;
        }
        
        foreach (var header in request.Headers)
        {
            client.Headers.Add(header.Key, header.Value);
        }
        
        if (!request.Accept.IsNullOrWhiteSpace())
        {
            client.Headers.Add(HttpRequestHeader.Accept, request.Accept);
        }
    }
    
    internal static byte[] DownloadWithProgress(this System.Net.WebClient webClient, string url, IProgress<int> progress)
    {
        var contentLength = GetContentLength(webClient);
        byte[] data;
        if (contentLength > 0)
        {
            using (var respStream = webClient.OpenRead(url))
            {
                data = new byte[contentLength];
                var buffer = new byte[4096];
                int bytesRead;
                var totalBytesRead = 0;

                while ((bytesRead = respStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    Buffer.BlockCopy(buffer, 0, data, totalBytesRead, bytesRead);
                    totalBytesRead += bytesRead;
                                
                    ClientHelper.ReportProgress(progress, contentLength, totalBytesRead);
                }
            }
        }
        else
        {
            data = webClient.DownloadData(url);
            progress?.Report(100);    
        }
            
        return data;
    }

    internal static long GetContentLength(this System.Net.WebClient webClient)
    {
        var total = -1L;
        if (webClient.ResponseHeaders == null)
        {
            return total;
        }
            
        var contentLengthAsString = webClient.ResponseHeaders[HttpRequestHeader.ContentLength];
        if (!string.IsNullOrEmpty(contentLengthAsString))
        {
            total = Convert.ToInt64(contentLengthAsString, CultureInfo.InvariantCulture);
        }

        return total;
    }

    internal static HttpStatusCode? GetStatusCode(this System.Net.WebClient webClient)
    {
        if (webClient is InternalWebClient iwc)
        {
            return iwc.StatusCode;
        }
        
        return null;
    }
}