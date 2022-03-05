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
using System.Globalization;
using System.Xml;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class IdentifiableName : Identifiable<IdentifiableName>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T Create<T>(int id) where T: IdentifiableName, new()
        { 
            var t = new T (){Id = id};
            return t;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiableName"/> class.
        /// </summary>
        public IdentifiableName() { }

        /// <summary>
        /// Initializes the class by using the given Id and Name.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <param name="name">The Name.</param>
        internal IdentifiableName(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentifiableName"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public IdentifiableName(XmlReader reader)
        {
            Initialize(reader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public IdentifiableName(JsonReader reader)
        {
            Initialize(reader);
        }

        private void Initialize(XmlReader reader)
        {
            ReadXml(reader);
        }

        private void Initialize(JsonReader reader)
        {
            ReadJson(reader);
        }

        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string Name { get; set; }
        #endregion

        #region Implementation of IXmlSerializable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
            Id = reader.ReadAttributeAsInt(RedmineKeys.ID);
            Name = reader.GetAttribute(RedmineKeys.NAME);
            reader.Read();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString(RedmineKeys.ID, Id.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString(RedmineKeys.NAME, Name);
        }

        #endregion

        #region Implementation of IJsonSerializable
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

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    switch (reader.Value)
                    {
                        case RedmineKeys.ID: Id = reader.ReadAsInt(); break;
                        case RedmineKeys.NAME: Name = reader.ReadAsString(); break;
                        default: reader.Read(); break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteJson(JsonWriter writer)
        {
            writer.WriteIdIfNotNull(RedmineKeys.ID, this);
            if (!Name.IsNullOrWhiteSpace())
            {
                writer.WriteProperty(RedmineKeys.NAME, Name);
            }
        }
        #endregion

        #region Implementation of IEquatable<IdentifiableName>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(IdentifiableName other)
        {
            if (other == null) return false;
            return Id == other.Id && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
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
                hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
                return hashCode;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(IdentifiableName)}: {base.ToString()}, Name={Name}]";
    }
}