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

namespace Redmine.Net.Api
{

    /// <summary>
    /// 
    /// </summary>
    public static class HttpVerbs
    {
        /// <summary>
        /// Represents an HTTP PUT protocol method that is used to replace an entity identified by a URI.
        /// </summary>
        public const string PUT = "PUT";
        /// <summary>
        /// Represents an HTTP POST protocol method that is used to post a new entity as an addition to a URI.
        /// </summary>
        public const string POST = "POST";
        /// <summary>
        /// Represents an HTTP PATCH protocol method that is used to patch an existing entity identified  by a URI.
        /// </summary>
        public const string PATCH = "PATCH";
        /// <summary>
        /// Represents an HTTP DELETE protocol method that is used to delete an existing entity identified  by a URI.
        /// </summary>
        public const string DELETE = "DELETE";
    }
}