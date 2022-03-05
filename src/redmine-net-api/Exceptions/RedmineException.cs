/*
   Copyright 2011 - 2022 Adrian Popescu

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
using System.Globalization;
using System.Runtime.Serialization;

namespace Redmine.Net.Api.Exceptions
{
    /// <summary>
    /// Thrown in case something went wrong in Redmine
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class RedmineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineException"/> class.
        /// </summary>
        public RedmineException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RedmineException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineException"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public RedmineException(string format, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture,format, args))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public RedmineException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineException" /> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="args">The arguments.</param>
        public RedmineException(string format, Exception innerException, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture,format, args), innerException)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        protected RedmineException(SerializationInfo serializationInfo, StreamingContext streamingContext):base(serializationInfo, streamingContext)
        {
            
        }
    }
}