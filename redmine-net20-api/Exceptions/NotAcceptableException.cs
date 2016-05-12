using System;
using System.Runtime.Serialization;

namespace Redmine.Net.Api.Exceptions
{
	public class NotAcceptableException : RedmineException
	{
		public NotAcceptableException()
			: base() { }

		public NotAcceptableException(string message)
			: base(message) { }

		public NotAcceptableException(string format, params object[] args)
			: base(string.Format(format, args)) { }

		public NotAcceptableException(string message, Exception innerException)
			: base(message, innerException) { }

		public NotAcceptableException(string format, Exception innerException, params object[] args)
			: base(string.Format(format, args), innerException) { }

		protected NotAcceptableException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}

