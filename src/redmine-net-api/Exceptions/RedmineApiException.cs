using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redmine.Net.Api.Exceptions
{
    /// <summary>
    /// Represents errors that occur during Redmine API HTTP requests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This exception provides detailed information about API failures including HTTP status codes,
    /// request correlation IDs, endpoints, and whether the error is transient (retryable).
    /// </para>
    /// <para>
    /// Use the <see cref="IsTransient"/> property to determine if the operation can be safely retried.
    /// Transient errors include network timeouts, temporary server errors (5xx), and rate limiting.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var issue = await manager.GetAsync&lt;Issue&gt;("12345");
    /// }
    /// catch (RedmineApiException ex) when (ex.IsTransient)
    /// {
    ///     // Retry logic for transient errors
    ///     Console.WriteLine($"Transient error (HTTP {ex.HttpStatusCode}), retrying...");
    ///     await Task.Delay(1000);
    ///     // Retry the operation
    /// }
    /// catch (RedmineApiException ex)
    /// {
    ///     Console.WriteLine($"API Error: {ex.Message}");
    ///     Console.WriteLine($"Status Code: {ex.HttpStatusCode}");
    ///     Console.WriteLine($"Endpoint: {ex.Endpoint}");
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public sealed class RedmineApiException : RedmineException
    {
        /// <summary>
        /// 
        /// </summary>
        public RedmineApiException()
            : this(errorCode: null, false) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public RedmineApiException(string message)
            : this(message, errorCode: null, false) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public RedmineApiException(string message, Exception innerException)
            : this(message, innerException, errorCode: null, false) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="isTransient"></param>
        public RedmineApiException(string errorCode, bool isTransient)
            : this(string.Empty, errorCode, isTransient) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        /// <param name="isTransient"></param>
        public RedmineApiException(string message, string errorCode, bool isTransient)
            : this(message, null, errorCode, isTransient) { }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        /// <param name="errorCode"></param>
        /// <param name="isTransient"></param>
        public RedmineApiException(string message, Exception inner, string errorCode, bool isTransient)
            : base(message, inner)
        {
            this.ErrorCode = errorCode ?? "UNKNOWN";
            this.IsTransient = isTransient;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        /// <param name="isTransient"></param>
        /// <param name="correlationId"></param>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="httpStatusCode"></param>
        /// <param name="responseHeaders"></param>
        public RedmineApiException(
            string message, 
            Exception? inner = null, 
            bool isTransient = false, 
            string? correlationId = null,
            string? method = null, 
            string? endpoint = null,
            int? httpStatusCode = null,
            IDictionary<string,string>? responseHeaders = null)
            : base(message, inner)
        {
            IsTransient = isTransient;
            CorrelationId = correlationId;
            HttpStatusCode = httpStatusCode;
            Method =  method;
            Endpoint = endpoint;
            ResponseHeaders = responseHeaders;
        }
        
        /// <summary>
        /// Gets the error code parameter.
        /// </summary>
        /// <value>The error code associated with the <see cref="RedmineApiException" /> exception.</value>
        public string ErrorCode { get; }

        /// <summary>
        /// Gets a value indicating whether gets exception is Transient and operation can be retried.
        /// </summary>
        /// <value>Value indicating whether the exception is transient or not.</value>
        public bool IsTransient { get; }
        
        /// <summary>
        /// Gets the correlation ID for tracking the request across systems.
        /// </summary>
        /// <value>
        /// A unique identifier for correlating this request with server logs, or <c>null</c> if not available.
        /// </value>
        public string? CorrelationId { get; }
    
        /// <summary>
        /// Gets the HTTP status code returned by the Redmine server.
        /// </summary>
        /// <value>
        /// The HTTP status code (e.g., 404, 401, 500), or <c>null</c> if the request didn't reach the server.
        /// </value>
        /// <remarks>
        /// Common status codes:
        /// <list type="bullet">
        /// <item><description>401 - Unauthorized (authentication required or failed)</description></item>
        /// <item><description>403 - Forbidden (insufficient permissions)</description></item>
        /// <item><description>404 - Not Found (resource doesn't exist)</description></item>
        /// <item><description>422 - Unprocessable Entity (validation error)</description></item>
        /// <item><description>500 - Internal Server Error (server-side error)</description></item>
        /// </list>
        /// </remarks>
        public int? HttpStatusCode { get; }
        
        /// <summary>
        /// Gets the HTTP method used for the request.
        /// </summary>
        /// <value>
        /// The HTTP method (GET, POST, PUT, DELETE), or <c>null</c> if not available.
        /// </value>
        public string? Method { get; }
        
        /// <summary>
        /// Gets the API endpoint that was called.
        /// </summary>
        /// <value>
        /// The relative endpoint path (e.g., "/issues/12345.json"), or <c>null</c> if not available.
        /// </value>
        public string? Endpoint { get; }
    
        /// <summary>
        /// Gets the HTTP response headers returned by the server.
        /// </summary>
        /// <value>
        /// A dictionary of response headers, or <c>null</c> if not available.
        /// </value>
        /// <remarks>
        /// Response headers may include rate limiting information, server version, and other metadata.
        /// </remarks>
        public IDictionary<string,string>? ResponseHeaders { get; }
        
        #if !(NET8_0_OR_GREATER)
        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(this.ErrorCode), this.ErrorCode);
            info.AddValue(nameof(this.IsTransient), this.IsTransient);
        }
        #endif
    }
}