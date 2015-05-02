/*
   Copyright 2011 - 2014 Adrian Popescu, Dorin Huzum.

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