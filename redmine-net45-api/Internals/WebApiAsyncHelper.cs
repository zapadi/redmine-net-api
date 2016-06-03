using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Internals
{
    internal static class WebApiAsyncHelper
    {
        public static async Task ExecuteUpload(RedmineManager redmineManager, string address, string actionType, string data,
            string methodName)
        {
            using (var wc = redmineManager.CreateWebClient(null))
            {
                try
                {
                    if (actionType == HttpVerbs.POST || actionType == HttpVerbs.DELETE || actionType == HttpVerbs.PUT ||
                        actionType == HttpVerbs.PATCH)
                    {
                        await wc.UploadStringTaskAsync(address, actionType, data).ConfigureAwait(false);
                    }
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, redmineManager.MimeFormat);
                }
            }
        }

        public static async Task<T> ExecuteUpload<T>(RedmineManager redmineManager, string address, string actionType, string data,
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
                        var response = await wc.UploadStringTaskAsync(address, actionType, data).ConfigureAwait(false);
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

        public static async Task<T> ExecuteDownload<T>(RedmineManager redmineManager, string address, string methodName,
            NameValueCollection parameters = null)
            where T : class, new()
        {
            using (var wc = redmineManager.CreateWebClient(parameters))
            {
                try
                {
                    var response = await wc.DownloadStringTaskAsync(address).ConfigureAwait(false);
                    return RedmineSerializer.Deserialize<T>(response, redmineManager.MimeFormat);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, redmineManager.MimeFormat);
                }
                return default(T);
            }
        }

        public static async Task<List<T>> ExecuteDownloadList<T>(RedmineManager redmineManager, string address,
           string methodName,
           NameValueCollection parameters = null) where T : class, new()
        {
            using (var wc = redmineManager.CreateWebClient(parameters))
            {
                try
                {
                    var response = await wc.DownloadStringTaskAsync(address).ConfigureAwait(false);
                    var result = RedmineSerializer.DeserializeList<T>(response, redmineManager.MimeFormat);
                    if (result != null)
                        return result.Objects;
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, redmineManager.MimeFormat);
                }
                return null;
            }
        }


        public static async Task<PaginatedObjects<T>> ExecuteDownloadPaginatedList<T>(RedmineManager redmineManager, string address,
            string methodName,
            NameValueCollection parameters = null) where T : class, new()
        {
            using (var wc = redmineManager.CreateWebClient(parameters))
            {
                try
                {
                    var response = await wc.DownloadStringTaskAsync(address).ConfigureAwait(false);
                    return RedmineSerializer.DeserializeList<T>(response, redmineManager.MimeFormat);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, redmineManager.MimeFormat);
                }
                return null;
            }
        }

        public static async Task<byte[]> ExecuteDownloadFile(RedmineManager redmineManager, string address, string methodName)
        {
            using (var wc = redmineManager.CreateWebClient(null, true))
            {
                try
                {
                    return await wc.DownloadDataTaskAsync(address);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(methodName, redmineManager.MimeFormat);
                }
                return null;
            }
        }

        public static async Task<Upload> ExecuteUploadFile(RedmineManager redmineManager, string address, byte[] data, string methodName)
        {
            using (var wc = redmineManager.CreateWebClient(null, true))
            {
                try
                {
                    var response = await wc.UploadDataTaskAsync(address, data);
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