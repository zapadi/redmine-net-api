namespace Padi.RedmineApi.Internals;

/// <summary>
/// 
/// </summary>
internal static class HttpConstants
{
    /// <summary>
    /// HTTP status codes including custom codes used by Redmine.
    /// </summary>
    internal static class StatusCodes
    {
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int NotAcceptable = 406;
        public const int RequestTimeout = 408;
        public const int Conflict = 409;
        public const int UnprocessableEntity = 422;
        public const int TooManyRequests = 429;
        public const int InternalServerError = 500;
        public const int BadGateway = 502;
        public const int ServiceUnavailable = 503;
        public const int GatewayTimeout = 504;
    }

    /// <summary>
    /// Standard HTTP headers used in API requests and responses.
    /// </summary>
    internal static class Headers
    {
        public const string Authorization = "Authorization";
        public const string ApiKey = "X-Redmine-API-Key";
        public const string Impersonate = "X-Redmine-Switch-User";
        public const string ContentType = "Content-Type";
    }
    
    internal static class Names
    {
        /// <summary>HTTP User-Agent header name.</summary>
        public static string UserAgent => "User-Agent";
    }

    internal static class Values
    {
        /// <summary>User agent string to use for all HTTP requests.</summary>
        public static string UserAgent => "Redmine-NET-API";
    }

    /// <summary>
    /// MIME content types used in API requests and responses.
    /// </summary>
    internal static class ContentTypes
    {
        public const string ApplicationJson = "application/json";
        public const string ApplicationXml = "application/xml";
        public const string ApplicationOctetStream = "application/octet-stream";
    }

    /// <summary>
    /// Error messages for different HTTP status codes.
    /// </summary>
    internal static class ErrorMessages
    {
        public const string NotFound = "The requested resource was not found.";
        public const string Unauthorized = "Authentication is required or has failed.";
        public const string Forbidden = "You don't have permission to access this resource.";
        public const string Conflict = "The resource you are trying to update has been modified since you last retrieved it.";
        public const string NotAcceptable = "The requested format is not supported.";
        public const string InternalServerError = "The server encountered an unexpected error.";
        public const string UnprocessableEntity = "Validation failed for the submitted data.";
        public const string Cancelled = "The operation was cancelled.";
        public const string TimedOut = "The operation has timed out.";
    }
    
    /// <summary>
    /// 
    /// </summary>
    internal static class HttpVerbs
    {
        /// <summary>
        /// Represents an HTTP GET protocol method that is used to get an entity identified by a URI.
        /// </summary>
        public const string GET = "GET";
        /// <summary>
        /// Represents an HTTP PUT protocol method that is used to replace an entity identified by a URI.
        /// </summary>
        public const string PUT = "PUT";
        /// <summary>
        /// Represents an HTTP POST protocol method that is used to post a new entity as an addition to a URI.
        /// </summary>
        public const string POST = "POST";
        /// <summary>
        /// Represents an HTTP PATCH protocol method that is used to patch an existing entity identified  by a URI.
        /// </summary>
        public const string PATCH = "PATCH";
        /// <summary>
        /// Represents an HTTP DELETE protocol method that is used to delete an existing entity identified  by a URI.
        /// </summary>
        public const string DELETE = "DELETE";
        
        internal const string DOWNLOAD = "DOWNLOAD";
        
        internal const string UPLOAD = "UPLOAD";
    }
}