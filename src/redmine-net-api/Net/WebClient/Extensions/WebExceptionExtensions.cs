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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class WebExceptionExtensions
    {
        /// <summary>
        /// Handles the web exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="serializer"></param>
        public static void HandleApiRequestException(this WebException exception, IRedmineSerializer serializer)
        {
            if (exception == null)
            {
                return;
            }

            var innerException = exception.InnerException ?? exception;

            switch (exception.Status)
            {
                case WebExceptionStatus.ProtocolError:
                    {
                        var response = (HttpWebResponse)exception.Response;
                        Debug.Assert(response != null, $"{nameof(response)} != null");
                        switch ((int)response.StatusCode)
                        {
                            case (int)HttpStatusCode.Conflict:
                                throw new RedmineException("The page that you are trying to update is staled!", innerException);

                            case 422:
                                RedmineException redmineException;
                                var errors = GetRedmineExceptions(exception.Response, serializer);

                                if (errors != null)
                                {
                                    var sb = new StringBuilder();
                                    foreach (var error in errors)
                                    {
                                        sb.Append(error.Info).Append(Environment.NewLine);
                                    }
                                    
                                    redmineException = new RedmineException($"Invalid or missing attribute parameters: {sb}", innerException, "Unprocessable Content");
                                    sb.Length = 0;
                                }
                                else
                                {
                                    redmineException = new RedmineException("Invalid or missing attribute parameters", innerException);
                                }
                               
                                throw redmineException;

                            default:
                                throw new RedmineException(response.StatusDescription, innerException);
                        }
                    }
                    
                default:
                    throw new RedmineException(innerException.Message, innerException);
            }
        }

        /// <summary>
        /// Gets the redmine exceptions.
        /// </summary>
        /// <param name="webResponse">The web response.</param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        private static IEnumerable<Error> GetRedmineExceptions(WebResponse webResponse, IRedmineSerializer serializer)
        {
            using (var responseStream = webResponse.GetResponseStream())
            {
                if (responseStream == null)
                {
                    return null;
                }

                using (var streamReader = new StreamReader(responseStream))
                {
                    var responseContent = streamReader.ReadToEnd();

                    if (responseContent.IsNullOrWhiteSpace())
                    {
                        return null;
                    }

                    var result = serializer.DeserializeToPagedResults<Error>(responseContent);
                    return result.Items;
                }
            }
        }
    }
}