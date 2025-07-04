﻿/*
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

using System.Collections.Generic;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization.Json;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class Attachments : Dictionary<int, Attachment>, IJsonSerializable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ReadJson(JsonReader reader) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteJson(JsonWriter writer)
        {
            using (new JsonObject(writer, RedmineKeys.ATTACHMENTS))
            {
                writer.WriteStartArray();
                foreach (var item in this)
                {
                    writer.WritePropertyName(item.Key.ToInvariantString());
                    item.Value.WriteJson(writer);
                }
                writer.WriteEndArray();
            }
        }
    }
}