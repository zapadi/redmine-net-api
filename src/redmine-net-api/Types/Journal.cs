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
using Redmine.Net.Api.Serialization.Json.Extensions;
using Redmine.Net.Api.Serialization.Xml.Extensions;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
    [XmlRoot(RedmineKeys.JOURNAL)]
    public sealed class Journal : 
        Identifiable<Journal>
        ,ICloneable<Journal>
    {
        #region Properties
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public IdentifiableName User { get; internal set; }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>
        /// The notes.
        /// </value>
        /// <remarks> Setting Notes to string.empty or null will destroy the journal
        /// </remarks>
        public string Notes { get; set; }

        /// <summary>
        /// Gets the created on.
        /// </summary>
        /// <value>
        /// The created on.
        /// </value>
        public DateTime? CreatedOn { get; internal set; }
        
        /// <summary>
        /// Gets the updated on.
        /// </summary>
        /// <value>
        /// The updated on.
        /// </value>
        public DateTime? UpdatedOn { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool PrivateNotes { get; internal set; }

        /// <summary>
        /// Gets the details.
        /// </summary>
        /// <value>
        /// The details.
        /// </value>
        public List<Detail> Details { get; internal set; }
        
        /// <summary>
        /// 
        /// </summary>
        public IdentifiableName UpdatedBy { get; internal set; }
        #endregion

        #region Implementation of IXmlSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
            Id = reader.ReadAttributeAsInt(RedmineKeys.ID);
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
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.DETAILS: Details = reader.ReadElementContentAsCollection<Detail>(); break;
                    case RedmineKeys.NOTES: Notes = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.PRIVATE_NOTES: PrivateNotes = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;
                    case RedmineKeys.UPDATED_BY: UpdatedBy = new IdentifiableName(reader); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <inheritdoc />
        public override void WriteXml(XmlWriter writer)
        { 
            writer.WriteElementString(RedmineKeys.NOTES, Notes);
        }

        #endregion

        #region Implementation of IJsonSerialization
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

                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                switch (reader.Value)
                {
                    case RedmineKeys.ID: Id = reader.ReadAsInt(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.DETAILS: Details = reader.ReadAsCollection<Detail>(); break;
                    case RedmineKeys.NOTES: Notes = reader.ReadAsString(); break;
                    case RedmineKeys.PRIVATE_NOTES: PrivateNotes = reader.ReadAsBool(); break;
                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;
                    case RedmineKeys.UPDATED_BY: UpdatedBy = new IdentifiableName(reader); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer)
        {
            writer.WriteProperty(RedmineKeys.NOTES, Notes);
        }

        #endregion

        #region Implementation of IEquatable<Journal>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(Journal other)
        {
            if (other == null) return false;
            var result = base.Equals(other);
            result = result && User == other.User;
            result = result && UpdatedBy == other.UpdatedBy;
            result = result && (Details?.Equals<Detail>(other.Details) ?? other.Details == null);
            result = result && string.Equals(Notes, other.Notes, StringComparison.Ordinal);
            result = result && CreatedOn == other.CreatedOn;
            result = result && UpdatedOn == other.UpdatedOn;
            result = result && PrivateNotes == other.PrivateNotes;
            return result;
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
            return Equals(obj as Journal);
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
                hashCode = HashCodeHelper.GetHashCode(User, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Notes, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Details, hashCode);
                hashCode = HashCodeHelper.GetHashCode(PrivateNotes, hashCode);
                hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(UpdatedBy, hashCode);
                return hashCode;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Journal left, Journal right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Journal left, Journal right)
        {
            return !Equals(left, right);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[Journal: Id={Id.ToInvariantString()}, CreatedOn={CreatedOn?.ToString("u", CultureInfo.InvariantCulture)}]";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new Journal Clone(bool resetId)
        {
            if (resetId)
            {
                return new Journal
                {
                    User = User?.Clone(false),
                    Notes = Notes,
                    CreatedOn = CreatedOn,
                    PrivateNotes = PrivateNotes,
                    Details = Details?.Clone(false)
                };
            }
            return new Journal
            {
                Id = Id,
                User = User?.Clone(false),
                Notes = Notes,
                CreatedOn = CreatedOn,
                PrivateNotes = PrivateNotes,
                Details = Details?.Clone(false)
            };
        }
    }
}