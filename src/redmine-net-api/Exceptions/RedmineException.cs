/*
   Copyright 2011 - 2025 Adrian Popescu

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
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Redmine.Net.Api.Exceptions
{
    /// <summary>
    /// Thrown in case something went wrong in Redmine
    /// </summary>
    /// <seealso cref="System.Exception" />
    [DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
    [Serializable]
    public class RedmineException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual string ErrorCode => "REDMINE-GEN-001";

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> ErrorDetails { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public RedmineException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public RedmineException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public RedmineException(string message, Exception innerException) : base(message, innerException) { }
        
#if !(NET8_0_OR_GREATER) 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected RedmineException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public RedmineException AddErrorDetail(string key, string value)
        {
            ErrorDetails ??= new Dictionary<string, string>();
            
            ErrorDetails[key] = value;
            return this;
        }
        
        private string DebuggerDisplay => $"[{Message}]";
    }
}