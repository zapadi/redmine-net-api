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
    [XmlRoot(RedmineKeys.JOURNAL)]
    public sealed class Journal : Identifiable<Journal>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the user.
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
        public string Notes { get; internal set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>
        /// The created on.
        /// </value>
        public DateTime? CreatedOn { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool PrivateNotes { get; internal set; }

        /// <summary>
        /// Gets or sets the details.
        /// </summary>
        /// <value>
        /// The details.
        /// </value>
        public IList<Detail> Details { get; internal set; }
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
                    case RedmineKeys.DETAILS: Details = reader.ReadElementContentAsCollection<Detail>(); break;
                    case RedmineKeys.NOTES: Notes = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.PRIVATE_NOTES: PrivateNotes = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;
                    default: reader.Read(); break;
                }
            }
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
                    case RedmineKeys.DETAILS: Details = reader.ReadAsCollection<Detail>(); break;
                    case RedmineKeys.NOTES: Notes = reader.ReadAsString(); break;
                    case RedmineKeys.PRIVATE_NOTES: PrivateNotes = reader.ReadAsBool(); break;
                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;
                    default: reader.Read(); break;
                }
            }
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
            return Id == other.Id
                && User == other.User
                && Notes == other.Notes
                && CreatedOn == other.CreatedOn
                && (Details != null ? Details.Equals<Detail>(other.Details) : other.Details == null);
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
                hashCode = HashCodeHelper.GetHashCode(Id, hashCode);
                hashCode = HashCodeHelper.GetHashCode(User, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Notes, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Details, hashCode);
                return hashCode;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[{nameof(Journal)}: {ToString()}, User={User}, Notes={Notes}, CreatedOn={CreatedOn?.ToString("u", CultureInfo.InvariantCulture)}, Details={Details.Dump()}]";

    }
}