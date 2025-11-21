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
    /// Thrown when the requested resource could not be found (HTTP 404 Not Found).
    /// </summary>
    /// <remarks>
    /// <para>
    /// This exception indicates that the specified resource (issue, project, user, etc.) does not exist
    /// on the Redmine server or the current user doesn't have permission to view it.
    /// </para>
    /// <para>
    /// Common scenarios:
    /// <list type="bullet">
    /// <item><description>Resource ID doesn't exist</description></item>
    /// <item><description>Resource was deleted</description></item>
    /// <item><description>User lacks permission to view the resource (appears as "not found" for security)</description></item>
    /// <item><description>Incorrect resource identifier format</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var issue = await manager.GetAsync&lt;Issue&gt;("99999");
    /// }
    /// catch (NotFoundException ex)
    /// {
    ///     Console.WriteLine($"Issue not found: {ex.Message}");
    ///     // Verify the ID is correct
    ///     // Check if the resource was deleted
    ///     // Ensure user has permission to view the resource
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="Redmine.Net.Api.Exceptions.RedmineException" />
    [Serializable]
    public sealed class NotFoundException : RedmineException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NotFoundException" /> class.
        /// </summary>
        public NotFoundException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotFoundException" /> class.
        /// </summary>
        /// <param name="message"></param>
        public NotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotFoundException" /> class.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public NotFoundException(string format, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture,format, args))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotFoundException" /> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotFoundException" /> class.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="innerException"></param>
        /// <param name="args"></param>
        public NotFoundException(string format, Exception innerException, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture,format, args), innerException)
        {
        }

#if !(NET8_0_OR_GREATER) 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        private NotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext):base(serializationInfo, streamingContext)
        {
         
        }
#endif
    }
}