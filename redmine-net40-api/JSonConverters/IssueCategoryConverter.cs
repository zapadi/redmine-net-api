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
using System.Web.Script.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters
{
    internal class IssueCategoryConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var issueCategory = new IssueCategory();

                issueCategory.Id = dictionary.GetValue<int>("id");
                issueCategory.Project = dictionary.GetValueAsIdentifiableName("project");
                issueCategory.AsignTo = dictionary.GetValueAsIdentifiableName("assigned_to");
                issueCategory.Name = dictionary.GetValue<string>("name");

                return issueCategory;
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as IssueCategory;
            var root = new Dictionary<string, object>();
            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.Add("name", entity.Name);
                result.WriteIdIfNotNull(entity.Project, "project_id");
                result.WriteIdIfNotNull(entity.AsignTo, "assigned_to_id");

                root["issue_category"] = result;
                return root;
            }

            return result;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(IssueCategory) }); } }

        #endregion
    }
}
