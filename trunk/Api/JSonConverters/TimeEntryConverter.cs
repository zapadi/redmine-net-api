/*
   Copyright 2011 - 2012 Adrian Popescu, Dorin Huzum.

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
//#if RUNNING_ON_35_OR_ABOVE
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters
{
    public class TimeEntryConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if ((dictionary != null) && (type == typeof(TimeEntry)))
            {
                var timeEntry = new TimeEntry();

                timeEntry.Id = dictionary.GetValue<int>("id");
                timeEntry.Activity = dictionary.GetValueAsIdentifiableName("activity_id");
                timeEntry.Comments = dictionary.GetValue<string>("comments");
                timeEntry.Hours = dictionary.GetValue<decimal>("hours");
                timeEntry.Issue = dictionary.GetValueAsIdentifiableName("identifier");
                timeEntry.Project = dictionary.GetValueAsIdentifiableName("project_id");
                timeEntry.SpentOn = dictionary.GetValue<DateTime?>("spent_on");
                timeEntry.User = dictionary.GetValueAsIdentifiableName("user");

                return timeEntry;
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as TimeEntry;
            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.WriteIdIfNotNull(entity.Issue,"issue_id");
                result.WriteIdIfNotNull(entity.Project,"project_id");
                result.WriteIdIfNotNull(entity.Activity, "activity_id");
                if (!entity.SpentOn.HasValue) entity.SpentOn = DateTime.Now;
                result.Add("spent_on", entity.SpentOn);
                result.Add("hours", entity.Hours);
                result.Add("comments", entity.Comments);
               
                return result;
            }

            return result;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new List<Type>(new[] { typeof(TimeEntry) }); }
        }

        #endregion
    }
}
//#endif