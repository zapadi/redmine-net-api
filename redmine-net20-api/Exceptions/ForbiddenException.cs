using System;
using System.Runtime.Serialization;

namespace Redmine.Net.Api.Exceptions
{
    /// <summary>
    /// </summary>
    /// <seealso cref="Redmine.Net.Api.Exceptions.RedmineException" />
    public class ForbiddenException : RedmineException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ForbiddenException" /> class.
        /// </summary>
        public ForbiddenException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ForbiddenException" /> class.
        /// </summary>
        /// <param name="message"></param>
        public ForbiddenException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ForbiddenException" /> class.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public ForbiddenException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ForbiddenException" /> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ForbiddenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ForbiddenException" /> class.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="innerException"></param>
        /// <param name="args"></param>
        public ForbiddenException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ForbiddenException" /> class.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ForbiddenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}