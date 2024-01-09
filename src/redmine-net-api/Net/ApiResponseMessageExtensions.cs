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

using System.Collections.Generic;
using System.Text;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Net;

internal static class ApiResponseMessageExtensions
{
    internal static T DeserializeTo<T>(this ApiResponseMessage responseMessage, IRedmineSerializer redmineSerializer) where T : new()
    {
        if (responseMessage?.Content == null)
        {
            return default;
        }
            
        var responseAsString = Encoding.UTF8.GetString(responseMessage.Content);
            
        return redmineSerializer.Deserialize<T>(responseAsString);
    }
        
    internal static PagedResults<T> DeserializeToPagedResults<T>(this ApiResponseMessage responseMessage, IRedmineSerializer redmineSerializer) where T : class, new()
    {
        if (responseMessage?.Content == null)
        {
            return default;
        }
            
        var responseAsString = Encoding.UTF8.GetString(responseMessage.Content);
            
        return redmineSerializer.DeserializeToPagedResults<T>(responseAsString);
    }
        
    internal static List<T> DeserializeToList<T>(this ApiResponseMessage responseMessage, IRedmineSerializer redmineSerializer) where T : class, new()
    {
        if (responseMessage?.Content == null)
        {
            return default;
        }
            
        var responseAsString = Encoding.UTF8.GetString(responseMessage.Content);
            
        return redmineSerializer.Deserialize<List<T>>(responseAsString);
    }
}