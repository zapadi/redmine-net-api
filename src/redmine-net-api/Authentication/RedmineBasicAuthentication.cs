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
using System.Net;
using System.Text;
using Redmine.Net.Api.Exceptions;

namespace Redmine.Net.Api.Authentication
{
    /// <summary>
    /// Provides HTTP Basic authentication for Redmine API requests using username and password.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Basic authentication sends credentials as a Base64-encoded string in the <c>Authorization</c> HTTP header.
    /// While supported by Redmine, API key authentication is generally preferred for security reasons.
    /// </para>
    /// <para>
    /// The credentials are encoded as "username:password" and sent with each request.
    /// </para>
    /// <para>
    /// <strong>Security Warning:</strong> Always use HTTPS when using basic authentication to prevent
    /// credentials from being transmitted in cleartext over the network.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create basic authentication
    /// var auth = new RedmineBasicAuthentication("john.doe", "mySecurePassword");
    /// 
    /// // Use with RedmineManagerOptionsBuilder
    /// var options = new RedmineManagerOptionsBuilder()
    ///     .WithHost("https://redmine.example.com")
    ///     .WithBasicAuthentication("john.doe", "mySecurePassword");
    /// 
    /// var manager = new RedmineManager(options);
    /// var projects = await manager.GetAsync&lt;Project&gt;();
    /// </code>
    /// </example>
    public sealed class RedmineBasicAuthentication: IRedmineAuthentication
    {
        /// <inheritdoc />
        public string AuthenticationType => "Basic";

        /// <inheritdoc />
        public string Token { get; init; }

        /// <inheritdoc />
        public ICredentials Credentials { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedmineBasicAuthentication"/> class.
        /// </summary>
        /// <param name="username">The Redmine username for authentication.</param>
        /// <param name="password">The password associated with the username.</param>
        /// <exception cref="RedmineException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// <para>
        /// The username and password are combined and Base64-encoded to create the authentication token.
        /// </para>
        /// <para>
        /// <strong>Security Best Practices:</strong>
        /// <list type="bullet">
        /// <item><description>Never hardcode credentials in source code</description></item>
        /// <item><description>Use secure credential storage (e.g., Azure Key Vault, AWS Secrets Manager)</description></item>
        /// <item><description>Always use HTTPS to protect credentials in transit</description></item>
        /// <item><description>Consider using API key authentication instead for better security</description></item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Read credentials from secure storage (recommended)
        /// var username = configuration["Redmine:Username"];
        /// var password = configuration["Redmine:Password"];
        /// var auth = new RedmineBasicAuthentication(username, password);
        /// </code>
        /// </example>
        public RedmineBasicAuthentication(string username, string password)
        {
            if (username == null) throw new RedmineException(nameof(username));
            if (password == null) throw new RedmineException(nameof(password));
            
            Token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        }
    }
}