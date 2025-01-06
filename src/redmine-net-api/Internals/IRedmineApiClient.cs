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

using System.Threading;
#if!(NET20)
using System.Threading.Tasks;
#endif

namespace Redmine.Net.Api.Net;

/// <summary>
/// 
/// </summary>
internal interface IRedmineApiClient
{
    ApiResponseMessage Get(string address, RequestOptions requestOptions = null);
    ApiResponseMessage GetPaged(string address, RequestOptions requestOptions = null);
    ApiResponseMessage Create(string address, string payload, RequestOptions requestOptions = null);
    ApiResponseMessage Update(string address, string payload, RequestOptions requestOptions = null);
    ApiResponseMessage Patch(string address, string payload, RequestOptions requestOptions = null);
    ApiResponseMessage Delete(string address, RequestOptions requestOptions = null);
    ApiResponseMessage Upload(string address, byte[] data, RequestOptions requestOptions = null);
    ApiResponseMessage Download(string address, RequestOptions requestOptions = null);
    
    #if !(NET20)
    Task<ApiResponseMessage> GetAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    Task<ApiResponseMessage> GetPagedAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    Task<ApiResponseMessage> CreateAsync(string address, string payload, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    Task<ApiResponseMessage> UpdateAsync(string address, string payload, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    Task<ApiResponseMessage> PatchAsync(string address, string payload, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    Task<ApiResponseMessage> DeleteAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    Task<ApiResponseMessage> UploadFileAsync(string address, byte[] data, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    Task<ApiResponseMessage> DownloadAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    #endif
}