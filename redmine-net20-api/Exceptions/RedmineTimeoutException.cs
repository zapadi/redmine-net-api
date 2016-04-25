using System;
using System.Runtime.Serialization;

namespace Redmine.Net.Api.Exceptions
{
	public class RedmineTimeoutException: RedmineException
	{
		public RedmineTimeoutException()
			: base() { }

		public RedmineTimeoutException(string message)
			: base(message) { }

		public RedmineTimeoutException(string format, params object[] args)
			: base(string.Format(format, args)) { }

		public RedmineTimeoutException(string message, Exception innerException)
			: base(message, innerException) { }

		public RedmineTimeoutException(string format, Exception innerException, params object[] args)
			: base(string.Format(format, args), innerException) { }

		protected RedmineTimeoutException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}

