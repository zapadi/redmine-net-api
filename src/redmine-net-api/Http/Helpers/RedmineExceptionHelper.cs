using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
    /// Maps an HTTP status code to an appropriate Redmine exception.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="responseStream">The response stream containing error details.</param>
    /// <param name="inner">The inner exception, if any.</param>
    /// <param name="serializer">The serializer to use for deserializing error messages.</param>
    /// <exception cref="RedmineException">A specific Redmine exception based on the status code.</exception>
    internal static void MapStatusCodeToException(int statusCode, Stream responseStream, Exception inner, IRedmineSerializer serializer)
    {
        switch (statusCode)
        {
            case HttpConstants.StatusCodes.NotFound:
                throw new NotFoundException(HttpConstants.ErrorMessages.NotFound, inner);

            case HttpConstants.StatusCodes.Unauthorized:
                throw new UnauthorizedException(HttpConstants.ErrorMessages.Unauthorized, inner);

            case HttpConstants.StatusCodes.Forbidden:
                throw new ForbiddenException(HttpConstants.ErrorMessages.Forbidden, inner);

            case HttpConstants.StatusCodes.Conflict:
                throw new ConflictException(HttpConstants.ErrorMessages.Conflict, inner);

            case HttpConstants.StatusCodes.UnprocessableEntity:
                throw CreateUnprocessableEntityException(responseStream, inner, serializer);

            case HttpConstants.StatusCodes.NotAcceptable:
                throw new NotAcceptableException(HttpConstants.ErrorMessages.NotAcceptable, inner);

            case HttpConstants.StatusCodes.InternalServerError:
                throw new InternalServerErrorException(HttpConstants.ErrorMessages.InternalServerError, inner);

            default:
                throw new RedmineException($"HTTP {statusCode} – {(HttpStatusCode)statusCode}", inner);
        }
    }
    
    /// <summary>
    /// Creates an exception for a 422 Unprocessable Entity response.
    /// </summary>
    /// <param name="responseStream">The response stream containing error details.</param>
    /// <param name="inner">The inner exception, if any.</param>
    /// <param name="serializer">The serializer to use for deserializing error messages.</param>
    /// <returns>A RedmineException with details about the validation errors.</returns>
    private static RedmineException CreateUnprocessableEntityException(Stream responseStream, Exception inner, IRedmineSerializer serializer)
    {
        var errors = GetRedmineErrors(responseStream, serializer);

        if (errors is null)
        {
            return new RedmineException(HttpConstants.ErrorMessages.UnprocessableEntity, inner);
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
        
        return new RedmineException($"Unprocessable Content: {sb}", inner);
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
            catch(Exception ex)
            {
                throw new RedmineApiException(ex.Message, ex);
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
        catch(Exception ex)
        {
            throw new RedmineException(ex.Message, ex);
        }
    }
}
