using System;

namespace Redmine.Net.Api.Exceptions;

/// <summary>
/// Represents an exception thrown when a serialization or deserialization error occurs in the Redmine API client.
/// </summary>
[Serializable]
public sealed class RedmineSerializationException : RedmineException
{
    /// <inheritdoc />
    public override string ErrorCode => "REDMINE-SERIALIZATION-008";

    /// <summary>
    /// Gets the name of the parameter that caused the serialization or deserialization error, if applicable.
    /// </summary>
    public string ParamName { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineSerializationException"/> class.
    /// </summary>
    public RedmineSerializationException()
        : base("A serialization or deserialization error occurred in the Redmine API client.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineSerializationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
    public RedmineSerializationException(string message)
        : base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineSerializationException"/> class with a specified error message and parameter name.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="paramName">The name of the parameter that caused the serialization or deserialization error.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> or <paramref name="paramName"/> is null.</exception>
    public RedmineSerializationException(string message, string paramName)
        : base(message)
    {
       ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineSerializationException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> or <paramref name="innerException"/> is null.</exception>
    public RedmineSerializationException(string message, Exception innerException)
        : base(message, innerException)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineSerializationException"/> class with a specified error message, parameter name, and inner exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="paramName">The name of the parameter that caused the serialization or deserialization error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/>, <paramref name="paramName"/>, or <paramref name="innerException"/> is null.</exception>
    public RedmineSerializationException(string message, string paramName, Exception innerException)
        : base(message, innerException)
    {
       ParamName = paramName ?? throw new ArgumentNullException(nameof(paramName));
    }
}