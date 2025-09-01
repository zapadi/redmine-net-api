#nullable enable
using System;
using System.Collections.Generic;

namespace Padi.RedmineApi.Exceptions;

/// <summary>
/// 
/// </summary>
[Serializable]
public sealed class RedmineApiException : RedmineException
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="message"></param>
    public RedmineApiException(RedmineApiErrorCode errorCode, string message)
        : this(errorCode, message,false) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="message"></param>
    /// <param name="isTransient"></param>
    public RedmineApiException(RedmineApiErrorCode errorCode, string message, bool isTransient)
        : this(errorCode, message, null, isTransient) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    /// <param name="errorCode"></param>
    /// <param name="isTransient"></param>
    /// <param name="correlationId"></param>
    /// <param name="method"></param>
    /// <param name="endpoint"></param>
    /// <param name="httpStatusCode"></param>
    /// <param name="responseHeaders"></param>
    public RedmineApiException(RedmineApiErrorCode errorCode, 
        string message, 
        Exception? inner = null, 
        bool isTransient = false, 
        string? correlationId = null,
        string? method = null, 
        string? endpoint = null,
        int? httpStatusCode = null,
        IDictionary<string,string>? responseHeaders = null)
        : base(errorCode, message, inner)
    {
        ErrorCode = errorCode;
        IsTransient = isTransient;
        CorrelationId = correlationId;
        HttpStatusCode = httpStatusCode;
        Method =  method;
        Endpoint = endpoint;
        ResponseHeaders = responseHeaders;
    }
    
    /// <summary>
    /// Gets a value indicating whether the exception is Transient and the operation can be retried.
    /// </summary>
    /// <value>Value indicating whether the exception is transient or not.</value>
    public bool IsTransient { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public string? CorrelationId { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public int? HttpStatusCode { get; }
    /// <summary>
    /// 
    /// </summary>
    public string? Method { get; }
    /// <summary>
    /// 
    /// </summary>
    public string? Endpoint { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public IDictionary<string,string>? ResponseHeaders { get; }
}