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

#if !(NET20)
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRedmineManagerAsync
    {
        /// <summary>
        /// Returns the count of items asynchronously for a given type T.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="requestOptions">Optional request options.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The count of items as an integer.</returns>
        Task<int> CountAsync<T>(RequestOptions requestOptions, CancellationToken cancellationToken = default) 
            where T : class, new();
        
        /// <summary>
        ///     Gets the paginated objects asynchronous.
        /// </summary>
        /// <typeparam name="T">The type of the results.</typeparam>
        /// <param name="requestOptions">Optional request options.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A task representing the asynchronous operation that returns the paged results.</returns>
        Task<PagedResults<T>> GetPagedAsync<T>(RequestOptions requestOptions = null, CancellationToken cancellationToken = default) 
            where T : class, new();

        /// <summary>
        ///     Gets the objects asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<T>> GetAsync<T>(RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
            where T : class, new();

        /// <summary>
        ///     Gets a Redmine object asynchronous.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="id">The ID of the object to retrieve.</param>
        /// <param name="requestOptions">Optional request options.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The retrieved object of type T.</returns>
        Task<T> GetAsync<T>(string id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
            where T : class, new();

        /// <summary>
        ///     Creates a new Redmine object asynchronous.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity to create.</param>
        /// <param name="requestOptions">The optional request options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task representing the asynchronous operation, returning the created entity.</returns>
        /// <remarks>
        /// This method creates an entity of type T asynchronously. It accepts an entity object, along with optional request options and cancellation token. 
        /// The method is generic and constrained to accept only classes that have a default constructor. 
        /// It uses the CreateAsync method to create the entity, passing the entity, request options, and cancellation token as arguments. 
        /// The method is awaited and returns a Task of type T representing the asynchronous operation.
        /// </remarks>
        Task<T> CreateAsync<T>(T entity, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
            where T : class, new();

        /// <summary>
        ///     Creates a new Redmine object. This method does not block the calling thread.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The entity object to create.</param>
        /// <param name="ownerId">The ID of the owner.</param>
        /// <param name="requestOptions">Optional request options.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The created entity.</returns>
        /// <exception cref="System.Exception">Thrown when an error occurs during the creation process.</exception>
        Task<T> CreateAsync<T>(T entity, string ownerId, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
            where T : class, new();

        /// <summary>
        ///     Updates the object asynchronous.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="id">The ID of the entity to update.</param>
        /// <param name="entity">The entity to update.</param>
        /// <param name="requestOptions">Optional request options.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A task representing the asynchronous update operation.</returns>
        /// <remarks>
        /// This method sends an update request to the Redmine API to update the entity with the specified ID.
        /// </remarks>
        Task UpdateAsync<T>(string id, T entity, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
            where T : class, new();

        /// <summary>
        ///     Deletes the Redmine object asynchronous.
        /// </summary>
        /// <typeparam name="T">The type of the resource to delete.</typeparam>
        /// <param name="id">The ID of the resource to delete.</param>
        /// <param name="requestOptions">Optional request options.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous delete operation.</returns>
        /// <remarks>
        /// This method sends a DELETE request to the Redmine API to delete a resource identified by the given ID.
        /// </remarks>
        Task DeleteAsync<T>(string id, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
            where T : class, new();

        /// <summary>
        ///     Support for adding attachments through the REST API is added in Redmine 1.4.0.
        ///     Upload a file to server. This method does not block the calling thread.
        /// </summary>
        /// <param name="data">The content of the file that will be uploaded on server.</param>
        /// <param name="fileName"></param>
        /// <param name="requestOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     .
        /// </returns>
        Task<Upload> UploadFileAsync(byte[] data, string fileName = null, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);

        /// <summary>
        ///     Downloads the file asynchronous.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="requestOptions"></param>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<byte[]> DownloadFileAsync(string address, RequestOptions requestOptions = null, IProgress<int> progress = null, CancellationToken cancellationToken = default);
    }
}
#endif