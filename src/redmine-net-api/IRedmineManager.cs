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
using System.Collections.Generic;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types;

/// <summary>
/// Defines the contract for synchronous Redmine API operations.
/// </summary>
/// <remarks>
/// <para>
/// This interface provides synchronous methods for performing CRUD (Create, Read, Update, Delete) operations
/// on Redmine resources such as issues, projects, users, time entries, and more.
/// </para>
/// <para>
/// For asynchronous operations, use <see cref="IRedmineManagerAsync"/>.
/// The <see cref="RedmineManager"/> class implements both interfaces.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Initialize the manager
/// var options = new RedmineManagerOptionsBuilder()
///     .WithHost("https://redmine.example.com")
///     .WithApiKeyAuthentication("your-api-key");
/// 
/// IRedmineManager manager = new RedmineManager(options);
/// 
/// // Get a single issue
/// var issue = manager.Get&lt;Issue&gt;("12345");
/// 
/// // Get all projects
/// var projects = manager.Get&lt;Project&gt;();
/// 
/// // Create a new issue
/// var newIssue = new Issue { Subject = "Bug report", ProjectId = 1 };
/// var created = manager.Create(newIssue);
/// </code>
/// </example>
public partial interface IRedmineManager
{
    /// <summary>
    /// Gets the total count of resources of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of Redmine resource (e.g., <see cref="Issue"/>, <see cref="Project"/>).</typeparam>
    /// <param name="requestOptions">Optional request options for filtering or custom headers.</param>
    /// <returns>The total number of resources matching the specified criteria.</returns>
    /// <exception cref="UnauthorizedException">Thrown when authentication fails.</exception>
    /// <exception cref="ForbiddenException">Thrown when the user lacks permission.</exception>
    /// <exception cref="RedmineException">Thrown when a general error occurs.</exception>
    int Count<T>(RequestOptions requestOptions = null) 
        where T : class, new();
        
    /// <summary>
    /// Retrieves a single resource by its identifier.
    /// </summary>
    /// <typeparam name="T">The type of Redmine resource to retrieve.</typeparam>
    /// <param name="id">The unique identifier of the resource.</param>
    /// <param name="requestOptions">Optional request options for including associated data.</param>
    /// <returns>The requested resource.</returns>
    /// <exception cref="NotFoundException">Thrown when the resource doesn't exist.</exception>
    /// <exception cref="UnauthorizedException">Thrown when authentication fails.</exception>
    /// <exception cref="ForbiddenException">Thrown when the user lacks permission.</exception>
    /// <exception cref="RedmineException">Thrown when a general error occurs.</exception>
    T Get<T>(string id, RequestOptions requestOptions = null) 
        where T : class, new();

    /// <summary>
    /// Retrieves all resources of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of Redmine resource to retrieve.</typeparam>
    /// <param name="requestOptions">Optional request options for filtering or sorting.</param>
    /// <returns>A list of all resources matching the specified criteria.</returns>
    /// <exception cref="UnauthorizedException">Thrown when authentication fails.</exception>
    /// <exception cref="ForbiddenException">Thrown when the user lacks permission.</exception>
    /// <exception cref="RedmineException">Thrown when a general error occurs.</exception>
    /// <remarks>
    /// This method automatically handles pagination. For large result sets, consider using <see cref="GetPaginated{T}"/>.
    /// </remarks>
    List<T> Get<T>(RequestOptions requestOptions = null) 
        where T : class, new();
        
    /// <summary>
    /// Retrieves a paginated result set of resources.
    /// </summary>
    /// <typeparam name="T">The type of Redmine resource to retrieve.</typeparam>
    /// <param name="requestOptions">Optional request options for filtering or page parameters.</param>
    /// <returns>A <see cref="PagedResults{T}"/> object containing the current page and metadata.</returns>
    /// <exception cref="UnauthorizedException">Thrown when authentication fails.</exception>
    /// <exception cref="ForbiddenException">Thrown when the user lacks permission.</exception>
    /// <exception cref="RedmineException">Thrown when a general error occurs.</exception>
    PagedResults<T> GetPaginated<T>(RequestOptions requestOptions = null) 
        where T : class, new();

    /// <summary>
    /// Creates a new resource on the Redmine server.
    /// </summary>
    /// <typeparam name="T">The type of Redmine resource to create.</typeparam>
    /// <param name="entity">The resource object to create.</param>
    /// <param name="ownerId">Optional owner/parent identifier.</param>
    /// <param name="requestOptions">Optional request options.</param>
    /// <returns>The created resource with server-assigned properties.</returns>
    /// <exception cref="UnauthorizedException">Thrown when authentication fails.</exception>
    /// <exception cref="ForbiddenException">Thrown when the user lacks permission.</exception>
    /// <exception cref="RedmineApiException">Thrown when validation fails (HTTP 422).</exception>
    /// <exception cref="RedmineException">Thrown when a general error occurs.</exception>
    T Create<T>(T entity, string ownerId = null,RequestOptions requestOptions = null) 
        where T : class, new();

    /// <summary>
    /// Updates an existing resource on the Redmine server.
    /// </summary>
    /// <typeparam name="T">The type of Redmine resource to update.</typeparam>
    /// <param name="id">The unique identifier of the resource to update.</param>
    /// <param name="entity">The resource object with updated values.</param>
    /// <param name="projectId">Optional project identifier (required for some resources).</param>
    /// <param name="requestOptions">Optional request options.</param>
    /// <exception cref="NotFoundException">Thrown when the resource doesn't exist.</exception>
    /// <exception cref="UnauthorizedException">Thrown when authentication fails.</exception>
    /// <exception cref="ForbiddenException">Thrown when the user lacks permission.</exception>
    /// <exception cref="RedmineApiException">Thrown when validation fails or a conflict occurs.</exception>
    /// <exception cref="RedmineException">Thrown when a general error occurs.</exception>
    void Update<T>(string id, T entity, string projectId = null, RequestOptions requestOptions = null) 
        where T : class, new();

    /// <summary>
    /// Deletes a resource from the Redmine server.
    /// </summary>
    /// <typeparam name="T">The type of Redmine resource to delete.</typeparam>
    /// <param name="id">The unique identifier of the resource to delete.</param>
    /// <param name="requestOptions">Optional request options.</param>
    /// <exception cref="NotFoundException">Thrown when the resource doesn't exist.</exception>
    /// <exception cref="UnauthorizedException">Thrown when authentication fails.</exception>
    /// <exception cref="ForbiddenException">Thrown when the user lacks permission.</exception>
    /// <exception cref="RedmineException">Thrown when a general error occurs.</exception>
    /// <remarks>
    /// <strong>Warning:</strong> This operation is typically irreversible.
    /// </remarks>
    void Delete<T>(string id, RequestOptions requestOptions = null) 
        where T : class, new();

    /// <summary>
    /// Uploads a file to the Redmine server.
    /// </summary>
    /// <param name="data">The file content as a byte array.</param>
    /// <param name="fileName">Optional file name.</param>
    /// <returns>An <see cref="Upload"/> object containing the upload token.</returns>
    /// <exception cref="RedmineException">Thrown when the upload fails.</exception>
    /// <remarks>
    /// <para>
    /// Support for file uploads was added in Redmine 1.4.0.
    /// The returned upload token can be used when creating or updating resources with attachments.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Upload a file
    /// byte[] fileData = File.ReadAllBytes("screenshot.png");
    /// var upload = manager.UploadFile(fileData, "screenshot.png");
    /// 
    /// // Use the token when creating an issue
    /// var issue = new Issue
    /// {
    ///     Subject = "Bug with screenshot",
    ///     Uploads = new List&lt;Upload&gt; { upload }
    /// };
    /// manager.Create(issue);
    /// </code>
    /// </example>
    Upload UploadFile(byte[] data, string fileName = null);

    /// <summary>
    /// Downloads a file from the Redmine server.
    /// </summary>
    /// <param name="address">The file URL or path.</param>
    /// <param name="requestOptions">Optional request options.</param>
    /// <param name="progress">Optional progress reporter for tracking download progress.</param>
    /// <returns>The file content as a byte array.</returns>
    /// <exception cref="RedmineException">Thrown when the download fails.</exception>
    /// <example>
    /// <code>
    /// // Download an attachment
    /// var issue = manager.Get&lt;Issue&gt;("12345");
    /// if (issue.Attachments?.Count > 0)
    /// {
    ///     var attachment = issue.Attachments[0];
    ///     byte[] fileData = manager.DownloadFile(attachment.ContentUrl);
    ///     File.WriteAllBytes(attachment.FileName, fileData);
    /// }
    /// </code>
    /// </example>
    byte[] DownloadFile(string address,RequestOptions requestOptions = null, IProgress<int> progress = null);
}