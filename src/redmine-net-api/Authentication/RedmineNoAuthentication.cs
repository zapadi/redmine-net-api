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
/// Represents no authentication for accessing public Redmine resources that don't require authentication.
/// </summary>
/// <remarks>
/// <para>
/// This authentication type is used internally when no authentication is explicitly configured.
/// It allows access to public Redmine resources that don't require user authentication.
/// </para>
/// <para>
/// Most Redmine API endpoints require authentication, so this is typically only useful
/// for accessing publicly visible projects or resources.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // This is used internally when no authentication is provided
/// var options = new RedmineManagerOptionsBuilder()
///     .WithHost("https://redmine.example.com");
/// // No authentication method called - RedmineNoAuthentication is used by default
/// 
/// var manager = new RedmineManager(options);
/// // Can only access public resources
/// </code>
/// </example>
public sealed class RedmineNoAuthentication: IRedmineAuthentication
{
    /// <inheritdoc />
    public string AuthenticationType => "NoAuth";

    /// <inheritdoc />
    public string Token { get; init; }

    /// <inheritdoc />
    public ICredentials Credentials { get; init; }
}