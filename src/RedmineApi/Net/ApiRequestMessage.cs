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
using System.Collections.Specialized;

namespace Padi.RedmineApi.Net;

/// <summary>
/// 
/// </summary>
public sealed class ApiRequestMessage
{
    /// <summary>
    /// 
    /// </summary>
    public ApiRequestContent Content { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string Method { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string RequestUri { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public NameValueCollection QueryString { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string ImpersonateUser { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string ContentType { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string UserAgent { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, string> Headers { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string RequestId { get; set; }
}