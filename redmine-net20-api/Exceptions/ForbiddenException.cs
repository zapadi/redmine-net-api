using System;
using System.Runtime.Serialization;

namespace Redmine.Net.Api.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
	public class ForbiddenException : RedmineException
	{
        /// <summary>
        /// 
        /// </summary>
		public ForbiddenException()
			: base() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
		public ForbiddenException(string message)
			: base(message) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
		public ForbiddenException(string format, params object[] args)
			: base(string.Format(format, args)) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
		public ForbiddenException(string message, Exception innerException)
			: base(message, innerException) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="innerException"></param>
        /// <param name="args"></param>
		public ForbiddenException(string format, Exception innerException, params object[] args)
			: base(string.Format(format, args), innerException) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
		protected ForbiddenException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}