/*
   Copyright 2011 - 2016 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Diagnostics;

namespace Redmine.Net.Api.Logging
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