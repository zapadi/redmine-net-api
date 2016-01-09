using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api
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
                            throw new RedmineException(response.StatusDescription, exception);

                        case (int)HttpStatusCode.Conflict:
                            throw new RedmineException("The page that you are trying to update is staled!", exception);

                        case 422:
                            var errors = GetRedmineExceptions(exception.Response, mimeFormat);
                            string message = string.Empty;
                            if (errors != null)
                            {
                                message = errors.Aggregate(message, (current, error) => current + (error.Info + "\n"));
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

                    if (responseFromServer.Trim().Length > 0)
                    {
                        try
                        {
                            return RedmineSerializer.Deserialize<List<Error>>(responseFromServer, mimeFormat);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError(ex.Message);
                        }
                    }
                }
                return null;
            }
        }
    }
}