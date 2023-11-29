﻿/*
   Copyright 2011 - 2023 Adrian Popescu

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
using System.Globalization;
using Newtonsoft.Json;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    internal class Attachments : Dictionary<int, Attachment>, IJsonSerializable
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
        /// <param name="full"></param>
        public void WriteJson(JsonWriter writer, bool full = false)
        {
            using (new JsonObject(writer, RedmineKeys.ATTACHMENTS))
            {
                writer.WriteStartArray();
                foreach (var item in this)
                {
                    writer.WritePropertyName(item.Key.ToString(CultureInfo.InvariantCulture));
                    item.Value.WriteJson(writer);
                }
                writer.WriteEndArray();
            }
        }
    }
}