using System;
using Redmine.Net.Api.Http.Constants;

namespace Redmine.Net.Api.Exceptions;

/// <summary>
/// Represents an exception thrown when a Redmine API request cannot be processed due to semantic errors (HTTP 422).
/// </summary>
[Serializable]
public class RedmineUnprocessableEntityException : RedmineApiException
{
    /// <summary>
    /// Gets the error code for this exception.
    /// </summary>
    public override string ErrorCode => "REDMINE-UNPROCESSABLE-009";

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineUnprocessableEntityException"/> class.
    /// </summary>
    public RedmineUnprocessableEntityException()
        : base("The Redmine API request cannot be processed due to invalid data.", (string)null, HttpConstants.StatusCodes.UnprocessableEntity, null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineUnprocessableEntityException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
    public RedmineUnprocessableEntityException(string message)
        : base(message, (string)null, HttpConstants.StatusCodes.UnprocessableEntity, null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineUnprocessableEntityException"/> class with a specified error message and URL.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="url">The URL of the Redmine API resource that caused the error.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
    public RedmineUnprocessableEntityException(string message, string url)
        : base(message, url, HttpConstants.StatusCodes.UnprocessableEntity, null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineUnprocessableEntityException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> or <paramref name="innerException"/> is null.</exception>
    public RedmineUnprocessableEntityException(string message, Exception innerException)
        : base(message, (string)null, HttpConstants.StatusCodes.UnprocessableEntity, innerException)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineUnprocessableEntityException"/> class with a specified error message, URL, and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="url">The URL of the Redmine API resource that caused the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> or <paramref name="innerException"/> is null.</exception>
    public RedmineUnprocessableEntityException(string message, string url, Exception innerException)
        : base(message, url, HttpConstants.StatusCodes.UnprocessableEntity, innerException)
    { }
}