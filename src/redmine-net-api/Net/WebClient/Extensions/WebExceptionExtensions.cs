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

using System.Net;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Net.Internal;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Net.WebClient.Extensions
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
        /// <exception cref="RedmineException"> </exception>
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
                    if (exception.Response != null)
                    {
                        using var responseStream = exception.Response.GetResponseStream();
                        HttpStatusHelper.MapStatusCodeToException((int)exception.Status, responseStream, innerException, serializer);
                    }

                    break;
            }
            throw new RedmineException(exception.Message, innerException);
        }
    }
}