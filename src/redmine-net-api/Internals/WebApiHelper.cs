/*
   Copyright 2011 - 2019 Adrian Popescu.

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

using System.Collections.Specialized;
using System.Net;
using System.Text;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Internals
{
    /// <summary>
    /// 
    /// </summary>
    internal static class WebApiHelper
    {
	    /// <summary>
	    /// Executes the upload.
	    /// </summary>
	    /// <param name="redmineManager">The redmine manager.</param>
	    /// <param name="address">The address.</param>
	    /// <param name="actionType">Type of the action.</param>
	    /// <param name="data">The data.</param>
	    /// <param name="parameters">The parameters</param>
	    public static void ExecuteUpload(RedmineManager redmineManager, string address, string actionType, string data,
             NameValueCollection parameters = null)
        {
            using (var wc = redmineManager.CreateWebClient(parameters))
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
                    webException.HandleWebException(redmineManager.Serializer);
                }
            }
        }

        /// <summary>
        /// Executes the upload.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="address">The address.</param>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static T ExecuteUpload<T>(RedmineManager redmineManager, string address, string actionType, string data)
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
                        return redmineManager.Serializer.Deserialize<T>(response);
                    }
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(redmineManager.Serializer);
                }
                return default(T);
            }
        }

        /// <summary>
        /// Executes the download.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="address">The address.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static T ExecuteDownload<T>(RedmineManager redmineManager, string address, 
            NameValueCollection parameters = null)
            where T : class, new()
        {
            using (var wc = redmineManager.CreateWebClient(parameters))
            {
                try
                {
                    var response = wc.DownloadString(address);
                    if (!string.IsNullOrEmpty(response))
                        return redmineManager.Serializer.Deserialize<T>(response);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(redmineManager.Serializer);
                }
                return default(T);
            }
        }

        /// <summary>
        /// Executes the download list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="address">The address.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static PagedResults<T> ExecuteDownloadList<T>(RedmineManager redmineManager, string address,
            
            NameValueCollection parameters = null) where T : class, new()
        {
            using (var wc = redmineManager.CreateWebClient(parameters))
            {
                try
                {
                    var response = wc.DownloadString(address);
                    return redmineManager.Serializer.DeserializeToPagedResults<T>(response);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(redmineManager.Serializer);
                }
                return null;
            }
        }

        /// <summary>
        /// Executes the download file.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public static byte[] ExecuteDownloadFile(RedmineManager redmineManager, string address)
        {
            using (var wc = redmineManager.CreateWebClient(null, true))
            {
                try
                {
                    return wc.DownloadData(address);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(redmineManager.Serializer);
                }
                return null;
            }
        }

        /// <summary>
        /// Executes the upload file.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="address">The address.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Upload ExecuteUploadFile(RedmineManager redmineManager, string address, byte[] data)
        {
            using (var wc = redmineManager.CreateWebClient(null, true))
            {
                try
                {
                    var response = wc.UploadData(address, data);
                    var responseString = Encoding.ASCII.GetString(response);
                    return redmineManager.Serializer.Deserialize<Upload>(responseString);
                }
                catch (WebException webException)
                {
                    webException.HandleWebException(redmineManager.Serializer);
                }
                return null;
            }
        }
    }
}