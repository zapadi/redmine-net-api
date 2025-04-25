using System;

namespace Redmine.Net.Api.Exceptions;

/// <summary>
/// Represents an error that occurs during JSON serialization or deserialization.
/// </summary>
public class RedmineSerializationException : RedmineException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineSerializationException"/> class.
    /// </summary>
    public RedmineSerializationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineSerializationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public RedmineSerializationException(string message) : base(message)
    {
    }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineSerializationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    ///  /// <param name="paramName">The name of the parameter that caused the exception.</param>
    public RedmineSerializationException(string message, string paramName) : base(message)
    {
        ParamName = paramName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineSerializationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public RedmineSerializationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Gets the name of the parameter that caused the current exception.
    /// </summary>
    public string ParamName { get; }
}