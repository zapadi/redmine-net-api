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
using System.Globalization;
using System.Runtime.Serialization;

namespace Redmine.Net.Api.Exceptions
{
    /// <summary>
    /// Thrown when authentication is required or has failed (HTTP 401 Unauthorized).
    /// </summary>
    /// <remarks>
    /// <para>
    /// This exception indicates that the request requires authentication or that the provided
    /// credentials (API key or username/password) are invalid or expired.
    /// </para>
    /// <para>
    /// Common causes:
    /// <list type="bullet">
    /// <item><description>Invalid or expired API key</description></item>
    /// <item><description>Incorrect username or password</description></item>
    /// <item><description>No authentication credentials provided</description></item>
    /// <item><description>User account is locked or disabled</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var manager = new RedmineManager(options);
    ///     var issues = await manager.GetAsync&lt;Issue&gt;();
    /// }
    /// catch (UnauthorizedException ex)
    /// {
    ///     Console.WriteLine("Authentication failed. Please check your API key or credentials.");
    ///     // Verify API key is valid and not expired
    ///     // Check that authentication is properly configured
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="Redmine.Net.Api.Exceptions.RedmineException" />
    [Serializable]
    public sealed class UnauthorizedException : RedmineException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UnauthorizedException" /> class.
        /// </summary>
        public UnauthorizedException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UnauthorizedException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnauthorizedException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UnauthorizedException" /> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public UnauthorizedException(string format, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture,format, args))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UnauthorizedException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a null reference (Nothing in
        ///     Visual Basic) if no inner exception is specified.
        /// </param>
        public UnauthorizedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UnauthorizedException" /> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="args">The arguments.</param>
        public UnauthorizedException(string format, Exception innerException, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture,format, args), innerException)
        {
        }
#if !(NET8_0_OR_GREATER) 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        /// <exception cref="NotImplementedException"></exception>
        private UnauthorizedException(SerializationInfo serializationInfo, StreamingContext streamingContext):base(serializationInfo, streamingContext)
        {
         
        }
#endif
    }
}