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
using System.Text;
using Redmine.Net.Api.Common;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Http.Messages;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Http.Extensions;

internal static class RedmineApiResponseExtensions
{
    internal static T DeserializeTo<T>(this RedmineApiResponse responseMessage, IRedmineSerializer redmineSerializer) where T : new()
    {
        var responseAsString = GetResponseContentAsString(responseMessage);
        return responseAsString.IsNullOrWhiteSpace() ? default : redmineSerializer.Deserialize<T>(responseAsString);
    }
        
    internal static PagedResults<T> DeserializeToPagedResults<T>(this RedmineApiResponse responseMessage, IRedmineSerializer redmineSerializer) where T : class, new()
    {
        var responseAsString = GetResponseContentAsString(responseMessage);
        return responseAsString.IsNullOrWhiteSpace() ? default : redmineSerializer.DeserializeToPagedResults<T>(responseAsString);
    }
        
    internal static List<T> DeserializeToList<T>(this RedmineApiResponse responseMessage, IRedmineSerializer redmineSerializer) where T : class, new()
    {
        var responseAsString = GetResponseContentAsString(responseMessage);
        return responseAsString.IsNullOrWhiteSpace() ? null : redmineSerializer.Deserialize<List<T>>(responseAsString);
    }

    /// <summary>
    /// Gets the response content as a UTF-8 encoded string.
    /// </summary>
    /// <param name="responseMessage">The API response message.</param>
    /// <returns>The content as a string, or null if the response or content is null.</returns>
    private static string GetResponseContentAsString(RedmineApiResponse responseMessage)
    {
        return responseMessage?.Content == null ? null : Encoding.UTF8.GetString(responseMessage.Content);
    }
}