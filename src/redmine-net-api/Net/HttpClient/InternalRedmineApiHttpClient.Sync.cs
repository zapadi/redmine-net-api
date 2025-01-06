#if NET45_OR_GREATER || NETCOREAPP
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

using System;
using Redmine.Net.Api.Authentication;
using Redmine.Net.Api.Http;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Net.HttpClient;

internal sealed partial class InternalRedmineApiHttpClient: IRedmineApiClient
{
    public ApiResponseMessage Get(string address, RequestOptions requestOptions = null)
    {
        throw new System.NotImplementedException();
    }

    public ApiResponseMessage GetPaged(string address, RequestOptions requestOptions = null)
    {
        throw new System.NotImplementedException();
    }

    public ApiResponseMessage Create(string address, string payload, RequestOptions requestOptions = null)
    {
        throw new System.NotImplementedException();
    }

    public ApiResponseMessage Update(string address, string payload, RequestOptions requestOptions = null)
    {
        throw new System.NotImplementedException();
    }

    public ApiResponseMessage Patch(string address, string payload, RequestOptions requestOptions = null)
    {
        throw new System.NotImplementedException();
    }

    public ApiResponseMessage Delete(string address, RequestOptions requestOptions = null)
    {
        throw new System.NotImplementedException();
    }

    public ApiResponseMessage Upload(string address, byte[] data, RequestOptions requestOptions = null)
    {
        throw new System.NotImplementedException();
    }

    public ApiResponseMessage Download(string address, RequestOptions requestOptions = null)
    {
        throw new System.NotImplementedException();
    }
}
#endif