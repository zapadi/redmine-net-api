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
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.CUSTOM_FIELD)]
    public sealed class MyAccountCustomField : IdentifiableName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyAccountCustomField"/> class.
        /// </summary>
        /// <remarks>Serialization</remarks>
        public MyAccountCustomField() { }

        
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; internal set; }
        
        internal MyAccountCustomField(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <inheritdoc />
        public override void ReadXml(XmlReader reader)
        {
            base.ReadXml(reader);
            while (!reader.EOF)
            {
                if (reader.IsEmptyElement)
                {
                    reader.Read();
                    continue;
                }

                switch (reader.Name)
                {
                    case RedmineKeys.VALUE:
                        Value = reader.ReadElementContentAsString();
                        break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        /// <inheritdoc />
        public override void WriteXml(XmlWriter writer)
        {
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
                        case RedmineKeys.VALUE: Value = reader.ReadAsString(); break;
                        default: reader.Read(); break;
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer)
        {
        }

        /// <inheritdoc />
        public override bool Equals(IdentifiableName other)
        {
            var result = base.Equals(other);
           
            return result && string.Equals(Value,((MyAccountCustomField)other)?.Value, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = HashCodeHelper.GetHashCode(Value, hashCode);
                return hashCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(MyAccountCustomField)}: {ToString()}, Value: {Value}]";

    }
}