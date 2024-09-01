#if NET40_OR_GREATER || NETCOREAPP
/*
   Copyright 2011 - 2024 Adrian Popescu

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
using System.Threading.Tasks;

namespace Redmine.Net.Api.Net.HttpClient;

internal sealed partial class InternalRedmineApiHttpClient
{
    public Task<ApiResponseMessage> GetAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<ApiResponseMessage> GetPagedAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<ApiResponseMessage> CreateAsync(string address, string payload, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<ApiResponseMessage> UpdateAsync(string address, string payload, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<ApiResponseMessage> PatchAsync(string address, string payload, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<ApiResponseMessage> DeleteAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<ApiResponseMessage> UploadFileAsync(string address, byte[] data, RequestOptions requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<ApiResponseMessage> DownloadAsync(string address, RequestOptions requestOptions = null, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}
#endif