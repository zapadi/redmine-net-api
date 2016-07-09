/*
   Copyright 2011 - 2016 Adrian Popescu.

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
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.3
    /// </summary>
    [XmlRoot(RedmineKeys.VERSION)]
    public class Version : IdentifiableName, IEquatable<Version>
    {
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        [XmlElement(RedmineKeys.PROJECT)]
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlElement(RedmineKeys.DESCRIPTION)]
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [XmlElement(RedmineKeys.STATUS)]
        public VersionStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>The due date.</value>
        [XmlElement(RedmineKeys.DUE_DATE, IsNullable = true)]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the sharing.
        /// </summary>
        /// <value>The sharing.</value>
        [XmlElement(RedmineKeys.SHARING)]
        public VersionSharing Sharing { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement(RedmineKeys.CREATED_ON, IsNullable = true)]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        [XmlElement(RedmineKeys.UPDATED_ON, IsNullable = true)]
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        [XmlArray(RedmineKeys.CUSTOM_FIELDS)]
        [XmlArrayItem(RedmineKeys.CUSTOM_FIELD)]
        public IList<IssueCustomField> CustomFields { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public override void ReadXml(XmlReader reader)
        {
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
                    case RedmineKeys.ID: Id = reader.ReadElementContentAsInt(); break;

                    case RedmineKeys.NAME: Name = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;

                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.STATUS: Status = (VersionStatus)Enum.Parse(typeof(VersionStatus), reader.ReadElementContentAsString(), true); break;

                    case RedmineKeys.DUE_DATE: DueDate = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.SHARING: Sharing = (VersionSharing)Enum.Parse(typeof(VersionSharing), reader.ReadElementContentAsString(), true); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;

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
            writer.WriteElementString(RedmineKeys.STATUS, Status.ToString().ToLowerInvariant());
            writer.WriteElementString(RedmineKeys.SHARING, Sharing.ToString().ToLowerInvariant());

            writer.WriteDateOrEmpty(DueDate, RedmineKeys.DUE_DATE);
            writer.WriteElementString(RedmineKeys.DESCRIPTION, Description);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Version other)
        {
            if (other == null) return false;
            return (Id == other.Id && Name == other.Name
                && Project == other.Project
                && Description == other.Description
                && Status == other.Status
                && DueDate == other.DueDate
                && Sharing == other.Sharing
                && CreatedOn == other.CreatedOn
                && UpdatedOn == other.UpdatedOn
                && (CustomFields != null ? CustomFields.Equals<IssueCustomField>(other.CustomFields) : other.CustomFields == null));
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
                return hashCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Version: {8}, Project={0}, Description={1}, Status={2}, DueDate={3}, Sharing={4}, CreatedOn={5}, UpdatedOn={6}, CustomFields={7}]",
                Project, Description, Status, DueDate, Sharing, CreatedOn, UpdatedOn, CustomFields, base.ToString());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum VersionSharing
    {
        /// <summary>
        /// 
        /// </summary>
        none = 1,
        /// <summary>
        /// 
        /// </summary>
        descendants,
        /// <summary>
        /// 
        /// </summary>
        hierarchy,
        /// <summary>
        /// 
        /// </summary>
        tree,
        /// <summary>
        /// 
        /// </summary>
        system
    }

    /// <summary>
    /// 
    /// </summary>
    public enum VersionStatus
    {
        /// <summary>
        /// 
        /// </summary>
        open = 1,
        /// <summary>
        /// 
        /// </summary>
        locked,
        /// <summary>
        /// 
        /// </summary>
        closed
    }
}