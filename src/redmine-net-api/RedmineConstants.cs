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

namespace Redmine.Net.Api
{
    /// <summary>
    /// 
    /// </summary>
    public static class RedmineConstants
    {
        /// <summary>
        /// 
        /// </summary>
        internal const string OBSOLETE_TEXT = "In next major release, it will no longer be available.";
        /// <summary>
        /// 
        /// </summary>
        public const int DEFAULT_PAGE_SIZE_VALUE = 25;
        
        /// <summary>
        /// 
        /// </summary>
        public const string CONTENT_TYPE_APPLICATION_JSON = "application/json";
        /// <summary>
        /// 
        /// </summary>
        public const string CONTENT_TYPE_APPLICATION_XML = "application/xml";
        /// <summary>
        /// 
        /// </summary>
        public const string CONTENT_TYPE_APPLICATION_STREAM = "application/octet-stream";
        
        /// <summary>
        /// 
        /// </summary>
        public const string IMPERSONATE_HEADER_KEY = "X-Redmine-Switch-User";
        
        /// <summary>
        /// 
        /// </summary>
        public const string AUTHORIZATION_HEADER_KEY = "Authorization";
        
        /// <summary>
        /// 
        /// </summary>
        public const string XML = "xml";
        
        /// <summary>
        /// 
        /// </summary>
        public const string JSON = "json";
    }
}