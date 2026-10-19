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
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.0
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [XmlRoot(RedmineKeys.PROJECT)]
    public sealed class Project : IdentifiableName, IEquatable<Project>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <remarks>
        /// <para>Required for create</para>
        /// <para>Sent to Redmine when creating or updating.</para>
        /// </remarks>
        /// <value>The identifier.</value>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        /// <remarks>Sent to Redmine when creating or updating, only when set.</remarks>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        /// <remarks>Sent to Redmine when creating or updating, only when set. Only its Id is sent.</remarks>
        public IdentifiableName Parent { get; set; }

        /// <summary>
        /// Gets or sets the home page.
        /// </summary>
        /// <value>The home page.</value>
        /// <remarks>Sent to Redmine when creating or updating, only when set.</remarks>
        public string HomePage { get; set; }

        /// <summary>
        /// Gets the created on.
        /// </summary>
        /// <value>The created on.</value>
        /// <remarks>Read-only: set by Redmine and not sent when creating or updating.</remarks>
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        /// <remarks>Read-only: set by Redmine and not sent when creating or updating.</remarks>
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        /// <remarks>Read-only: set by Redmine and not sent when creating or updating.</remarks>
        public ProjectStatus Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this project is public.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this project is public; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// <para>Available in Redmine starting with 2.6.0 version.</para>
        /// <para>Sent to Redmine when creating or updating.</para>
        /// </remarks>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [inherit members].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [inherit members]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Sent to Redmine when creating or updating.</remarks>
        public bool InheritMembers { get; set; }

        /// <summary>
        /// Gets or sets the trackers.
        /// </summary>
        /// <value>
        /// The trackers.
        /// </value>
        /// <remarks>
        /// <para>Available in Redmine starting with 2.6.0 version.</para>
        /// <para>Sent to Redmine when creating or updating, only when set. Only their Ids are sent.</para>
        /// </remarks>
        public IList<ProjectTracker> Trackers { get; set; }

        /// <summary>
        /// Gets or sets the enabled modules.
        /// </summary>
        /// <value>
        /// The enabled modules.
        /// </value>
        /// <remarks>
        /// <para>Available in Redmine starting with 2.6.0 version.</para>
        /// <para>Sent to Redmine when creating or updating, only when set. Only their names are sent.</para>
        /// </remarks>
        public IList<ProjectEnabledModule> EnabledModules { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>
        /// The custom fields.
        /// </value>
        /// <remarks>Sent to Redmine when creating or updating, only when set. Only their Ids are sent. Only sent when creating.</remarks>
        [Obsolete($"{RedmineConstants.OBSOLETE_TEXT} Use {nameof(IssueCustomFields)} instead.")]
        [JsonIgnore]
        public IList<IssueCustomField> CustomFields
        {
            get => IssueCustomFields;
            set => IssueCustomFields = (List<IssueCustomField>)value;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Sent to Redmine when creating or updating, only when set. Only their Ids are sent. Only sent when creating.</remarks>
        public List<IssueCustomField> IssueCustomFields { get; set; }
        
        /// <summary>
        /// Obsolete. When read from Redmine, holds only the Id and Name of each project custom field (no values).
        /// Use <see cref="ProjectCustomFields"/> instead.
        /// </summary>
        /// <remarks>Sent to Redmine when creating or updating, only when set.</remarks>
        [Obsolete(RedmineConstants.OBSOLETE_TEXT + " Use ProjectCustomFields instead.")]
        public List<CustomField> CustomFieldValues { get; set; }

        /// <summary>
        /// Gets or sets the project custom fields and their values.
        /// </summary>
        /// <value>
        /// The project custom fields.
        /// </value>
        /// <remarks>Sent to Redmine when creating or updating, only when set.</remarks>
        public List<IssueCustomField> ProjectCustomFields { get; set; }

        /// <summary>
        /// Gets the issue categories.
        /// </summary>
        /// <value>
        /// The issue categories.
        /// </value>
        /// <remarks>
        /// <para>Available in Redmine starting with the 2.6.0 version.</para>
        /// <para>Read-only: set by Redmine and not sent when creating or updating.</para>
        /// </remarks>
        public IList<ProjectIssueCategory> IssueCategories { get; set; }

        /// <summary>
        /// Gets the time entry activities.
        /// </summary>
        /// <remarks>
        /// <para>Available in Redmine starting with the 3.4.0 version.</para>
        /// <para>Read-only: set by Redmine and not sent when creating or updating.</para>
        /// </remarks>
        public IList<ProjectTimeEntryActivity> TimeEntryActivities { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Sent to Redmine when creating or updating, only when set. Only its Id is sent.</remarks>
        public IdentifiableName DefaultVersion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Sent to Redmine when creating or updating, only when set. Only its Id is sent.</remarks>
        public IdentifiableName DefaultAssignee { get; set; }
        #endregion

        #region Implementation of IXmlSerializer
        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
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
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.CUSTOM_FIELDS: ProjectCustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.ENABLED_MODULES: EnabledModules = reader.ReadElementContentAsCollection<ProjectEnabledModule>(); break;
                    case RedmineKeys.HOMEPAGE: HomePage = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.IDENTIFIER: Identifier = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.INHERIT_MEMBERS: InheritMembers = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.IS_PUBLIC: IsPublic = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.ISSUE_CATEGORIES: IssueCategories = reader.ReadElementContentAsCollection<ProjectIssueCategory>(); break;
                    case RedmineKeys.ISSUE_CUSTOM_FIELDS: IssueCustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.NAME: Name = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.PARENT: Parent = new IdentifiableName(reader); break;
                    case RedmineKeys.STATUS: Status = (ProjectStatus)reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.TIME_ENTRY_ACTIVITIES: TimeEntryActivities = reader.ReadElementContentAsCollection<ProjectTimeEntryActivity>(); break;
                    case RedmineKeys.TRACKERS: Trackers = reader.ReadElementContentAsCollection<ProjectTracker>(); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.DEFAULT_ASSIGNEE: DefaultAssignee = new IdentifiableName(reader); break;
                    case RedmineKeys.DEFAULT_VERSION: DefaultVersion = new IdentifiableName(reader); break;
                    default: reader.Skip(); break;
                }
            }

            SetCustomFieldValues();
        }

        /// <summary>
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.NAME, Name);
            writer.WriteElementString(RedmineKeys.IDENTIFIER, Identifier);
            writer.WriteIfNotDefaultOrNull(RedmineKeys.DESCRIPTION, Description);
            writer.WriteIfNotDefaultOrNull(RedmineKeys.HOMEPAGE, HomePage);
            writer.WriteBoolean(RedmineKeys.INHERIT_MEMBERS, InheritMembers);
            writer.WriteBoolean(RedmineKeys.IS_PUBLIC, IsPublic);
            writer.WriteIdIfNotNull(RedmineKeys.PARENT_ID, Parent);

            //It works only when the new project is a subproject and it inherits the members. 
            writer.WriteIdIfNotNull(RedmineKeys.DEFAULT_ASSIGNED_TO_ID, DefaultAssignee);
            //It works only with existing shared versions.
            writer.WriteIdIfNotNull(RedmineKeys.DEFAULT_VERSION_ID, DefaultVersion);

            writer.WriteRepeatableElement(RedmineKeys.TRACKER_IDS, RedmineKeys.TRACKER_ID, (IEnumerable<IValue>)Trackers);
            writer.WriteRepeatableElement(RedmineKeys.ENABLED_MODULE_NAMES, RedmineKeys.ENABLED_MODULE_NAME,(IEnumerable<IValue>)EnabledModules);

            if (Id == 0)
            {
                writer.WriteRepeatableElement(RedmineKeys.ISSUE_CUSTOM_FIELD_IDS, RedmineKeys.ISSUE_CUSTOM_FIELD_ID, (IEnumerable<IValue>)IssueCustomFields);
            }

            if (ProjectCustomFields != null && ProjectCustomFields.Count > 0)
            {
                writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, ProjectCustomFields);
            }
            else if (CustomFieldValues != null && CustomFieldValues.Count > 0)
            {
                writer.WriteStartElement(RedmineKeys.CUSTOM_FIELDS);
                writer.WriteAttributeString(RedmineKeys.TYPE, "array");

                foreach (var customField in CustomFieldValues)
                {
                    if (customField?.PossibleValues != null && customField.PossibleValues.Count > 0)
                    {
                        writer.WriteStartElement(RedmineKeys.CUSTOM_FIELD);
                        writer.WriteAttributeString(RedmineKeys.ID, customField.Id.ToInvariantString());
                        if (customField.PossibleValues.Count > 1)
                        {
                            writer.WriteArray(RedmineKeys.VALUE, RedmineKeys.VALUE, customField.PossibleValues, possibleValue => possibleValue?.Value);
                        }
                        else
                        {
                            writer.WriteElementString(RedmineKeys.VALUE, customField.PossibleValues[0]?.Value);
                        }

                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
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
                    break;
                }

                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                switch (reader.Value)
                {
                    case RedmineKeys.ID: Id = reader.ReadAsInt(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.CUSTOM_FIELDS: ProjectCustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.DESCRIPTION: Description = reader.ReadAsString(); break;
                    case RedmineKeys.ENABLED_MODULES: EnabledModules = reader.ReadAsCollection<ProjectEnabledModule>(); break;
                    case RedmineKeys.HOMEPAGE: HomePage = reader.ReadAsString(); break;
                    case RedmineKeys.IDENTIFIER: Identifier = reader.ReadAsString(); break;
                    case RedmineKeys.INHERIT_MEMBERS: InheritMembers = reader.ReadAsBool(); break;
                    case RedmineKeys.IS_PUBLIC: IsPublic = reader.ReadAsBool(); break;
                    case RedmineKeys.ISSUE_CATEGORIES: IssueCategories = reader.ReadAsCollection<ProjectIssueCategory>(); break;
                    case RedmineKeys.ISSUE_CUSTOM_FIELDS: IssueCustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.NAME: Name = reader.ReadAsString(); break;
                    case RedmineKeys.PARENT: Parent = new IdentifiableName(reader); break;
                    case RedmineKeys.STATUS: Status = (ProjectStatus)reader.ReadAsInt(); break;
                    case RedmineKeys.TIME_ENTRY_ACTIVITIES: TimeEntryActivities = reader.ReadAsCollection<ProjectTimeEntryActivity>(); break;
                    case RedmineKeys.TRACKERS: Trackers = reader.ReadAsCollection<ProjectTracker>(); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.DEFAULT_ASSIGNEE: DefaultAssignee = new IdentifiableName(reader); break;
                    case RedmineKeys.DEFAULT_VERSION: DefaultVersion = new IdentifiableName(reader); break;
                    default: reader.Skip(); break;
                }
            }

            SetCustomFieldValues();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteJson(JsonWriter writer)
        {
            using (new JsonObject(writer, RedmineKeys.PROJECT))
            {
                writer.WriteProperty(RedmineKeys.NAME, Name);
                writer.WriteProperty(RedmineKeys.IDENTIFIER, Identifier);
                writer.WriteIfNotDefaultOrNull(RedmineKeys.DESCRIPTION, Description);
                writer.WriteIfNotDefaultOrNull(RedmineKeys.HOMEPAGE, HomePage);
                writer.WriteBoolean(RedmineKeys.INHERIT_MEMBERS, InheritMembers);
                writer.WriteBoolean(RedmineKeys.IS_PUBLIC, IsPublic);
                writer.WriteIdIfNotNull(RedmineKeys.PARENT_ID, Parent);
                
                //It works only when the new project is a subproject and it inherits the members. 
                writer.WriteIdIfNotNull(RedmineKeys.DEFAULT_ASSIGNED_TO_ID, DefaultAssignee);
                //It works only with existing shared versions.
                writer.WriteIdIfNotNull(RedmineKeys.DEFAULT_VERSION_ID, DefaultVersion);
                
                writer.WriteArrayIds(RedmineKeys.TRACKER_IDS, (IEnumerable<IdentifiableName>)Trackers);
                writer.WriteArrayNames(RedmineKeys.ENABLED_MODULE_NAMES, (IEnumerable<IdentifiableName>)EnabledModules);
               
                if (Id == 0)
                {
                    writer.WriteArrayIds(RedmineKeys.ISSUE_CUSTOM_FIELD_IDS, (IEnumerable<IdentifiableName>)IssueCustomFields);
                }

                if (ProjectCustomFields != null && ProjectCustomFields.Count > 0)
                {
                    writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, ProjectCustomFields);
                }
                else if (CustomFieldValues != null && CustomFieldValues.Count > 0)
                {
                    writer.WritePropertyName(RedmineKeys.CUSTOM_FIELD_VALUES);
                    writer.WriteStartObject();

                    foreach (var customField in CustomFieldValues)
                    {
                        if (customField?.PossibleValues != null && customField.PossibleValues.Count > 0)
                        {
                            if (customField.PossibleValues.Count > 1)
                            {
                                writer.WritePropertyName(customField.Id.ToInvariantString());
                                writer.WriteStartArray();
                                foreach (var customFieldPossibleValue in customField.PossibleValues)
                                {
                                    writer.WriteValue(customFieldPossibleValue?.Value);
                                }
                                writer.WriteEndArray();
                            }
                            else
                            {
                                writer.WriteProperty(customField.Id.ToInvariantString(), customField.PossibleValues[0]?.Value);
                            }
                        }
                    }
                    
                    writer.WriteEndObject();
                }
            }
        }
        #endregion

        private void SetCustomFieldValues()
        {
            if (ProjectCustomFields == null)
            {
                return;
            }

            CustomFieldValues = new List<CustomField>(ProjectCustomFields.Count);
            foreach (var projectCustomField in ProjectCustomFields)
            {
                CustomFieldValues.Add(Create<CustomField>(projectCustomField.Id, projectCustomField.Name));
            }
        }

        #region Implementation of IEquatable<Project>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Project other)
        {
            if (other == null)
            {
                return false;
            }

            return base.Equals(other)
                   && string.Equals(Identifier, other.Identifier, StringComparison.Ordinal)
                   && string.Equals(Description, other.Description, StringComparison.Ordinal)
                   && string.Equals(HomePage, other.HomePage, StringComparison.Ordinal)
                   && CreatedOn == other.CreatedOn
                   && UpdatedOn == other.UpdatedOn
                   && Status == other.Status
                   && IsPublic == other.IsPublic
                   && InheritMembers == other.InheritMembers
                   && DefaultAssignee == other.DefaultAssignee
                   && DefaultVersion == other.DefaultVersion
                   && Parent == other.Parent
                   && (Trackers?.Equals<ProjectTracker>(other.Trackers) ?? other.Trackers == null)
                   && (IssueCustomFields?.Equals<IssueCustomField>(other.IssueCustomFields) ?? other.IssueCustomFields == null)
                   && (CustomFieldValues?.Equals<CustomField>(other.CustomFieldValues) ?? other.CustomFieldValues == null)
                   && (ProjectCustomFields?.Equals<IssueCustomField>(other.ProjectCustomFields) ?? other.ProjectCustomFields == null)
                   && (IssueCategories?.Equals<ProjectIssueCategory>(other.IssueCategories) ?? other.IssueCategories == null)
                   && (EnabledModules?.Equals<ProjectEnabledModule>(other.EnabledModules) ?? other.EnabledModules == null)
                   && (TimeEntryActivities?.Equals<ProjectTimeEntryActivity>(other.TimeEntryActivities) ?? other.TimeEntryActivities == null);
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
            return Equals(obj as Project);
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
                hashCode = HashCodeHelper.GetHashCode(IssueCustomFields, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CustomFieldValues, hashCode);
                hashCode = HashCodeHelper.GetHashCode(ProjectCustomFields, hashCode);
                hashCode = HashCodeHelper.GetHashCode(IssueCategories, hashCode);
                hashCode = HashCodeHelper.GetHashCode(EnabledModules, hashCode);
                hashCode = HashCodeHelper.GetHashCode(TimeEntryActivities, hashCode);
                hashCode = HashCodeHelper.GetHashCode(DefaultAssignee, hashCode);
                hashCode = HashCodeHelper.GetHashCode(DefaultVersion, hashCode);

                return hashCode;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Project left, Project right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Project left, Project right)
        {
            return !Equals(left, right);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[Project: Id={Id.ToInvariantString()}, Name={Name}, Identifier={Identifier}, Status={Status:G}, IsPublic={IsPublic.ToInvariantString()}]";
    }
}
