/*
   Copyright 2011 - 2015 Adrian Popescu, Dorin Huzum.

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

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.0
    /// </summary>
    [XmlRoot(RedmineKeys.PROJECT)]
    public class Project : IdentifiableName, IEquatable<Project>
    {
        /// <summary>
        /// Gets or sets the identifier.
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

        [XmlArray(RedmineKeys.CUSTOM_FIELDS)]
        [XmlArrayItem(RedmineKeys.CUSTOM_FIELD)]
        public IList<IssueCustomField> CustomFields { get; set; }

        [XmlArray(RedmineKeys.ISSUE_CATEGORIES)]
        [XmlArrayItem(RedmineKeys.ISSUE_CATEGORY)]
        public IList<ProjectIssueCategory> IssueCategories { get; set; }

        /// <summary>
        /// since 2.6.0
        /// </summary>
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

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.NAME, Name);
            writer.WriteElementString(RedmineKeys.IDENTIFIER, Identifier);
            writer.WriteElementString(RedmineKeys.DESCRIPTION, Description);
            writer.WriteElementString(RedmineKeys.INHERIT_MEMBERS, InheritMembers.ToString());
            writer.WriteElementString(RedmineKeys.IS_PUBLIC, IsPublic.ToString());
            writer.WriteIdOrEmpty(Parent, RedmineKeys.PARENT_ID);
            writer.WriteElementString(RedmineKeys.HOMEPAGE, HomePage);

            if (Trackers != null)
            {
                foreach (var item in Trackers)
                {
                    writer.WriteElementString(RedmineKeys.TRACKER_IDS, item.Id.ToString());
                }
            }

            if (EnabledModules != null)
            {
                foreach (var item in EnabledModules)
                {
                    writer.WriteElementString(RedmineKeys.ENABLED_MODULE_NAMES, item.Name);
                }
            }

            if (Id == 0) return;

            writer.WriteArray(CustomFields, RedmineKeys.CUSTOM_FIELDS);
        }

        public bool Equals(Project other)
        {
            if (other == null) return false;
            return (Identifier == other.Identifier);
        }

        public override int GetHashCode()
        {
            var hashCode = !string.IsNullOrEmpty(Identifier) ? Identifier.GetHashCode() : 0;
            return hashCode;
        }
    }
}