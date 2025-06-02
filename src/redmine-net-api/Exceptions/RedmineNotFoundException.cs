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
    /// Thrown in case the objects requested for could not be found.
    /// </summary>
    /// <seealso cref="Redmine.Net.Api.Exceptions.RedmineException" />
    [Serializable]
    public sealed class RedmineNotFoundException : RedmineApiException
    {
        /// <inheritdoc />
        public override string ErrorCode => "REDMINE-NOTFOUND-006";
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="RedmineNotFoundException" /> class.
        /// </summary>
        public RedmineNotFoundException()
            : base("The requested Redmine API resource was not found.", (string)null, HttpConstants.StatusCodes.NotFound, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineNotFoundException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
        public RedmineNotFoundException(string message)
            : base(message, (string)null, HttpConstants.StatusCodes.NotFound, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineNotFoundException"/> class with a specified error message and URL.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="url">The URL of the Redmine API resource that was not found.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
        public RedmineNotFoundException(string message, string url)
            : base(message, url, HttpConstants.StatusCodes.NotFound, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineNotFoundException"/> class with a specified error message and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> or <paramref name="innerException"/> is null.</exception>
        public RedmineNotFoundException(string message, Exception innerException)
            : base(message, (string)null, HttpConstants.StatusCodes.NotFound, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineNotFoundException"/> class with a specified error message, URL, and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="url">The URL of the Redmine API resource that was not found.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> or <paramref name="innerException"/> is null.</exception>
        public RedmineNotFoundException(string message, string url, Exception innerException)
            : base(message, url, HttpConstants.StatusCodes.NotFound, innerException)
        { }
    }
}