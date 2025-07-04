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

namespace Redmine.Net.Api.Exceptions
{
    /// <summary>
    /// </summary>
    /// <seealso cref="Redmine.Net.Api.Exceptions.RedmineException" />
    [Serializable]
    public sealed class RedmineNotAcceptableException : RedmineApiException
    {
        /// <inheritdoc />
        public override string ErrorCode => "REDMINE-NOT-ACCEPTABLE-010";

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedmineNotAcceptableException" /> class.
        /// </summary>
        public RedmineNotAcceptableException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedmineNotAcceptableException" /> class.
        /// </summary>
        /// <param name="message"></param>
        public RedmineNotAcceptableException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedmineNotAcceptableException" /> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public RedmineNotAcceptableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}