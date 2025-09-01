namespace Padi.RedmineApi.Exceptions;

/// <summary>
/// 
/// </summary>
public enum RedmineApiErrorCode
{
    /// <summary>
    /// 
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// 
    /// </summary>
    ArgumentInvalid = 1,
    /// <summary>
    /// 
    /// </summary>
    ArgumentNull,
    /// <summary>
    /// 
    /// </summary>
    UnsupportedHttpMethod,
    /// <summary>
    /// 
    /// </summary>
    ArgumentNullOrEmpty,
    /// <summary>
    /// 
    /// </summary>
    ValidationFailed,
    /// <summary>
    /// 
    /// </summary>
    UnprocessableEntity,
    /// <summary>
    /// 
    /// </summary>
    NotFound,
    /// <summary>
    /// 
    /// </summary>
    Conflict,
    /// <summary>
    /// 
    /// </summary>
    Unauthorized,
    /// <summary>
    /// 
    /// </summary>
    Forbidden,
    /// <summary>
    /// 
    /// </summary>
    NotAcceptable,
    /// <summary>
    /// 
    /// </summary>
    Timeout,
    /// <summary>
    /// 
    /// </summary>
    NetworkFailure,
    /// <summary>
    /// 
    /// </summary>
    ServiceUnavailable,
    /// <summary>
    /// 
    /// </summary>
    SerializerError,
    /// <summary>
    /// 
    /// </summary>
    SerializationError,
    /// <summary>
    /// 
    /// </summary>
    DeserializationError,
    /// <summary>
    /// 
    /// </summary>
    InternalServerError,
    /// <summary>
    /// 
    /// </summary>
    RequestCanceled
}