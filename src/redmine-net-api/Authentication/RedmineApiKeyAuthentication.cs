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
/// Provides API key-based authentication for Redmine API requests.
/// </summary>
/// <remarks>
/// <para>
/// API key authentication is the recommended method for authenticating with Redmine.
/// Each user can generate their own API key from their account settings in Redmine.
/// </para>
/// <para>
/// The API key is sent in the <c>X-Redmine-API-Key</c> HTTP header with each request.
/// </para>
/// <para>
/// To find your API key in Redmine: Login → My Account → API access key (on the right sidebar).
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Create API key authentication
/// var auth = new RedmineApiKeyAuthentication("a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6q7r8s9t0");
/// 
/// // Use with RedmineManagerOptionsBuilder
/// var options = new RedmineManagerOptionsBuilder()
///     .WithHost("https://redmine.example.com")
///     .WithApiKeyAuthentication("a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6q7r8s9t0");
/// 
/// var manager = new RedmineManager(options);
/// var issues = await manager.GetAsync&lt;Issue&gt;();
/// </code>
/// </example>
public sealed class RedmineApiKeyAuthentication: IRedmineAuthentication
{
    /// <inheritdoc />
    public string AuthenticationType => "X-Redmine-API-Key";

    /// <inheritdoc />
    public string Token { get; init; }

    /// <inheritdoc />
    public ICredentials Credentials { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedmineApiKeyAuthentication"/> class.
    /// </summary>
    /// <param name="apiKey">The Redmine API key obtained from the user's account settings.</param>
    /// <remarks>
    /// <para>
    /// The API key is typically a 40-character hexadecimal string.
    /// </para>
    /// <para>
    /// <strong>Security Note:</strong> Never hardcode API keys in your source code. 
    /// Use environment variables, configuration files, or secure credential storage instead.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Read API key from environment variable (recommended)
    /// var apiKey = Environment.GetEnvironmentVariable("REDMINE_API_KEY");
    /// var auth = new RedmineApiKeyAuthentication(apiKey);
    /// 
    /// // Or from configuration
    /// var apiKey = configuration["Redmine:ApiKey"];
    /// var auth = new RedmineApiKeyAuthentication(apiKey);
    /// </code>
    /// </example>
    public RedmineApiKeyAuthentication(string apiKey)
    {
        Token = apiKey;
    }
}