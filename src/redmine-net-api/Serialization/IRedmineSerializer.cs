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

namespace Redmine.Net.Api.Serialization
{
    /// <summary>
    /// Serialization interface that supports serialize and deserialize methods.
    /// </summary>
    internal interface IRedmineSerializer
    {
        /// <summary>
        /// Gets the application format this serializer supports (e.g. "json", "xml").
        /// </summary>
        string Format { get; }
        
        /// <summary>
        /// 
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Serializes the specified object into a string.
        /// </summary>
        string Serialize<T>(T obj) where T : class;
     
        /// <summary>
        /// Deserializes the string into a PageResult of T object.
        /// </summary>
        PagedResults<T> DeserializeToPagedResults<T>(string response) where T : class, new();

        /// <summary>
        /// Deserializes the string into an object.
        /// </summary>
        T Deserialize<T>(string input) where T : new();
    }
}