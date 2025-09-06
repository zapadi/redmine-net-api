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
                   && string.Equals(Value, other.Value, StringComparison.Ordinal);
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