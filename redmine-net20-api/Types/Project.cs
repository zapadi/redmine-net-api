/*
   Copyright 2011 - 2015 Adrian Popescu

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
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.0
    /// </summary>
    [XmlRoot("project")]
    // [DataContract(Name = "project")]
    public class Project : IdentifiableName, IEquatable<Project>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [XmlElement("identifier")]
        public String Identifier { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlElement("description")]
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        [XmlElement("parent")]
        public IdentifiableName Parent { get; set; }

        /// <summary>
        /// Gets or sets the home page.
        /// </summary>
        /// <value>The home page.</value>
        [XmlElement("homepage")]
        public String HomePage { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement("created_on")]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        [XmlElement("updated_on")]
        public DateTime? UpdatedOn { get; set; }

        [XmlElement("status")]
        public ProjectStatus Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this project is public.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this project is public; otherwise, <c>false</c>.
        /// </value>
        /// <remarks> is exposed since 2.6.0</remarks>
        [XmlElement("is_public")]
        public bool IsPublic { get; set; }

        [XmlElement("inherit_members")]
        public bool InheritMembers { get; set; }

        /// <summary>
        /// Gets or sets the trackers.
        /// </summary>
        /// <value>
        /// The trackers.
        /// </value>
        [XmlArray("trackers")]
        [XmlArrayItem("tracker")]
        public IList<ProjectTracker> Trackers { get; set; }

        [XmlArray("custom_fields")]
        [XmlArrayItem("custom_field")]
        public IList<IssueCustomField> CustomFields { get; set; }

        [XmlArray("issue_categories")]
        [XmlArrayItem("issue_category")]
        public IList<ProjectIssueCategory> IssueCategories { get; set; }

        /// <summary>
        /// enabled_module_names: (repeatable element) the module name: boards, calendar, documents, files, gantt, issue_tracking, news, repository, time_tracking, wiki
        /// </summary>
        public string EnabledModuleNames { get; set; }

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
                    case "id": Id = reader.ReadElementContentAsInt(); break;

                    case "name": Name = reader.ReadElementContentAsString(); break;

                    case "identifier": Identifier = reader.ReadElementContentAsString(); break;

                    case "description": Description = reader.ReadElementContentAsString(); break;

                    case "status": Status = (ProjectStatus)reader.ReadElementContentAsInt(); break;

                    case "parent": Parent = new IdentifiableName(reader); break;

                    case "homepage": HomePage = reader.ReadElementContentAsString(); break;

                    case "is_public": IsPublic = reader.ReadElementContentAsBoolean(); break;

                    case "inherit_members": InheritMembers = reader.ReadElementContentAsBoolean(); break;

                    case "created_on": CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case "updated_on": UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case "trackers": Trackers = reader.ReadElementContentAsCollection<ProjectTracker>(); break;

                    case "custom_fields": CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;

                    case "issue_categories": IssueCategories = reader.ReadElementContentAsCollection<ProjectIssueCategory>(); break;
                    default: reader.Read(); break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("name", Name);
            writer.WriteElementString("identifier", Identifier);
            writer.WriteElementString("description", Description);
            writer.WriteElementString("inherit_members", InheritMembers.ToString());
            writer.WriteElementString("is_public", IsPublic.ToString());
            writer.WriteIdIfNotNull(Parent, "parent_id");
            writer.WriteElementString("homepage", HomePage);

            if (!string.IsNullOrEmpty(EnabledModuleNames))
            {
               var tokens= EnabledModuleNames.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
               if (tokens != null)
               {
                   foreach (var token in tokens)
                   {
                       writer.WriteElementString("enabled_module_names", token);
                   }
               }
            }

            if (Id == 0) return;

            if (CustomFields != null)
            {
                writer.WriteStartElement("custom_fields");
                writer.WriteAttributeString("type", "array");
                foreach (var cf in CustomFields)
                {
                    new XmlSerializer(cf.GetType()).Serialize(writer, cf);
                }
                writer.WriteEndElement();
            }
        }

        public bool Equals(Project other)
        {
            if (other == null) return false;
            return (Id == other.Id && Identifier == other.Identifier);
        }
    }
}