/*
   Copyright 2011 - 2016 Adrian Popescu.

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
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Extensions
{
    public static class WebExtensions
    {
        public static void HandleWebException(this WebException exception, string method, MimeFormat mimeFormat)
        {
            if (exception == null) return;

            switch (exception.Status)
            {
                case WebExceptionStatus.Timeout: throw new RedmineException("Timeout!", exception);
                case WebExceptionStatus.NameResolutionFailure: throw new RedmineException("Bad domain name!", exception);
                case WebExceptionStatus.ProtocolError:
                    {
                        var response = (HttpWebResponse)exception.Response;
                        switch ((int)response.StatusCode)
                        {
                            case (int)HttpStatusCode.InternalServerError:
                            case (int)HttpStatusCode.Unauthorized:
                            case (int)HttpStatusCode.NotFound:
                            case (int)HttpStatusCode.Forbidden:
								if (response.StatusDescription.Contains ("Not Found"))
									throw new NotFoundException (response.StatusDescription, exception);
						
                                throw new RedmineException(response.StatusDescription, exception);

                            case (int)HttpStatusCode.Conflict:
                                throw new RedmineException("The page that you are trying to update is staled!", exception);

                            case 422:
                                var errors = GetRedmineExceptions(exception.Response, mimeFormat);
                                string message = string.Empty;
                                if (errors != null)
                                {
                                    foreach (var error in errors)
                                        message = message + (error.Info + "\n");
                                }
                                throw new RedmineException(method + " has invalid or missing attribute parameters: " + message, exception);

                            case (int)HttpStatusCode.NotAcceptable: throw new RedmineException(response.StatusDescription, exception);
                        }
                    }
                    break;

                default: throw new RedmineException(exception.Message, exception);
            }
        }

        private static List<Error> GetRedmineExceptions(this WebResponse webResponse, MimeFormat mimeFormat)
        {
            using (var dataStream = webResponse.GetResponseStream())
            {
                if (dataStream == null) return null;
                using (var reader = new StreamReader(dataStream))
                {
                    var responseFromServer = reader.ReadToEnd();

                    if (string.IsNullOrEmpty(responseFromServer.Trim())) return null;
                    try
                    {
                        var result = RedmineSerializer.DeserializeList<Error>(responseFromServer, mimeFormat);
                        return result.Objects;
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.Message);
                    }
                }
                return null;
            }
        }
    }
}