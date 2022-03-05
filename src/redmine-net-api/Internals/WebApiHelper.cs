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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading;
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
                if (actionType == HttpVerbs.POST || actionType == HttpVerbs.DELETE || actionType == HttpVerbs.PUT ||
                    actionType == HttpVerbs.PATCH)
                {
                    wc.UploadString(address, actionType, data);
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
                switch (actionType)
                {
                    case HttpVerbs.POST:
                    case HttpVerbs.DELETE:
                    case HttpVerbs.PUT:
                    case HttpVerbs.PATCH:
                    {
                        var response = wc.UploadString(address, actionType, data);
                        return redmineManager.Serializer.Deserialize<T>(response);
                    }

                    default:
                        return default;
                }
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
                var response = wc.DownloadString(address);
                if (!string.IsNullOrEmpty(response))
                {
                    return redmineManager.Serializer.Deserialize<T>(response);
                }

                return default;
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
                var response = wc.DownloadString(address);
                return redmineManager.Serializer.DeserializeToPagedResults<T>(response);
            }
        }

        /// <summary>
        /// Executes the download file.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        /// <param name="address">The address.</param>
        /// <param name="filename">The name of the file to be placed on the local computer.</param>
        /// <returns></returns>
        public static void ExecuteDownloadFile(RedmineManager redmineManager, string address, string filename)
        {
            using (var wc = redmineManager.CreateWebClient(null, true))
            {
                wc.DownloadProgressChanged += HandleDownloadProgress;
                wc.DownloadFileCompleted += HandleDownloadComplete;

                var syncObject = new object();
                lock (syncObject)
                {
                    wc.DownloadFileAsync(new Uri(address), filename, syncObject);
                    //This would block the thread until download completes
                    Monitor.Wait(syncObject);
                }

                wc.DownloadProgressChanged -= HandleDownloadProgress;
                wc.DownloadFileCompleted -= HandleDownloadComplete;
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
                return wc.DownloadData(address);
            }
        }

        private static void HandleDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            lock (e.UserState)
            {
                //releases blocked thread
                Monitor.Pulse(e.UserState);
            }
        }

        private static void HandleDownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
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
                var response = wc.UploadData(address, data);
                var responseString = Encoding.ASCII.GetString(response);
                return redmineManager.Serializer.Deserialize<Upload>(responseString);
            }
        }
    }
}