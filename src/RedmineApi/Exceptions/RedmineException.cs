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

namespace Padi.RedmineApi.Exceptions;

/// <summary>
/// General redmine exception
/// </summary>
/// <seealso cref="System.Exception" />
public class RedmineException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineException"/> class.
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="message">The message that describes the error.</param>
    public RedmineException(RedmineApiErrorCode errorCode, string message)
        : this(errorCode, message, null)
    {
        ErrorCode = errorCode;
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public RedmineException(RedmineApiErrorCode errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
    
    /// <summary>
    /// Gets the error code parameter.
    /// </summary>
    /// <value>The error code associated with the <see cref="RedmineException" /> exception.</value>
    public RedmineApiErrorCode ErrorCode { get; protected set; }
}