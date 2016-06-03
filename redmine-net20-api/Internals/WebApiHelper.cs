using System.Collections.Specialized;
using System.Net;
using System.Text;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Internals
{
    internal static class WebApiHelper
    {
        public static void ExecuteUpload(RedmineManager redmineManager, string address, string actionType, string data,
            string methodName)
        {
            using (var wc = redmineManager.CreateWebClient(null))
            {
                try
                {
                    if (actionType == HttpVerbs.POST || actionType == HttpVerbs.DELETE || actionType == HttpVerbs.PUT ||
                        actionType == HttpVerbs.PATCH)
                    {
                        wc.UploadString(address, actionType, data);
                    }
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, redmineManager.MimeFormat);
                }
            }
        }

        public static T ExecuteUpload<T>(RedmineManager redmineManager, string address, string actionType, string data,
            string methodName)
            where T : class, new()
        {
            using (var wc = redmineManager.CreateWebClient(null))
            {
                try
                {
                    if (actionType == HttpVerbs.POST || actionType == HttpVerbs.DELETE || actionType == HttpVerbs.PUT ||
                        actionType == HttpVerbs.PATCH)
                    {
                        var response = wc.UploadString(address, actionType, data);
                        return RedmineSerializer.Deserialize<T>(response, redmineManager.MimeFormat);
                    }
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, redmineManager.MimeFormat);
                }
                return default(T);
            }
        }

        public static T ExecuteDownload<T>(RedmineManager redmineManager, string address, string methodName,
            NameValueCollection parameters = null)
            where T : class, new()
        {
            using (var wc = redmineManager.CreateWebClient(parameters))
            {
                try
                {
                    var response = wc.DownloadString(address);
                    if (!string.IsNullOrEmpty(response))
                        return RedmineSerializer.Deserialize<T>(response, redmineManager.MimeFormat);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, redmineManager.MimeFormat);
                }
                return default(T);
            }
        }

        public static PaginatedObjects<T> ExecuteDownloadList<T>(RedmineManager redmineManager, string address,
            string methodName,
            NameValueCollection parameters = null) where T : class, new()
        {
            using (var wc = redmineManager.CreateWebClient(parameters))
            {
                try
                {
                    var response = wc.DownloadString(address);
                    return RedmineSerializer.DeserializeList<T>(response, redmineManager.MimeFormat);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, redmineManager.MimeFormat);
                }
                return null;
            }
        }

        public static byte[] ExecuteDownloadFile(RedmineManager redmineManager, string address, string methodName)
        {
            using (var wc = redmineManager.CreateWebClient(null, true))
            {
                try
                {
                    return wc.DownloadData(address);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, redmineManager.MimeFormat);
                }
                return null;
            }
        }

        public static Upload ExecuteUploadFile(RedmineManager redmineManager, string address, byte[] data, string methodName)
        {
            using (var wc = redmineManager.CreateWebClient(null, true))
            {
                try
                {
                    var response = wc.UploadData(address, data);
                    var responseString = Encoding.ASCII.GetString(response);
                    return RedmineSerializer.Deserialize<Upload>(responseString, redmineManager.MimeFormat);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, redmineManager.MimeFormat);
                }
                return null;
            }
        }
    }
}