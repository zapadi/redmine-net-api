using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Internals;

/// <summary>
/// Handles HTTP status codes and converts them to appropriate Redmine exceptions.
/// </summary>
internal static class RedmineApiExceptionHelper
{
    private const string RequestIdHeader = "X-Request-Id";

    /// <summary>
    /// Maps an HTTP status code to an appropriate Redmine exception.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="inner">The inner exception, if any.</param>
    /// <param name="method">The HTTP method of the request.</param>
    /// <param name="endpoint">The request URL.</param>
    /// <param name="responseHeaders">The response headers.</param>
    /// <exception cref="RedmineApiException">Always thrown, with the status code in <see cref="RedmineApiException.HttpStatusCode"/>.</exception>
    internal static void MapStatusCodeToException(int statusCode, Exception inner, string method = null, string endpoint = null, IDictionary<string, string> responseHeaders = null)
    {
        var message = statusCode switch
        {
            HttpConstants.StatusCodes.NotFound => HttpConstants.ErrorMessages.NotFound,
            HttpConstants.StatusCodes.Unauthorized => HttpConstants.ErrorMessages.Unauthorized,
            HttpConstants.StatusCodes.Forbidden => HttpConstants.ErrorMessages.Forbidden,
            HttpConstants.StatusCodes.Conflict => HttpConstants.ErrorMessages.Conflict,
            HttpConstants.StatusCodes.NotAcceptable => HttpConstants.ErrorMessages.NotAcceptable,
            HttpConstants.StatusCodes.InternalServerError => HttpConstants.ErrorMessages.InternalServerError,
            _ => $"{(HttpStatusCode)statusCode}"
        };

        throw Create(message, inner, statusCode, method, endpoint, responseHeaders);
    }

    internal static RedmineApiException Create(string message, Exception inner, int? statusCode, string method, string endpoint, IDictionary<string, string> responseHeaders, bool isTransient = false)
    {
        string correlationId = null;
        responseHeaders?.TryGetValue(RequestIdHeader, out correlationId);

        return new RedmineApiException(
            message,
            inner,
            isTransient || (statusCode.HasValue && IsTransientStatusCode(statusCode.Value)),
            correlationId,
            method,
            endpoint,
            statusCode,
            responseHeaders);
    }

    private static bool IsTransientStatusCode(int statusCode)
    {
        return statusCode is 408 or 429 or 500 or 502 or 503 or 504;
    }

    /// <summary>
    /// Creates an exception for a 422 Unprocessable Entity response.
    /// </summary>
    /// <param name="responseStream">The response stream containing error details.</param>
    /// <param name="inner">The inner exception, if any.</param>
    /// <param name="serializer">The serializer to use for deserializing error messages.</param>
    /// <returns>A RedmineException with details about the validation errors.</returns>
    private static RedmineApiException CreateUnprocessableEntityException(Stream responseStream, Exception inner, IRedmineSerializer serializer)
    {
        var errors = GetRedmineErrors(responseStream, serializer);

        return Create(errors, inner);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="responseContent"></param>
    /// <param name="inner"></param>
    /// <param name="serializer"></param>
    /// <param name="method"></param>
    /// <param name="endpoint"></param>
    /// <param name="responseHeaders"></param>
    /// <returns></returns>
    public static RedmineApiException CreateUnprocessableEntityException(string responseContent, Exception inner, IRedmineSerializer serializer, string method = null, string endpoint = null, IDictionary<string, string> responseHeaders = null)
    {
        var errors = GetRedmineErrors(responseContent, serializer);

        return Create(errors, inner, method, endpoint, responseHeaders);
    }

    private static RedmineApiException Create(List<Error> errors, Exception inner, string method = null, string endpoint = null, IDictionary<string, string> responseHeaders = null)
    {
        if (errors is null)
        {
            return Create(HttpConstants.ErrorMessages.UnprocessableEntity, inner, HttpConstants.StatusCodes.UnprocessableEntity, method, endpoint, responseHeaders);
        }

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

        return Create($"{sb}", inner, HttpConstants.StatusCodes.UnprocessableEntity, method, endpoint, responseHeaders);
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
            try
            {
                using var reader = new StreamReader(responseStream);
                var content = reader.ReadToEnd();
                return GetRedmineErrors(content, serializer);
            }
            catch (Exception ex)
            {
                throw new RedmineApiException(ex.Message, ex, httpStatusCode: HttpConstants.StatusCodes.UnprocessableEntity);
            }
        }
    }

    /// <summary>
    /// Gets the Redmine errors from response content.
    /// </summary>
    /// <param name="content">The response content as a string.</param>
    /// <param name="serializer">The serializer to use for deserializing error messages.</param>
    /// <returns>A list of error objects or null if unable to parse errors.</returns>
    private static List<Error> GetRedmineErrors(string content, IRedmineSerializer serializer)
    {
        if (content.IsNullOrWhiteSpace())
        {
            return null;
        }

        try
        {
            var paged = serializer.DeserializeToPagedResults<Error>(content);
            return (List<Error>)paged.Items;
        }
        catch (Exception ex)
        {
            throw new RedmineApiException(ex.Message, ex, httpStatusCode: HttpConstants.StatusCodes.UnprocessableEntity);
        }
    }
}