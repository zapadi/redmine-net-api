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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [XmlRoot(RedmineKeys.CUSTOM_FIELD)]
    public sealed class MyAccountCustomField : IdentifiableName, IEquatable<MyAccountCustomField>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyAccountCustomField"/> class.
        /// </summary>
        /// <remarks>Serialization</remarks>
        public MyAccountCustomField() { }
        
        /// <summary>
        /// Gets or sets the value. For a multi-value field it holds the first value.
        /// </summary>
        /// <remarks>Sent to Redmine when creating or updating.</remarks>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets all the values of a multi-value field. It is null for a single-value field.
        /// </summary>
        /// <remarks>Sent to Redmine when creating or updating, only when set.</remarks>
        public IList<string> Values { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the field accepts multiple values.
        /// </summary>
        /// <remarks>Read-only: set by Redmine and not sent when creating or updating.</remarks>
        public bool Multiple { get; set; }
        
        internal MyAccountCustomField(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <inheritdoc />
        public override void ReadXml(XmlReader reader)
        {
            Multiple = reader.ReadAttributeAsBoolean(RedmineKeys.MULTIPLE);
            base.ReadXml(reader);
            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == RedmineKeys.VALUE && string.Equals(reader.GetAttribute(RedmineKeys.TYPE), "array", StringComparison.Ordinal))
                {
                    SetValues(ReadXmlValues(reader));
                    continue;
                }

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
                        reader.Skip();
                        break;
                }
            }
        }

        /// <inheritdoc />
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString(RedmineKeys.ID, Id.ToInvariantString());

            if (WritesValues)
            {
                writer.WriteArray(RedmineKeys.VALUE, RedmineKeys.VALUE, Values, value => value);
            }
            else
            {
                writer.WriteElementString(RedmineKeys.VALUE, Value);
            }
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
                        case RedmineKeys.MULTIPLE: Multiple = reader.ReadAsBool(); break;
                        case RedmineKeys.VALUE: ReadJsonValue(reader); break;
                        default: reader.Skip(); break;
                    }
                }
            }
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteProperty(RedmineKeys.ID, Id);

            if (WritesValues)
            {
                writer.WritePropertyName(RedmineKeys.VALUE);
                writer.WriteStartArray();
                foreach (var value in Values)
                {
                    writer.WriteValue(value);
                }
                writer.WriteEndArray();
            }
            else
            {
                writer.WriteProperty(RedmineKeys.VALUE, Value);
            }

            writer.WriteEndObject();
        }

        private bool WritesValues => Values != null && (Multiple || Values.Count > 1);

        private void SetValues(List<string> values)
        {
            Values = values;
            Value = values.Count > 0 ? values[0] : null;
        }

        private static List<string> ReadXmlValues(XmlReader reader)
        {
            var values = new List<string>();

            if (reader.IsEmptyElement)
            {
                reader.Read();
                return values;
            }

            reader.Read();
            while (!reader.EOF && reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == RedmineKeys.VALUE)
                {
                    values.Add(reader.ReadElementContentAsString());
                }
                else
                {
                    reader.Skip();
                }
            }

            if (!reader.EOF)
            {
                reader.Read();
            }

            return values;
        }

        private void ReadJsonValue(JsonReader reader)
        {
            var dateParseHandling = reader.DateParseHandling;
            reader.DateParseHandling = DateParseHandling.None;
            try
            {
                ReadJsonValueCore(reader);
            }
            finally
            {
                reader.DateParseHandling = dateParseHandling;
            }
        }

        private void ReadJsonValueCore(JsonReader reader)
        {
            reader.Read();
            switch (reader.TokenType)
            {
                case JsonToken.Null:
                    break;
                case JsonToken.StartArray:
                    var values = new List<string>();
                    while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                    {
                        if (reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.StartArray)
                        {
                            reader.Skip();
                        }
                        else if (reader.Value != null)
                        {
                            values.Add(Convert.ToString(reader.Value, CultureInfo.InvariantCulture));
                        }
                    }
                    SetValues(values);
                    break;
                case JsonToken.StartObject:
                    reader.Skip();
                    break;
                default:
                    Value = Convert.ToString(reader.Value, CultureInfo.InvariantCulture);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is MyAccountCustomField other && Equals(other);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MyAccountCustomField other)
        {
            if (other == null) return false;
            return base.Equals(other)
                   && string.Equals(Value, other.Value, StringComparison.Ordinal)
                   && Multiple == other.Multiple
                   && ValuesEqual(Values, other.Values);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = HashCodeHelper.GetHashCode(Value, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Values, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Multiple, hashCode);
                return hashCode;
            }
        }
        
        private static bool ValuesEqual(IList<string> left, IList<string> right)
        {
            if (left == null || right == null)
            {
                return left == null && right == null;
            }

            if (left.Count != right.Count)
            {
                return false;
            }

            for (var i = 0; i < left.Count; i++)
            {
                if (!string.Equals(left[i], right[i], StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(MyAccountCustomField left, MyAccountCustomField right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(MyAccountCustomField left, MyAccountCustomField right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[MyAccountCustomField: Id={Id.ToInvariantString()}, Name={Name}, Value: {Value}]";

    }
}