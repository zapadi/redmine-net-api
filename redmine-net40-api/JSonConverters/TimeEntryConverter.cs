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
using System.Web.Script.Serialization;
using Redmine.Net.Api.Types;
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api.JSonConverters
{
    internal class TimeEntryConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        /// <summary>
        ///     When overridden in a derived class, converts the provided dictionary into an object of the specified type.
        /// </summary>
        /// <param name="dictionary">
        ///     An <see cref="T:System.Collections.Generic.IDictionary`2" /> instance of property data stored
        ///     as name/value pairs.
        /// </param>
        /// <param name="type">The type of the resulting object.</param>
        /// <param name="serializer">The <see cref="T:System.Web.Script.Serialization.JavaScriptSerializer" /> instance.</param>
        /// <returns>
        ///     The deserialized object.
        /// </returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type,
            JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var timeEntry = new TimeEntry();

                timeEntry.Id = dictionary.GetValue<int>(RedmineKeys.ID);
                timeEntry.Activity =
                    dictionary.GetValueAsIdentifiableName(dictionary.ContainsKey(RedmineKeys.ACTIVITY)
                        ? RedmineKeys.ACTIVITY
                        : RedmineKeys.ACTIVITY_ID);
                timeEntry.Comments = dictionary.GetValue<string>(RedmineKeys.COMMENTS);
                timeEntry.Hours = dictionary.GetValue<decimal>(RedmineKeys.HOURS);
                timeEntry.Issue =
                    dictionary.GetValueAsIdentifiableName(dictionary.ContainsKey(RedmineKeys.ISSUE)
                        ? RedmineKeys.ISSUE
                        : RedmineKeys.ISSUE_ID);
                timeEntry.Project =
                    dictionary.GetValueAsIdentifiableName(dictionary.ContainsKey(RedmineKeys.PROJECT)
                        ? RedmineKeys.PROJECT
                        : RedmineKeys.PROJECT_ID);
                timeEntry.SpentOn = dictionary.GetValue<DateTime?>(RedmineKeys.SPENT_ON);
                timeEntry.User = dictionary.GetValueAsIdentifiableName(RedmineKeys.USER);
                timeEntry.CustomFields = dictionary.GetValueAsCollection<IssueCustomField>(RedmineKeys.CUSTOM_FIELDS);
                timeEntry.CreatedOn = dictionary.GetValue<DateTime?>(RedmineKeys.CREATED_ON);
                timeEntry.UpdatedOn = dictionary.GetValue<DateTime?>(RedmineKeys.UPDATED_ON);

                return timeEntry;
            }

            return null;
        }

        /// <summary>
        ///     When overridden in a derived class, builds a dictionary of name/value pairs.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The object that is responsible for the serialization.</param>
        /// <returns>
        ///     An object that contains key/value pairs that represent the object’s data.
        /// </returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as TimeEntry;
            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.WriteIdIfNotNull(entity.Issue, RedmineKeys.ISSUE_ID);
                result.WriteIdIfNotNull(entity.Project, RedmineKeys.PROJECT_ID);
                result.WriteIdIfNotNull(entity.Activity, RedmineKeys.ACTIVITY_ID);

                if (!entity.SpentOn.HasValue) entity.SpentOn = DateTime.Now;

                result.WriteDateOrEmpty(entity.SpentOn, RedmineKeys.SPENT_ON);
                result.Add(RedmineKeys.HOURS, entity.Hours);
                result.Add(RedmineKeys.COMMENTS, entity.Comments);
                result.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields, new IssueCustomFieldConverter(),
                    serializer);

                var root = new Dictionary<string, object>();
                root[RedmineKeys.TIME_ENTRY] = result;
                return root;
            }

            return result;
        }

        /// <summary>
        ///     When overridden in a derived class, gets a collection of the supported types.
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new List<Type>(new[] {typeof(TimeEntry)}); }
        }

        #endregion
    }
}