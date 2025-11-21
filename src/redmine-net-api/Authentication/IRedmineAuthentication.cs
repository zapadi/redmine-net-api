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

namespace Redmine.Net.Api.Authentication;

/// <summary>
/// Defines the contract for authentication mechanisms used to connect to a Redmine server.
/// </summary>
/// <remarks>
/// <para>
/// Redmine supports multiple authentication methods including API key authentication and HTTP Basic authentication.
/// This interface provides a unified way to handle different authentication types.
/// </para>
/// <para>
/// Implementations of this interface are used by <see cref="RedmineManager"/> to authenticate API requests.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Using API key authentication
/// IRedmineAuthentication auth = new RedmineApiKeyAuthentication("your-api-key-here");
/// 
/// // Using basic authentication
/// IRedmineAuthentication auth = new RedmineBasicAuthentication("username", "password");
/// 
/// // Configure RedmineManager with authentication
/// var options = new RedmineManagerOptionsBuilder()
///     .WithHost("https://redmine.example.com")
///     .WithApiKeyAuthentication("your-api-key");
/// var manager = new RedmineManager(options);
/// </code>
/// </example>
public interface IRedmineAuthentication
{
    /// <summary>
    /// Gets the authentication type identifier used in HTTP headers.
    /// </summary>
    /// <value>
    /// Returns "X-Redmine-API-Key" for API key authentication, or "Basic" for HTTP Basic authentication.
    /// </value>
    /// <remarks>
    /// This value determines how the authentication token is sent to the Redmine server.
    /// </remarks>
    string AuthenticationType { get; }

    /// <summary>
    /// Gets the authentication token or encoded credentials.
    /// </summary>
    /// <value>
    /// For API key authentication, this is the raw API key.
    /// For basic authentication, this is the Base64-encoded username:password string.
    /// </value>
    /// <remarks>
    /// The token format depends on the <see cref="AuthenticationType"/>. This value is included
    /// in HTTP headers when making requests to the Redmine API.
    /// </remarks>
    string Token { get; }

    /// <summary>
    /// Gets the network credentials for authentication.
    /// </summary>
    /// <value>
    /// The credentials object, or <c>null</c> if not applicable for the authentication type.
    /// </value>
    /// <remarks>
    /// This property is primarily used for HTTP Basic authentication scenarios where
    /// <see cref="ICredentials"/> is required by the underlying HTTP client.
    /// </remarks>
    ICredentials Credentials { get; }
}