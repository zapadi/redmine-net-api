using System;
using System.Runtime.Serialization;

namespace Redmine.Net.Api.Exceptions
{
	public class UnauthorizedException : RedmineException
	{
		public UnauthorizedException()
			: base() { }

		public UnauthorizedException(string message)
			: base(message) { }

		public UnauthorizedException(string format, params object[] args)
			: base(string.Format(format, args)) { }

		public UnauthorizedException(string message, Exception innerException)
			: base(message, innerException) { }

		public UnauthorizedException(string format, Exception innerException, params object[] args)
			: base(string.Format(format, args), innerException) { }

		protected UnauthorizedException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}

