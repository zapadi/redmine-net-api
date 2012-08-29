using System;
using System.Runtime.Serialization;

namespace Redmine.Net.Api
{
    public class RedmineException : Exception
    {
        public RedmineException()
            : base() { }

        public RedmineException(string message)
            : base(message) { }

        public RedmineException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        public RedmineException(string message, Exception innerException)
            : base(message, innerException) { }

        public RedmineException(string format, Exception innerException, params object[] args)
            : base(string.Format(format, args), innerException) { }

        protected RedmineException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}