using System;
using Redmine.Net.Api.Http.Constants;

namespace Redmine.Net.Api.Exceptions;

/// <summary>
/// Represents an exception thrown when a Redmine API operation is canceled, typically due to a CancellationToken.
/// </summary>
[Serializable]
public sealed class RedmineOperationCanceledException : RedmineException
{
    /// <summary>
    /// Gets the error code for this exception.
    /// </summary>
    public override string ErrorCode => "REDMINE-CANCEL-003";

    /// <summary>
    /// Gets the URL of the Redmine API resource associated with the canceled operation, if applicable.
    /// </summary>
    public string Url { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineOperationCanceledException"/> class.
    /// </summary>
    public RedmineOperationCanceledException()
        : base("The Redmine API operation was canceled.")
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineOperationCanceledException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
    public RedmineOperationCanceledException(string message)
        : base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineOperationCanceledException"/> class with a specified error message and URL.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="url">The URL of the Redmine API resource associated with the canceled operation.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> or <paramref name="url"/> is null.</exception>
    public RedmineOperationCanceledException(string message, string url)
        : base(message)
    {
        Url = url;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineOperationCanceledException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> or <paramref name="innerException"/> is null.</exception>
    public RedmineOperationCanceledException(string message, Exception innerException)
        : base(message, innerException)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineOperationCanceledException"/> class with a specified error message, URL, and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="url">The URL of the Redmine API resource associated with the canceled operation.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or null if none.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> or <paramref name="url"/> is null.</exception>
    public RedmineOperationCanceledException(string message, string url, Exception innerException)
        : base(message, innerException)
    {
       Url = url;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineOperationCanceledException"/> class with a specified error message, URL, and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="url">The URL of the Redmine API resource associated with the canceled operation.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or null if none.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> or <paramref name="url"/> is null.</exception>
    public RedmineOperationCanceledException(string message, Uri url, Exception innerException)
        : base(message, innerException)
    {
       Url = url?.ToString();
    }
}