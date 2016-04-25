using System;

namespace Redmine.Net.Api.Exceptions
{
	public class NotFoundException : RedmineException
	{
		public NotFoundException(string message, Exception innerException)
			: base(message, innerException) { }
	}
}

