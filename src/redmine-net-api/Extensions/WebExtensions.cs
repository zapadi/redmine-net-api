/*
   Copyright 2011 - 2022 Adrian Popescu

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
using System.IO;
using System.Net;
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
        /// <exception cref="RedmineTimeoutException">Timeout!</exception>
        /// <exception cref="NameResolutionFailureException">Bad domain name!</exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="InternalServerErrorException"></exception>
        /// <exception cref="UnauthorizedException"></exception>
        /// <exception cref="ForbiddenException"></exception>
        /// <exception cref="ConflictException">The page that you are trying to update is staled!</exception>
        /// <exception cref="RedmineException">
        /// </exception>
        /// <exception cref="NotAcceptableException"></exception>
        public static void HandleWebException(this WebException exception, IRedmineSerializer serializer)
        {
            if (exception == null)
            {
                return;
            }

            var innerException = exception.InnerException ?? exception;

            switch (exception.Status)
            {
                case WebExceptionStatus.Timeout:
                    throw new RedmineTimeoutException(nameof(WebExceptionStatus.Timeout), innerException);
                case WebExceptionStatus.NameResolutionFailure:
                    throw new NameResolutionFailureException("Bad domain name.", innerException);
                
                case WebExceptionStatus.ProtocolError:
                    {
                        var response = (HttpWebResponse)exception.Response;
                        switch ((int)response.StatusCode)
                        {
                            case (int)HttpStatusCode.NotFound:
                                throw new NotFoundException(response.StatusDescription, innerException);

                            case (int)HttpStatusCode.Unauthorized:
                                throw new UnauthorizedException(response.StatusDescription, innerException);

                            case (int)HttpStatusCode.Forbidden:
                                throw new ForbiddenException(response.StatusDescription, innerException);

                            case (int)HttpStatusCode.Conflict:
                                throw new ConflictException("The page that you are trying to update is staled!", innerException);

                            case 422:
                                var errors = GetRedmineExceptions(exception.Response, serializer);
                                var message = string.Empty;
                                
                                if (errors == null) 
                                    throw new RedmineException($"Invalid or missing attribute parameters: {message}", innerException);
                                
                                foreach (var error in errors)
                                {
                                    message = message + error.Info + Environment.NewLine;
                                }
                                
                                throw new RedmineException("Invalid or missing attribute parameters: " + message, innerException);

                            case (int)HttpStatusCode.NotAcceptable:
                                throw new NotAcceptableException(response.StatusDescription, innerException);

                            default:
                                throw new RedmineException(response.StatusDescription, innerException);
                        }
                    }
                    
                default:
                    throw new RedmineException(exception.Message, innerException);
            }
        }

        /// <summary>
        /// Gets the redmine exceptions.
        /// </summary>
        /// <param name="webResponse">The web response.</param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        private static IEnumerable<Error> GetRedmineExceptions(this WebResponse webResponse, IRedmineSerializer serializer)
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