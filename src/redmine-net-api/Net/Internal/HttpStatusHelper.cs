using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Net.Internal;

internal static class HttpStatusHelper
{
    internal static void MapStatusCodeToException(int statusCode, Stream responseStream, Exception inner, IRedmineSerializer serializer)
    {
        switch (statusCode)
        {
            case (int)HttpStatusCode.NotFound:
                throw new NotFoundException("Not found.", inner);

            case (int)HttpStatusCode.Unauthorized:
                throw new UnauthorizedException("Unauthorized.", inner);

            case (int)HttpStatusCode.Forbidden:
                throw new ForbiddenException("Forbidden.", inner);

            case (int)HttpStatusCode.Conflict:
                throw new ConflictException("The page that you are trying to update is stale!", inner);

            case 422:
                var exception = CreateUnprocessableEntityException(responseStream, inner, serializer);
                throw exception;

            case (int)HttpStatusCode.NotAcceptable:
                throw new NotAcceptableException("Not acceptable.", inner);

            case (int)HttpStatusCode.InternalServerError:
                throw new InternalServerErrorException("Internal server error.", inner);

            default:
                throw new RedmineException($"HTTP {(int)statusCode} – {statusCode}", inner);
        }
    }

    private static RedmineException CreateUnprocessableEntityException(
        Stream responseStream,
        Exception   inner,
        IRedmineSerializer serializer)
    {
        var errors = GetRedmineErrors(responseStream, serializer);

        if (errors is null)
        {
            return new RedmineException("Unprocessable Content", inner);
        }

        var sb = new StringBuilder();
        foreach (var error in errors)
        {
            sb.Append(error.Info).Append(Environment.NewLine);
        }

        sb.Length -= 1;
        return new RedmineException($"Unprocessable Content: {sb}", inner);
    }


    /// <summary>
    /// Gets the redmine exceptions.
    /// </summary>
    /// <param name="responseStream"></param>
    /// <param name="serializer"></param>
    /// <returns></returns>
    private static List<Error> GetRedmineErrors(Stream responseStream, IRedmineSerializer serializer)
    {
        if (responseStream == null)
        {
            return null;
        }
        
        using (responseStream)
        {
            using var streamReader = new StreamReader(responseStream);
            var responseContent = streamReader.ReadToEnd();

            return GetRedmineErrors(responseContent, serializer);
        }
    }
    
    private static List<Error> GetRedmineErrors(string content, IRedmineSerializer serializer)
    {
        if (content.IsNullOrWhiteSpace()) return null;

        var paged = serializer.DeserializeToPagedResults<Error>(content);
        return (List<Error>)paged.Items;
    }
}