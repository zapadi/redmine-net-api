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
    /// Availability 1.1
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.TIME_ENTRY)]
    public sealed class TimeEntry : Identifiable<TimeEntry>, ICloneable
    {
        #region Properties
        private string comments;

        /// <summary>
        /// Gets or sets the issue id to log time on.
        /// </summary>
        /// <value>The issue id.</value>
        public IdentifiableName Issue { get; set; }

        /// <summary>
        /// Gets or sets the project id to log time on.
        /// </summary>
        /// <value>The project id.</value>
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the date the time was spent (default to the current date).
        /// </summary>
        /// <value>The spent on.</value>
        public DateTime? SpentOn { get; set; }

        /// <summary>
        /// Gets or sets the number of spent hours.
        /// </summary>
        /// <value>The hours.</value>
        public decimal Hours { get; set; }

        /// <summary>
        /// Gets or sets the activity id of the time activity. This parameter is required unless a default activity is defined in Redmine.
        /// </summary>
        /// <value>The activity id.</value>
        public IdentifiableName Activity { get; set; }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public IdentifiableName User { get; internal set; }

        /// <summary>
        /// Gets or sets the short description for the entry (255 characters max).
        /// </summary>
        /// <value>The comments.</value>
        public string Comments
        {
            get => comments;
            set => comments = value.Truncate(255);
        }

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
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        public IList<IssueCustomField> CustomFields { get; set; }
        #endregion

        #region Implementation of IXmlSerialization
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
                    case RedmineKeys.ACTIVITY: Activity = new IdentifiableName(reader); break;
                    case RedmineKeys.ACTIVITY_ID: Activity = new IdentifiableName(reader); break;
                    case RedmineKeys.COMMENTS: Comments = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.HOURS: Hours = reader.ReadElementContentAsDecimal(); break;
                    case RedmineKeys.ISSUE_ID: Issue = new IdentifiableName(reader); break;
                    case RedmineKeys.ISSUE: Issue = new IdentifiableName(reader); break;
                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;
                    case RedmineKeys.PROJECT_ID: Project = new IdentifiableName(reader); break;
                    case RedmineKeys.SPENT_ON: SpentOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;
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
            writer.WriteIdIfNotNull(RedmineKeys.ISSUE_ID, Issue);
            writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, Project);
            writer.WriteDateOrEmpty(RedmineKeys.SPENT_ON, SpentOn.GetValueOrDefault(DateTime.Now));
            writer.WriteValueOrEmpty<decimal>(RedmineKeys.HOURS, Hours);
            writer.WriteIdIfNotNull(RedmineKeys.ACTIVITY_ID, Activity);
            writer.WriteElementString(RedmineKeys.COMMENTS, Comments);
            writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, CustomFields);
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
                    case RedmineKeys.ACTIVITY: Activity = new IdentifiableName(reader); break;
                    case RedmineKeys.ACTIVITY_ID: Activity = new IdentifiableName(reader); break;
                    case RedmineKeys.COMMENTS: Comments = reader.ReadAsString(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.HOURS: Hours = reader.ReadAsDecimal().GetValueOrDefault(); break;
                    case RedmineKeys.ISSUE: Issue = new IdentifiableName(reader); break;
                    case RedmineKeys.ISSUE_ID: Issue = new IdentifiableName(reader); break;
                    case RedmineKeys.PROJECT: Project = new IdentifiableName(reader); break;
                    case RedmineKeys.PROJECT_ID: Project = new IdentifiableName(reader); break;
                    case RedmineKeys.SPENT_ON: SpentOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.USER: User = new IdentifiableName(reader); break;
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
            using (new JsonObject(writer, RedmineKeys.TIME_ENTRY))
            {
                writer.WriteIdIfNotNull(RedmineKeys.ISSUE_ID, Issue);
                writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, Project);
                writer.WriteIdIfNotNull(RedmineKeys.ACTIVITY_ID, Activity);
                writer.WriteDateOrEmpty(RedmineKeys.SPENT_ON, SpentOn.GetValueOrDefault(DateTime.Now));
                writer.WriteProperty(RedmineKeys.HOURS, Hours);
                writer.WriteProperty(RedmineKeys.COMMENTS, Comments);
                writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, CustomFields);
            }
        }
        #endregion

        #region Implementation of IEquatable<TimeEntry>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(TimeEntry other)
        {
            if (other == null) return false;
            return Id == other.Id
                && Issue == other.Issue
                && Project == other.Project
                && SpentOn == other.SpentOn
                && Hours == other.Hours
                && Activity == other.Activity
                && Comments == other.Comments
                && User == other.User
                && CreatedOn == other.CreatedOn
                && UpdatedOn == other.UpdatedOn
                && (CustomFields != null ? CustomFields.Equals<IssueCustomField>(other.CustomFields) : other.CustomFields == null);
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
                hashCode = HashCodeHelper.GetHashCode(Issue, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
                hashCode = HashCodeHelper.GetHashCode(SpentOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Hours, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Activity, hashCode);
                hashCode = HashCodeHelper.GetHashCode(User, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Comments, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);
                return hashCode;
            }
        }
        #endregion

        #region Implementation of ICloneable 
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var timeEntry = new TimeEntry
            {
                Activity = Activity,
                Comments = Comments,
                Hours = Hours,
                Issue = Issue,
                Project = Project,
                SpentOn = SpentOn,
                User = User,
                CustomFields = CustomFields
            };
            return timeEntry;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay =>
            $@"[{nameof(TimeEntry)}: {ToString()}, Issue={Issue}, Project={Project}, 
SpentOn={SpentOn?.ToString("u", CultureInfo.InvariantCulture)}, 
Hours={Hours.ToString("F", CultureInfo.InvariantCulture)}, 
Activity={Activity}, 
User={User}, 
Comments={Comments}, 
CreatedOn={CreatedOn?.ToString("u", CultureInfo.InvariantCulture)}, 
UpdatedOn={UpdatedOn?.ToString("u", CultureInfo.InvariantCulture)}, 
CustomFields={CustomFields.Dump()}]";

    }
}