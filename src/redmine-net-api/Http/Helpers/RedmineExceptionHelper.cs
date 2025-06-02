using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http.Constants;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Http.Helpers;

/// <summary>
/// Handles HTTP status codes and converts them to appropriate Redmine exceptions.
/// </summary>
internal static class RedmineExceptionHelper
{
    /// <summary>
    /// Creates an exception for a 422 Unprocessable Entity response.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="responseStream">The response stream containing error details.</param>
    /// <param name="inner">The inner exception, if any.</param>
    /// <param name="serializer">The serializer to use for deserializing error messages.</param>
    /// <returns>A RedmineApiException with details about the validation errors.</returns>
    internal static RedmineApiException CreateUnprocessableEntityException(string uri, Stream responseStream, Exception inner, IRedmineSerializer serializer)
    {
        var errors = GetRedmineErrors(responseStream, serializer);

        if (errors is null)
        {
            return new RedmineUnprocessableEntityException(HttpConstants.ErrorMessages.UnprocessableEntity ,uri, inner);
        }

        var message = BuildUnprocessableContentMessage(errors);
        
        return new RedmineUnprocessableEntityException(message, url: uri, inner);
    }
    
    internal static RedmineApiException CreateUnprocessableEntityException(string url, string content, IRedmineSerializer serializer)
    {
        var paged = serializer.DeserializeToPagedResults<Error>(content);
       
        var message = BuildUnprocessableContentMessage(paged.Items);
        
        return new RedmineApiException(message: message, url: url, httpStatusCode: HttpConstants.StatusCodes.UnprocessableEntity, innerException: null);
    }

    internal static string BuildUnprocessableContentMessage(List<Error> errors)
    {
        var sb = new StringBuilder();
        foreach (var error in errors)
        {
            sb.Append(error.Info);
            sb.Append(Environment.NewLine);
        }

        if (sb.Length > 0)
        {
            sb.Length -= 1;
        }
        
        return sb.ToString();
    }
    
    /// <summary>
    /// Gets the Redmine errors from a response stream.
    /// </summary>
    /// <param name="responseStream">The response stream containing error details.</param>
    /// <param name="serializer">The serializer to use for deserializing error messages.</param>
    /// <returns>A list of error objects or null if unable to parse errors.</returns>
    private static List<Error> GetRedmineErrors(Stream responseStream, IRedmineSerializer serializer)
    {
        if (responseStream == null)
        {
            return null;
        }
        
        using (responseStream)
        {
            using var reader = new StreamReader(responseStream);
            var content = reader.ReadToEnd();
            if (content.IsNullOrWhiteSpace())
            {
                return null;
            }

            var paged = serializer.DeserializeToPagedResults<Error>(content);
            return paged.Items;
        }
    }
}
