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

using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class Attachments : Dictionary<int, Attachment>, IJsonSerializable
    {
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(RedmineKeys.ATTACHMENTS);
            foreach (var item in this)
            {
                writer.WriteStartElement(RedmineKeys.ATTACHMENT);
                writer.WriteAttributeString(RedmineKeys.ID, item.Key.ToInvariantString());
                writer.WriteIfNotDefaultOrNull(RedmineKeys.FILE_NAME, item.Value.FileName);
                writer.WriteIfNotDefaultOrNull(RedmineKeys.DESCRIPTION, item.Value.Description);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

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
                foreach (var item in this)
                {
                    writer.WritePropertyName(item.Key.ToInvariantString());
                    writer.WriteStartObject();
                    writer.WriteIfNotDefaultOrNull(RedmineKeys.FILE_NAME, item.Value.FileName);
                    writer.WriteIfNotDefaultOrNull(RedmineKeys.DESCRIPTION, item.Value.Description);
                    writer.WriteEndObject();
                }
            }
        }
    }
}