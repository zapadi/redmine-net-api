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
using System.Collections.Generic;
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Net;
using Padi.RedmineApi.Serialization;
using Padi.RedmineApi.Types;

namespace Padi.RedmineApi.Internals;

internal static class ApiResponseMessageExtensions
{
    internal static T DeserializeTo<T>(this ApiResponseMessage responseMessage, IRedmineSerializer redmineSerializer) where T : new()
    {
        var responseAsString = GetResponseContentAsString(responseMessage, redmineSerializer);
        return responseAsString.IsNullOrWhiteSpace() ? default : redmineSerializer.Deserialize<T>(responseAsString);
    }
        
    internal static PagedResults<T> DeserializeToPagedResults<T>(this ApiResponseMessage responseMessage, IRedmineSerializer redmineSerializer) where T : class, new()
    {
        var responseAsString = GetResponseContentAsString(responseMessage, redmineSerializer);
        return responseAsString.IsNullOrWhiteSpace() ? null : redmineSerializer.DeserializeToPagedResults<T>(responseAsString);
    }
        
    internal static List<T> DeserializeToList<T>(this ApiResponseMessage responseMessage, IRedmineSerializer redmineSerializer) where T : class, new()
    {
        var responseAsString = GetResponseContentAsString(responseMessage, redmineSerializer);
        return responseAsString.IsNullOrWhiteSpace() ? null : redmineSerializer.Deserialize<List<T>>(responseAsString);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="redmineSerializer"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="RedmineException"></exception>
    private static string GetResponseContentAsString(ApiResponseMessage responseMessage, IRedmineSerializer redmineSerializer)
    {
        if (responseMessage == null)
        {
            return string.Empty;
        }

        if (!responseMessage.IsSuccessful)
        {
            if (responseMessage.Exception is not null)
            {
                throw responseMessage.Exception;
            }

            if (responseMessage.StatusCode == HttpConstants.StatusCodes.UnprocessableEntity)
            {
                throw RedmineApiExceptionHelper.CreateUnprocessableEntityException(responseMessage.Content, null, redmineSerializer);
            }
        }

        return responseMessage.Content;
    }
}