using System;
using System.Runtime.Serialization;

namespace Redmine.Net.Api.Exceptions
{
	public class NameResolutionFailureException : RedmineException
	{
		public NameResolutionFailureException()
			: base() { }

		public NameResolutionFailureException(string message)
			: base(message) { }

		public NameResolutionFailureException(string format, params object[] args)
			: base(string.Format(format, args)) { }

		public NameResolutionFailureException(string message, Exception innerException)
			: base(message, innerException) { }

		public NameResolutionFailureException(string format, Exception innerException, params object[] args)
			: base(string.Format(format, args), innerException) { }

		protected NameResolutionFailureException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}

