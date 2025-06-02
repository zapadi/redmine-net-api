using System;
using System.Net;
#if NET40_OR_GREATER || NET
using System.Net.Http;
#endif
using System.Net.Sockets;
using System.Runtime.Serialization;
using Redmine.Net.Api.Http.Constants;

namespace Redmine.Net.Api.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RedmineApiException : RedmineException
    {
        /// <summary>
        /// Gets the error code parameter.
        /// </summary>
        /// <value>The error code associated with the <see cref="RedmineApiException" /> exception.</value>
        public override string ErrorCode => "REDMINE-API-002";

        /// <summary>
        /// Gets a value indicating whether gets exception is Transient and operation can be retried.
        /// </summary>
        /// <value>Value indicating whether the exception is transient or not.</value>
        public bool IsTransient { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string Url { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public int? HttpStatusCode { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public RedmineApiException()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public RedmineApiException(string message) : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public RedmineApiException(string message, Exception innerException) : base(message, innerException)
        {
            var transientErrorResult = IsTransientError(InnerException);
            IsTransient = transientErrorResult.IsTransient;
            HttpStatusCode = transientErrorResult.StatusCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="url"></param>
        /// <param name="httpStatusCode"></param>
        /// <param name="innerException"></param>
        public RedmineApiException(string message, string url, int? httpStatusCode = null,
            Exception innerException = null)
            : base(message, innerException)
        {
            Url = url;
            var transientErrorResult = IsTransientError(InnerException);
            IsTransient = transientErrorResult.IsTransient;
            HttpStatusCode = httpStatusCode ?? transientErrorResult.StatusCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="requestUri"></param>
        /// <param name="httpStatusCode"></param>
        /// <param name="innerException"></param>
        public RedmineApiException(string message, Uri requestUri, int? httpStatusCode = null, Exception innerException = null)
            : base(message, innerException)
        {
            Url = requestUri?.ToString();
            var transientErrorResult = IsTransientError(InnerException);
            IsTransient = transientErrorResult.IsTransient;
            HttpStatusCode = httpStatusCode ?? transientErrorResult.StatusCode;
        }

#if !(NET8_0_OR_GREATER)
        /// <inheritdoc />
        protected RedmineApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Url = info.GetString(nameof(Url));
            HttpStatusCode = info.GetInt32(nameof(HttpStatusCode));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Url), Url);
            info.AddValue(nameof(HttpStatusCode), HttpStatusCode);
        }
#endif

        internal struct TransientErrorResult(bool isTransient, int? statusCode)
        {
            public bool IsTransient = isTransient;
            public int? StatusCode = statusCode;
        }
        
        internal static TransientErrorResult IsTransientError(Exception exception)
        {
            return exception switch
            {
                null => new TransientErrorResult(false, null),
                WebException webEx => HandleWebException(webEx),
#if NET40_OR_GREATER || NET
                HttpRequestException httpRequestEx => HandleHttpRequestException(httpRequestEx),
#endif
                _ => new TransientErrorResult(false, null)
            };
        }

        private static TransientErrorResult HandleWebException(WebException webEx)
        {
            switch (webEx.Status)
            {
                case WebExceptionStatus.ProtocolError when webEx.Response is HttpWebResponse response:
                    var statusCode = (int)response.StatusCode;
                    return new TransientErrorResult(IsTransientStatusCode(statusCode), statusCode);

                case WebExceptionStatus.Timeout:
                    return new TransientErrorResult(true, HttpConstants.StatusCodes.RequestTimeout);

                case WebExceptionStatus.NameResolutionFailure:
                case WebExceptionStatus.ConnectFailure:
                    return new TransientErrorResult(true, HttpConstants.StatusCodes.ServiceUnavailable);

                default:
                    return new TransientErrorResult(false, null);
            }
        }

        private static bool IsTransientStatusCode(int statusCode)
        {
            return statusCode == HttpConstants.StatusCodes.TooManyRequests ||
                   statusCode == HttpConstants.StatusCodes.InternalServerError ||
                   statusCode == HttpConstants.StatusCodes.BadGateway ||
                   statusCode == HttpConstants.StatusCodes.ServiceUnavailable ||
                   statusCode == HttpConstants.StatusCodes.GatewayTimeout;
        }

#if NET40_OR_GREATER || NET
        private static TransientErrorResult HandleHttpRequestException(
            HttpRequestException httpRequestEx)
        {
            if (httpRequestEx.InnerException is WebException innerWebEx)
            {
                return HandleWebException(innerWebEx);
            }

            if (httpRequestEx.InnerException is SocketException socketEx)
            {
                switch (socketEx.SocketErrorCode)
                {
                    case SocketError.HostNotFound:
                    case SocketError.HostUnreachable:
                    case SocketError.NetworkUnreachable:
                    case SocketError.ConnectionRefused:
                        return new TransientErrorResult(true, HttpConstants.StatusCodes.ServiceUnavailable);

                    case SocketError.TimedOut:
                        return new TransientErrorResult(true, HttpConstants.StatusCodes.RequestTimeout);

                    default:
                        return new TransientErrorResult(false, null);
                }
            }

#if NET
            if (httpRequestEx.StatusCode.HasValue)
            {
                var statusCode = (int)httpRequestEx.StatusCode.Value;
                return new TransientErrorResult(IsTransientStatusCode(statusCode), statusCode);
            }
#endif

            return new TransientErrorResult(false, null);
        }
#endif
    }
}