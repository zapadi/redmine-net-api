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
using System.Collections.Specialized;
using System.Net;

namespace Redmine.Net.Api.Net;

/// <summary>
/// 
/// </summary>
internal sealed class ApiResponseMessage
{
    /// <summary>
    /// 
    /// </summary>
    public NameValueCollection Headers { get; init; }
    /// <summary>
    
    public HttpStatusCode? StatusCode { get; init; }
    public string Content { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public byte[] RawContent { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public int? StatusCode { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public string EndPoint { get; init; }
    
    /// <summary>
    /// 
    /// </summary>
    public bool IsSuccessful  { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public Exception Exception { get; set; }
}