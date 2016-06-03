using System;
using System.Runtime.Serialization;

namespace Redmine.Net.Api.Exceptions
{
	public class InternalServerErrorException : RedmineException
	{
		public InternalServerErrorException()
			: base() { }

		public InternalServerErrorException(string message)
			: base(message) { }

		public InternalServerErrorException(string format, params object[] args)
			: base(string.Format(format, args)) { }

		public InternalServerErrorException(string message, Exception innerException)
			: base(message, innerException) { }

		public InternalServerErrorException(string format, Exception innerException, params object[] args)
			: base(string.Format(format, args), innerException) { }

		protected InternalServerErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context) { }
	}
}

