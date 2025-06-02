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
using Redmine.Net.Api.Http.Constants;

namespace Redmine.Net.Api.Exceptions
{
   
    /// <summary>
    /// Represents an exception thrown when a Redmine API request is forbidden (HTTP 403).
    /// </summary>
    [Serializable]
    public sealed class RedmineForbiddenException : RedmineApiException
    {
        /// <summary>
        /// Gets the error code for this exception.
        /// </summary>
        public override string ErrorCode => "REDMINE-FORBIDDEN-005";

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineForbiddenException"/> class.
        /// </summary>
        public RedmineForbiddenException()
            : base("Access to the Redmine API resource is forbidden.", (string)null, HttpConstants.StatusCodes.Forbidden, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineForbiddenException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
        public RedmineForbiddenException(string message)
            : base(message, (string)null, HttpConstants.StatusCodes.Forbidden, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineForbiddenException"/> class with a specified error message and URL.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="url">The URL of the Redmine API resource that caused the exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
        public RedmineForbiddenException(string message, string url)
            : base(message, url, HttpConstants.StatusCodes.Forbidden, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineForbiddenException"/> class with a specified error message and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> or <paramref name="innerException"/> is null.</exception>
        public RedmineForbiddenException(string message, Exception innerException)
            : base(message, (string)null, HttpConstants.StatusCodes.Forbidden, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineForbiddenException"/> class with a specified error message, URL, and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="url">The URL of the Redmine API resource that caused the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> or <paramref name="innerException"/> is null.</exception>
        public RedmineForbiddenException(string message, string url, Exception innerException)
            : base(message, url, HttpConstants.StatusCodes.Forbidden, innerException)
        { }
    }
}