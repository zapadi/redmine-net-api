/*
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

using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.STATUS)]
    public sealed class IssueAllowedStatus : IdentifiableName
    {
        /// <summary>
        /// 
        /// </summary>
        public bool? IsClosed { get; internal set; }

        /// <inheritdoc />
        public override void ReadXml(XmlReader reader)
        {
            Id = reader.ReadAttributeAsInt(RedmineKeys.ID);
            Name = reader.GetAttribute(RedmineKeys.NAME);
            IsClosed = reader.ReadAttributeAsBoolean(RedmineKeys.IS_CLOSED);
            reader.Read();
        }

        /// <inheritdoc />
        public override void ReadJson(JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    return;
                }

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    switch (reader.Value)
                    {
                        case RedmineKeys.ID: Id = reader.ReadAsInt(); break;
                        case RedmineKeys.NAME: Name = reader.ReadAsString(); break;
                        case RedmineKeys.IS_CLOSED: IsClosed = reader.ReadAsBoolean(); break;
                        default: reader.Read(); break;
                    }
                }
            }
        }
        
        private string DebuggerDisplay => $"[{nameof(IssueAllowedStatus)}: {ToString()}]";
    }
}