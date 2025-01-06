#if NET45_OR_GREATER || NETCOREAPP
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Net.HttpClient.Extensions;

internal static class HttpExceptionExtensions
{
    public static void HandleApiRequestException(this HttpRequestException exception, IRedmineSerializer serializer)
    {
        if (exception == null)
        {
            return;
        }

        var innerException = exception.InnerException ?? exception;

        #if NET5_0_OR_GREATER
        if (exception.StatusCode == HttpStatusCode.UnprocessableEntity)
        #else
        if (innerException is WebException webException && (int)webException.Status == 422)
        #endif
        {
            // var errors = GetRedmineExceptions(exception.Response, serializer);
            //
            // if (errors != null)
            // {
            //     var sb = new StringBuilder();
            //     foreach (var error in errors)
            //     {
            //         sb.Append(error.Info).Append(Environment.NewLine);
            //     }
            //                         
            //     redmineException = new RedmineException($"Invalid or missing attribute parameters: {sb}", innerException, "Unprocessable Content");
            //     sb.Length = 0;
            // }
            // else
            // {
            //     redmineException = new RedmineException("Invalid or missing attribute parameters", innerException);
            // }
            //                    
            // throw redmineException;
        }
 
        throw new RedmineException(innerException.Message, innerException);
       
    }
    
    public static async Task HandleUnprocessableResponse(this HttpResponseMessage httpResponseMessage, IRedmineSerializer serializer, CancellationToken cancellationToken)
    {
        if (httpResponseMessage == null)
        {
            return;
        }

        var data =  await httpResponseMessage.Content.ReadAsByteArrayAsync(
        #if !NETFRAMEWORK
            cancellationToken
        #endif
        ).ConfigureAwait(false);
          
        if (data.Length == 0)
        {
            return;
        }

        var content = Encoding.UTF8.GetString(data);
        var result = serializer.DeserializeToPagedResults<Error>(content);

        if (result != null && result.Items != null)
        {
            var sb = new StringBuilder();
            foreach (var error in result.Items)
            {
                sb.Append(error.Info).Append(Environment.NewLine);
            }
                
            content = sb.ToString();
            sb.Length = 0;
        }
                
        throw new RedmineApiException(content, "Unprocessable Content", false); 
    }
}
#endif