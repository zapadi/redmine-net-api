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

using System.Collections.Generic;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types;

/// <summary>
/// 
/// </summary>
public partial interface IRedmineManager
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="requestOptions"></param>
    /// <returns></returns>
    int Count<T>(RequestOptions requestOptions = null) 
        where T : class, new();
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestOptions"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T Get<T>(string id, RequestOptions requestOptions = null) 
        where T : class, new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestOptions"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    List<T> Get<T>(RequestOptions requestOptions = null) 
        where T : class, new();
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestOptions"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    PagedResults<T> GetPaginated<T>(RequestOptions requestOptions = null) 
        where T : class, new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="ownerId"></param>
    /// <param name="requestOptions"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T Create<T>(T entity, string ownerId = null,RequestOptions requestOptions = null) 
        where T : class, new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="entity"></param>
    /// <param name="projectId"></param>
    /// <param name="requestOptions"></param>
    /// <typeparam name="T"></typeparam>
    void Update<T>(string id, T entity, string projectId = null, RequestOptions requestOptions = null) 
        where T : class, new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="requestOptions"></param>
    /// <typeparam name="T"></typeparam>
    void Delete<T>(string id, RequestOptions requestOptions = null) 
        where T : class, new();

    /// <summary>
    ///     Support for adding attachments through the REST API is added in Redmine 1.4.0.
    ///     Upload a file to server.
    /// </summary>
    /// <param name="data">The content of the file that will be uploaded on server.</param>
    /// <param name="fileName"></param>
    /// <returns>
    ///     Returns the token for uploaded file.
    /// </returns>
    /// <exception cref="RedmineException"></exception>
    Upload UploadFile(byte[] data, string fileName = null);
        
    /// <summary>
    ///     Downloads a file from the specified address.
    /// </summary>
    /// <param name="address">The address.</param>
    /// <returns>The content of the downloaded file as a byte array.</returns>
    /// <exception cref="RedmineException"></exception>
    byte[] DownloadFile(string address);
}