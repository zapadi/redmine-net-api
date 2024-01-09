using System;
using System.Runtime.Serialization;

namespace Redmine.Net.Api.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
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
        /// Gets the error code parameter.
        /// </summary>
        /// <value>The error code associated with the <see cref="RedmineApiException" /> exception.</value>
        public string ErrorCode { get; }

        /// <summary>
        /// Gets a value indicating whether gets exception is Transient and operation can be retried.
        /// </summary>
        /// <value>Value indicating whether the exception is transient or not.</value>
        public bool IsTransient { get; }
        
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