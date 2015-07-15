using System;
using System.IO;

namespace Redmine.Net.Api
{
    public class LogEventArgs : EventArgs
    {
        public string Method { get; set; }

        public string Address { get; set; }

        public string Content { get; set; }

        public override string ToString()
        {
            return String.Format("[{0}] {1}", Method, Address);
        }
    }


    public class SerializationErrorEventArgs : ErrorEventArgs
    {
        public string Content { get; set; }

        public SerializationErrorEventArgs(Exception exception) : base(exception)
        {
        }
    }
}