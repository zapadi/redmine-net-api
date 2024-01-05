/*
   Copyright 2011 - 2023 Adrian Popescu

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
using System.Net;
using System.Text;
using Redmine.Net.Api.Exceptions;

namespace Redmine.Net.Api.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RedmineBasicAuthentication: IRedmineAuthentication
    {
        /// <inheritdoc />
        public string AuthenticationType => "Basic";

        /// <inheritdoc />
        public string Token { get; init; }

        /// <inheritdoc />
        public ICredentials Credentials { get; init; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public RedmineBasicAuthentication(string username, string password)
        {
            if (username == null) throw new RedmineException(nameof(username));
            if (password == null) throw new RedmineException(nameof(password));
            
            Token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        }
    }
}