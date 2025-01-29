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
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.CUSTOM_FIELD)]
    public sealed class IssueCustomField : 
        IdentifiableName
        ,IEquatable<IssueCustomField>
        ,ICloneable<IssueCustomField>, IValue
    {
        #region Properties
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public IList<CustomFieldValue> Values { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Multiple { get; set; }
        #endregion

        #region Implementation of IXmlSerializable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
            Id = Convert.ToInt32(reader.GetAttribute(RedmineKeys.ID), CultureInfo.InvariantCulture);
            Name = reader.GetAttribute(RedmineKeys.NAME);
            Multiple = reader.ReadAttributeAsBoolean(RedmineKeys.MULTIPLE);

            reader.Read();

            if (reader.NodeType == XmlNodeType.Whitespace)
            {
                reader.Read();
            }

            if (reader.NodeType == XmlNodeType.Text)
            {
                Values = new List<CustomFieldValue>
                {
                    new CustomFieldValue(reader.Value)
                };

                reader.Read();
                return;
            }

            var attributeExists = !reader.GetAttribute("type").IsNullOrWhiteSpace();

            if (!attributeExists)
            {
                if (reader.IsEmptyElement)
                {
                    reader.Read();
                    return;
                }

                Values = new List<CustomFieldValue>
                {
                    new CustomFieldValue(reader.ReadElementContentAsString())
                };
            }
            else
            {
                Values = reader.ReadElementContentAsCollection<CustomFieldValue>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            if (Values == null)
            {
                return;
            }

            var itemsCount = Values.Count;

            writer.WriteAttributeString(RedmineKeys.ID, Id.ToString(CultureInfo.InvariantCulture));

            Multiple = itemsCount > 1;
            
            if (Multiple)
            {
                writer.WriteArrayStringElement(RedmineKeys.VALUE, Values, GetValue);
            }
            else
            {
                writer.WriteElementString(RedmineKeys.VALUE, itemsCount > 0 ? Values[0].Info : null);
            }
            
            writer.WriteBoolean(RedmineKeys.MULTIPLE, Multiple);
        }
        #endregion

        #region Implementation of IJsonSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteJson(JsonWriter writer)
        {
            if (Values == null)
            {
                return;
            }

            var itemsCount = Values.Count;
            Multiple = itemsCount > 1;

            writer.WriteStartObject();
            writer.WriteProperty(RedmineKeys.ID, Id);
            writer.WriteProperty(RedmineKeys.NAME, Name);
            writer.WriteBoolean(RedmineKeys.MULTIPLE, Multiple);
            
            if (Multiple)
            {
                writer.WritePropertyName(RedmineKeys.VALUE);
                writer.WriteStartArray();
                foreach (var cfv in Values)
                {
                    writer.WriteValue(cfv.Info);
                }
                writer.WriteEndArray();
            }
            else
            {
                writer.WriteProperty(RedmineKeys.VALUE, itemsCount > 0 ? Values[0].Info : null);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadJson(JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    return;
                }

                switch (reader.Value)
                {
                    case RedmineKeys.ID: Id = reader.ReadAsInt(); break;
                    case RedmineKeys.MULTIPLE: Multiple = reader.ReadAsBool(); break;
                    case RedmineKeys.NAME: Name = reader.ReadAsString(); break;
                    case RedmineKeys.VALUE:
                        reader.Read();
                        switch (reader.TokenType)
                        {
                            case JsonToken.Null: break;
                            case JsonToken.StartArray:
                                Values = reader.ReadAsCollection<CustomFieldValue>();
                                break;
                            default:
                                Values = new List<CustomFieldValue> { new CustomFieldValue { Info = reader.Value as string } };
                                break;
                        }
                        break;
                }
            }
        }

        #endregion

        #region Implementation of IEquatable<IssueCustomField>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IssueCustomField other)
        {
            if (other == null) return false;
            return Id == other.Id
                && Name == other.Name
                && Multiple == other.Multiple
                && Values.Equals(other.Values);
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
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as IssueCustomField);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = HashCodeHelper.GetHashCode(Values, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Multiple, hashCode);
                return hashCode;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(IssueCustomField left, IssueCustomField right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(IssueCustomField left, IssueCustomField right)
        {
            return !Equals(left, right);
        }
        #endregion

        #region Implementation of IClonable<IssueCustomField>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new IssueCustomField Clone()
        {
            var clone = new IssueCustomField
            {
                Id = Id,
                Name = Name,
                Multiple = Multiple,
            };

            if (Values != null)
            {
                clone.Values = new List<CustomFieldValue>(Values);
            }

            return clone;
        }
        
        #endregion

        #region Implementation of IValue
        /// <summary>
        /// 
        /// </summary>
        public string Value => Id.ToString(CultureInfo.InvariantCulture);

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetValue(object item)
        {
            return ((CustomFieldValue)item).Info;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(IssueCustomField)}: {ToString()} Values={Values.Dump()}, Multiple={Multiple.ToString(CultureInfo.InvariantCulture)}]";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Equals(object obj)
    }
}