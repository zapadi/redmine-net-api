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

using System.Net;
using Padi.RedmineApi.Extensions;

namespace Padi.RedmineApi.Authentication;

/// <summary>
/// 
/// </summary>
public sealed class RedmineApiKeyAuthentication: IRedmineAuthentication
{
    /// <inheritdoc />
    public string AuthenticationType { get; } = RedmineAuthenticationType.ApiKey.ToText();

    /// <inheritdoc />
    public string Token { get; init; }

    /// <inheritdoc />
    public ICredentials Credentials { get; init; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="apiKey"></param>
    public RedmineApiKeyAuthentication(string apiKey)
    {
        Token = apiKey;
    }
}