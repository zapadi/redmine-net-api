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
    /// Availability 1.0
    /// </summary>
    [XmlRoot(RedmineKeys.PROJECT)]
    public class Project : IdentifiableName, IEquatable<Project>
    {
        /// <summary>
        /// Gets or sets the identifier (Required).
        /// </summary>
        /// <value>The identifier.</value>
        [XmlElement(RedmineKeys.IDENTIFIER)]
        public String Identifier { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlElement(RedmineKeys.DESCRIPTION)]
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        [XmlElement(RedmineKeys.PARENT)]
        public IdentifiableName Parent { get; set; }

        /// <summary>
        /// Gets or sets the home page.
        /// </summary>
        /// <value>The home page.</value>
        [XmlElement(RedmineKeys.HOMEPAGE)]
        public String HomePage { get; set; }

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
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [XmlElement(RedmineKeys.STATUS)]
        public ProjectStatus Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this project is public.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this project is public; otherwise, <c>false</c>.
        /// </value>
        /// <remarks> is exposed since 2.6.0</remarks>
        [XmlElement(RedmineKeys.IS_PUBLIC)]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [inherit members].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [inherit members]; otherwise, <c>false</c>.
        /// </value>
        [XmlElement(RedmineKeys.INHERIT_MEMBERS)]
        public bool InheritMembers { get; set; }

        /// <summary>
        /// Gets or sets the trackers.
        /// </summary>
        /// <value>
        /// The trackers.
        /// </value>
        [XmlArray(RedmineKeys.TRACKERS)]
        [XmlArrayItem(RedmineKeys.TRACKER)]
        public IList<ProjectTracker> Trackers { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>
        /// The custom fields.
        /// </value>
        [XmlArray(RedmineKeys.CUSTOM_FIELDS)]
        [XmlArrayItem(RedmineKeys.CUSTOM_FIELD)]
        public IList<IssueCustomField> CustomFields { get; set; }

        /// <summary>
        /// Gets or sets the issue categories.
        /// </summary>
        /// <value>
        /// The issue categories.
        /// </value>
        [XmlArray(RedmineKeys.ISSUE_CATEGORIES)]
        [XmlArrayItem(RedmineKeys.ISSUE_CATEGORY)]
        public IList<ProjectIssueCategory> IssueCategories { get; set; }

        /// <summary>
        /// since 2.6.0
        /// </summary>
        /// <value>
        /// The enabled modules.
        /// </value>
        [XmlArray(RedmineKeys.ENABLED_MODULES)]
        [XmlArrayItem(RedmineKeys.ENABLED_MODULE)]
        public IList<ProjectEnabledModule> EnabledModules { get; set; }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
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

                    case RedmineKeys.IDENTIFIER: Identifier = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.STATUS: Status = (ProjectStatus)reader.ReadElementContentAsInt(); break;

                    case RedmineKeys.PARENT: Parent = new IdentifiableName(reader); break;

                    case RedmineKeys.HOMEPAGE: HomePage = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.IS_PUBLIC: IsPublic = reader.ReadElementContentAsBoolean(); break;

                    case RedmineKeys.INHERIT_MEMBERS: InheritMembers = reader.ReadElementContentAsBoolean(); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.TRACKERS: Trackers = reader.ReadElementContentAsCollection<ProjectTracker>(); break;

                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;

                    case RedmineKeys.ISSUE_CATEGORIES: IssueCategories = reader.ReadElementContentAsCollection<ProjectIssueCategory>(); break;

                    case RedmineKeys.ENABLED_MODULES: EnabledModules = reader.ReadElementContentAsCollection<ProjectEnabledModule>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.NAME, Name);
            writer.WriteElementString(RedmineKeys.IDENTIFIER, Identifier);
            writer.WriteElementString(RedmineKeys.DESCRIPTION, Description);
            writer.WriteElementString(RedmineKeys.INHERIT_MEMBERS, InheritMembers.ToString().ToLowerInvariant());
            writer.WriteElementString(RedmineKeys.IS_PUBLIC, IsPublic.ToString().ToLowerInvariant());
            writer.WriteIdOrEmpty(Parent, RedmineKeys.PARENT_ID);
            writer.WriteElementString(RedmineKeys.HOMEPAGE, HomePage);

            writer.WriteListElements(Trackers as List<IValue>, RedmineKeys.TRACKER_IDS);
            writer.WriteListElements(EnabledModules as List<IValue>, RedmineKeys.ENABLED_MODULE_NAMES);

            if (Id == 0) return;

            writer.WriteArray(CustomFields, RedmineKeys.CUSTOM_FIELDS);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Project other)
        {
            if (other == null) return false;
            return (
                Id == other.Id
                && Identifier.Equals(other.Identifier)
                && Description.Equals(other.Description)
                && (Parent != null ? Parent.Equals(other.Parent) : other.Parent == null)
				&& (HomePage != null ? HomePage.Equals(other.HomePage) : other.HomePage == null)
                && CreatedOn == other.CreatedOn
                && UpdatedOn == other.UpdatedOn
                && Status == other.Status
                && IsPublic == other.IsPublic
                && InheritMembers == other.InheritMembers
                && (Trackers != null ? Trackers.Equals<ProjectTracker>(other.Trackers) : other.Trackers == null)
                && (CustomFields != null ? CustomFields.Equals<IssueCustomField>(other.CustomFields) : other.CustomFields == null)
                && (IssueCategories != null ? IssueCategories.Equals<ProjectIssueCategory>(other.IssueCategories) : other.IssueCategories == null)
                && (EnabledModules != null ? EnabledModules.Equals<ProjectEnabledModule>(other.EnabledModules) : other.EnabledModules == null)
            );
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
		        hashCode = HashCodeHelper.GetHashCode(Identifier, hashCode);
		        hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
		        hashCode = HashCodeHelper.GetHashCode(Parent, hashCode);
		        hashCode = HashCodeHelper.GetHashCode(HomePage, hashCode);
		        hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
		        hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);
		        hashCode = HashCodeHelper.GetHashCode(Status, hashCode);
		        hashCode = HashCodeHelper.GetHashCode(IsPublic, hashCode);
		        hashCode = HashCodeHelper.GetHashCode(InheritMembers, hashCode);
		        hashCode = HashCodeHelper.GetHashCode(Trackers, hashCode);
		        hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);
		        hashCode = HashCodeHelper.GetHashCode(IssueCategories, hashCode);
		        hashCode = HashCodeHelper.GetHashCode(EnabledModules, hashCode);

		        return hashCode;
	        }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Project: {13}, Identifier={0}, Description={1}, Parent={2}, HomePage={3}, CreatedOn={4}, UpdatedOn={5}, Status={6}, IsPublic={7}, InheritMembers={8}, Trackers={9}, CustomFields={10}, IssueCategories={11}, EnabledModules={12}]",
                Identifier, Description, Parent, HomePage, CreatedOn, UpdatedOn, Status, IsPublic, InheritMembers, Trackers, CustomFields, IssueCategories, EnabledModules, base.ToString());
        }
    }
}