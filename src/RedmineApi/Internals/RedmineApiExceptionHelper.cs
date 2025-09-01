using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Serialization;
using Padi.RedmineApi.Types;

namespace Padi.RedmineApi.Internals;

/// <summary>
/// Handles HTTP status codes and converts them to appropriate Redmine exceptions.
/// </summary>
internal static class RedmineApiExceptionHelper
{
    /// <summary>
    /// Maps an HTTP status code to an appropriate Redmine exception.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="inner">The inner exception, if any.</param>
    /// <exception cref="RedmineException">A specific Redmine exception based on the status code.</exception>
    internal static void MapStatusCodeToException(int statusCode, Exception inner)
    {
        throw statusCode switch
        {
            HttpConstants.StatusCodes.NotFound => new RedmineApiException(RedmineApiErrorCode.NotFound, HttpConstants.ErrorMessages.NotFound, inner, httpStatusCode: statusCode),
            HttpConstants.StatusCodes.Unauthorized => new RedmineApiException(RedmineApiErrorCode.Unauthorized, HttpConstants.ErrorMessages.Unauthorized, inner, httpStatusCode: statusCode),
            HttpConstants.StatusCodes.Forbidden => new RedmineApiException(RedmineApiErrorCode.Forbidden, HttpConstants.ErrorMessages.Forbidden, inner, httpStatusCode: statusCode),
            HttpConstants.StatusCodes.Conflict => new RedmineApiException(RedmineApiErrorCode.Conflict, HttpConstants.ErrorMessages.Conflict, inner, httpStatusCode: statusCode),
            HttpConstants.StatusCodes.NotAcceptable => new RedmineApiException(RedmineApiErrorCode.NotAcceptable, HttpConstants.ErrorMessages.NotAcceptable, inner, httpStatusCode: statusCode),
            HttpConstants.StatusCodes.InternalServerError => new RedmineApiException(RedmineApiErrorCode.InternalServerError, HttpConstants.ErrorMessages.InternalServerError, inner, httpStatusCode: statusCode),
            _ => new RedmineApiException(RedmineApiErrorCode.Unknown, $"{(HttpStatusCode)statusCode}", inner, httpStatusCode: statusCode)
        };
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
    /// <returns></returns>
    public static RedmineApiException CreateUnprocessableEntityException(string responseContent, Exception inner, IRedmineSerializer serializer)
    {
        var errors = GetRedmineErrors(responseContent, serializer);

        return Create(errors, inner);
    }

    private static RedmineApiException Create(List<Error> errors, Exception inner)
    {
        if (errors is null)
        {
            return new RedmineApiException(RedmineApiErrorCode.ArgumentNull, HttpConstants.ErrorMessages.UnprocessableEntity, inner, httpStatusCode: HttpConstants.StatusCodes.UnprocessableEntity);
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

        return new RedmineApiException(RedmineApiErrorCode.UnprocessableEntity, $"{sb}", inner, httpStatusCode: HttpConstants.StatusCodes.UnprocessableEntity);
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
                throw new RedmineApiException(RedmineApiErrorCode.Unknown, ex.Message, ex, httpStatusCode: HttpConstants.StatusCodes.UnprocessableEntity);
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
            throw new RedmineApiException(RedmineApiErrorCode.DeserializationError, ex.Message, ex, httpStatusCode: HttpConstants.StatusCodes.UnprocessableEntity);
        }
    }
}