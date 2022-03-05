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
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.3
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.VERSION)]
    public sealed class Version : Identifiable<Version>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the project.
        /// </summary>
        /// <value>The project.</value>
        public IdentifiableName Project { get; internal set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public VersionStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>The due date.</value>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the sharing.
        /// </summary>
        /// <value>The sharing.</value>
        public VersionSharing Sharing { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string WikiPageTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float? EstimatedHours { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float? SpentHours { get; set; }

        /// <summary>
        /// Gets the created on.
        /// </summary>
        /// <value>The created on.</value>
        public DateTime? CreatedOn { get; internal set; }

        /// <summary>
        /// Gets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        public DateTime? UpdatedOn { get; internal set; }

        /// <summary>
        /// Gets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        public IList<IssueCustomField> CustomFields { get; internal set; }
        #endregion

        #region Implementation of IXmlSerializable
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
            reader.Read();
            while (!reader.EOF)
            {
                switch (reader.Name)
                {
                    case RedmineKeys.ID: Id = reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.DUE_DATE: DueDate = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.NAME: Name = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;
                    case RedmineKeys.SHARING: Sharing = (VersionSharing)Enum.Parse(typeof(VersionSharing), reader.ReadElementContentAsString(), true); break;
                    case RedmineKeys.STATUS: Status = (VersionStatus)Enum.Parse(typeof(VersionStatus), reader.ReadElementContentAsString(), true); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.WIKI_PAGE_TITLE: WikiPageTitle = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.ESTIMATED_HOURS: EstimatedHours = reader.ReadElementContentAsNullableFloat(); break;
                    case RedmineKeys.SPENT_HOURS: SpentHours = reader.ReadElementContentAsNullableFloat(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.NAME, Name);
            writer.WriteElementString(RedmineKeys.STATUS, Status.ToString().ToLowerInv());
            writer.WriteElementString(RedmineKeys.SHARING, Sharing.ToString().ToLowerInv());
            writer.WriteDateOrEmpty(RedmineKeys.DUE_DATE, DueDate);
            writer.WriteElementString(RedmineKeys.DESCRIPTION, Description);
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
                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadAsString(); break;
                    case RedmineKeys.DUE_DATE: DueDate = reader.ReadAsDateTime(); break;
                    case RedmineKeys.NAME: Name = reader.ReadAsString(); break;
                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;
                    case RedmineKeys.SHARING: Sharing = (VersionSharing)Enum.Parse(typeof(VersionSharing), reader.ReadAsString() ?? string.Empty, true); break;
                    case RedmineKeys.STATUS: Status = (VersionStatus)Enum.Parse(typeof(VersionStatus), reader.ReadAsString() ?? string.Empty, true); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.WIKI_PAGE_TITLE: WikiPageTitle = reader.ReadAsString(); break;
                    case RedmineKeys.ESTIMATED_HOURS: EstimatedHours = (float?)reader.ReadAsDouble(); break;
                    case RedmineKeys.SPENT_HOURS: SpentHours = (float?)reader.ReadAsDouble(); break;
                    
                    
                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteJson(JsonWriter writer)
        {
            using (new JsonObject(writer, RedmineKeys.VERSION))
            {
                writer.WriteProperty(RedmineKeys.NAME, Name);
                writer.WriteProperty(RedmineKeys.STATUS, Status.ToString().ToLowerInv());
                writer.WriteProperty(RedmineKeys.SHARING, Sharing.ToString().ToLowerInv());
                writer.WriteProperty(RedmineKeys.DESCRIPTION, Description);
                writer.WriteDateOrEmpty(RedmineKeys.DUE_DATE, DueDate);
            }
        }
        #endregion

        #region Implementation of IEquatable<Version>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(Version other)
        {
            if (other == null) return false;
            return Id == other.Id && Name == other.Name
                && Project == other.Project
                && Description == other.Description
                && Status == other.Status
                && DueDate == other.DueDate
                && Sharing == other.Sharing
                && CreatedOn == other.CreatedOn
                && UpdatedOn == other.UpdatedOn
                && (CustomFields != null ? CustomFields.Equals<IssueCustomField>(other.CustomFields) : other.CustomFields == null)
                && string.Equals(WikiPageTitle,other.WikiPageTitle, StringComparison.InvariantCultureIgnoreCase)
                && EstimatedHours == other.EstimatedHours
                && SpentHours == other.SpentHours
                ;
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
                hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Status, hashCode);
                hashCode = HashCodeHelper.GetHashCode(DueDate, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Sharing, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);
                hashCode = HashCodeHelper.GetHashCode(WikiPageTitle, hashCode);
                hashCode = HashCodeHelper.GetHashCode(EstimatedHours, hashCode);
                hashCode = HashCodeHelper.GetHashCode(SpentHours, hashCode);
                return hashCode;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $@"[{nameof(Version)}: {ToString()}, 
Project={Project}, 
Description={Description}, 
Status={Status:G},
DueDate={DueDate?.ToString("u", CultureInfo.InvariantCulture)}, 
Sharing={Sharing:G}, 
CreatedOn={CreatedOn?.ToString("u", CultureInfo.InvariantCulture)}, 
UpdatedOn={UpdatedOn?.ToString("u", CultureInfo.InvariantCulture)}, 
EstimatedHours={EstimatedHours?.ToString("F", CultureInfo.InvariantCulture)},
SpentHours={SpentHours?.ToString("F", CultureInfo.InvariantCulture)},
WikiPageTitle={WikiPageTitle}
CustomFields={CustomFields.Dump()}]";

    }
}