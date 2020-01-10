/*
   Copyright 2011 - 2019 Adrian Popescu.

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
using System.Xml.Schema;
using System.Xml.Serialization;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.DETAIL)]
    public sealed class Detail : IXmlSerializable,  IEquatable<Detail>
    {
        /// <summary>
        /// 
        /// </summary>
        public Detail() { }

        internal Detail(string name = null, string property = null, string oldValue = null, string newValue = null)
        {
            Name = name;
            Property = property;
            OldValue = oldValue;
            NewValue = newValue;
        }

        #region Properties
        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public string Property { get; internal set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        /// <value>
        /// The old value.
        /// </value>
        public string OldValue { get; internal set; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        /// <value>
        /// The new value.
        /// </value>
        public string NewValue { get; internal set; }
        #endregion

        #region Implementation of IXmlSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema() { return null; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            Name = reader.GetAttribute(RedmineKeys.NAME);
            Property = reader.GetAttribute(RedmineKeys.PROPERTY);

            reader.Read();

            while (!reader.EOF)
            {
                if (reader.IsEmptyElement && !reader.HasAttributes)
                {
                    reader.Read();
                    continue;
                }

                switch (reader.Name)
                {
                    case RedmineKeys.NEW_VALUE: NewValue = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.OLD_VALUE: OldValue = reader.ReadElementContentAsString(); break;

                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer) { }
        #endregion

       

        #region Implementation of IEquatable<Detail>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Detail other)
        {
            if (other == null) return false;
            return (Property != null ? string.Equals(Property,other.Property, StringComparison.InvariantCultureIgnoreCase) : other.Property == null)
                && (Name != null ? string.Equals(Name,other.Name, StringComparison.InvariantCultureIgnoreCase) : other.Name == null)
                && (OldValue != null ? string.Equals(OldValue,other.OldValue, StringComparison.InvariantCultureIgnoreCase) : other.OldValue == null)
                && (NewValue != null ? string.Equals(NewValue,other.NewValue, StringComparison.InvariantCultureIgnoreCase) : other.NewValue == null);
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
            return Equals(obj as Detail);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 13;
                hashCode = HashCodeHelper.GetHashCode(Property, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
                hashCode = HashCodeHelper.GetHashCode(OldValue, hashCode);
                hashCode = HashCodeHelper.GetHashCode(NewValue, hashCode);

                return hashCode;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(Detail)}: Property={Property}, Name={Name}, OldValue={OldValue}, NewValue={NewValue}]";

    }
}