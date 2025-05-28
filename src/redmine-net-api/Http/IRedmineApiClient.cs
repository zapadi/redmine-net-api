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
using Redmine.Net.Api.Http.Messages;
#if !NET20
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Redmine.Net.Api.Http;
/// <summary>
/// 
/// </summary>
internal interface IRedmineApiClient : ISyncRedmineApiClient
#if !NET20
    , IAsyncRedmineApiClient
#endif
{
}

internal interface ISyncRedmineApiClient
{
    RedmineApiResponse Get(string address, RequestOptions requestOptions = null);

    RedmineApiResponse GetPaged(string address, RequestOptions requestOptions = null);

    RedmineApiResponse Create(string address, string payload, RequestOptions requestOptions = null);

    RedmineApiResponse Update(string address, string payload, RequestOptions requestOptions = null);

    RedmineApiResponse Patch(string address, string payload, RequestOptions requestOptions = null);

    RedmineApiResponse Delete(string address, RequestOptions requestOptions = null);

    RedmineApiResponse Upload(string address, byte[] data, RequestOptions requestOptions = null);

    RedmineApiResponse Download(string address, RequestOptions requestOptions = null, IProgress<int> progress = null);
}

#if !NET20
internal interface IAsyncRedmineApiClient
{
    Task<RedmineApiResponse> GetAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    
    Task<RedmineApiResponse> GetPagedAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    
    Task<RedmineApiResponse> CreateAsync(string address, string payload, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    
    Task<RedmineApiResponse> UpdateAsync(string address, string payload, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    
    Task<RedmineApiResponse> PatchAsync(string address, string payload, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    
    Task<RedmineApiResponse> DeleteAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    
    Task<RedmineApiResponse> UploadFileAsync(string address, byte[] data, RequestOptions requestOptions = null, CancellationToken cancellationToken = default);
    
    Task<RedmineApiResponse> DownloadAsync(string address, RequestOptions requestOptions = null, IProgress<int> progress = null, CancellationToken cancellationToken = default);
}
#endif