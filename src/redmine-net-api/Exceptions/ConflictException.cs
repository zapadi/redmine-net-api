﻿/*
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
    /// </summary>
    /// <seealso cref="Redmine.Net.Api.Exceptions.RedmineException" />
    [Serializable]
    public sealed class ConflictException : RedmineException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConflictException" /> class.
        /// </summary>
        public ConflictException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConflictException" /> class.
        /// </summary>
        /// <param name="message"></param>
        public ConflictException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConflictException" /> class.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public ConflictException(string format, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture,format, args))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConflictException" /> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ConflictException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConflictException" /> class.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="innerException"></param>
        /// <param name="args"></param>
        public ConflictException(string format, Exception innerException, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture,format, args), innerException)
        {
        }

#if !(NET8_0_OR_GREATER) 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        private ConflictException(SerializationInfo serializationInfo, StreamingContext streamingContext):base(serializationInfo, streamingContext)
        {
            
        }
#endif
    }
}