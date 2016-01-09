using System;
using System.Diagnostics;

namespace Redmine.Net.Api
{
	public class RedmineConsoleTraceListener : TraceListener
	{
		#region implemented abstract members of TraceListener

		public override void Write (string message)
		{
			Console.Write(message);
		}

		public override void WriteLine (string message)
		{
			Console.WriteLine(message);
		}

		#endregion

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
			string message)
		{
			WriteLine(message);
		}

		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id,
			string format, params object[] args)
		{
			WriteLine(string.Format(format, args));
		}
	}
}