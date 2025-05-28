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
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Serialization.Json;
using Redmine.Net.Api.Serialization.Xml.Extensions;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
    [XmlRoot(RedmineKeys.CHANGE_SET)]
    public sealed class ChangeSet : IXmlSerializable, IJsonSerializable, IEquatable<ChangeSet>
    ,ICloneable<ChangeSet>
    {
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public string Revision { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public IdentifiableName User { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string Comments { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? CommittedOn { get; internal set; }
        #endregion

        #region Implementation of IXmlSerializable
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
            reader.Read();
            while (!reader.EOF)
            {
                if (reader.IsEmptyElement && !reader.HasAttributes)
                {
                    reader.Read();
                    continue;
                }

                Revision = reader.GetAttribute(RedmineKeys.REVISION);

                switch (reader.Name)
                {
                    case RedmineKeys.COMMENTS: Comments = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.COMMITTED_ON: CommittedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;

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

        #region Implementation of IJsonSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ReadJson(JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    return;
                }

                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                switch (reader.Value)
                {
                    case RedmineKeys.COMMENTS: Comments = reader.ReadAsString(); break;

                    case RedmineKeys.COMMITTED_ON: CommittedOn = reader.ReadAsDateTime(); break;

                    case RedmineKeys.REVISION: Revision = reader.ReadAsString(); break;

                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;

                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteJson(JsonWriter writer) { }
        #endregion

        #region Implementation of IEquatable<ChangeSet>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ChangeSet other)
        {
            if (other == null) return false;

            return Revision == other.Revision
                && User == other.User
                && string.Equals(Comments, other.Comments, StringComparison.Ordinal)
                && CommittedOn == other.CommittedOn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resetId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ChangeSet Clone(bool resetId)
        {
            return new ChangeSet()
            {
                User = User,
                Comments = Comments,
                Revision = Revision,
                CommittedOn = CommittedOn,
            };
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
            return Equals(obj as ChangeSet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 17;
            hashCode = HashCodeHelper.GetHashCode(Revision, hashCode);
            hashCode = HashCodeHelper.GetHashCode(User, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Comments, hashCode);
            hashCode = HashCodeHelper.GetHashCode(CommittedOn, hashCode);
            return hashCode;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ChangeSet left, ChangeSet right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ChangeSet left, ChangeSet right)
        {
            return !Equals(left, right);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $" ChangeSet: Revision={Revision}, CommittedOn={CommittedOn?.ToString("u", CultureInfo.InvariantCulture)}]";

    }
}