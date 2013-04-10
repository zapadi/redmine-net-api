/*
   Copyright 2011 - 2013 Adrian Popescu, Dorin Huzum.

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
    public class ProjectConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var project = new Project();

                project.Id = dictionary.GetValue<int>("id");
                project.Description = dictionary.GetValue<string>("description");
                project.HomePage = dictionary.GetValue<string>("homepage");
                project.Name = dictionary.GetValue<string>("name");
                project.Identifier = dictionary.GetValue<string>("identifier");
                project.CreatedOn = dictionary.GetValue<DateTime?>("created_on");
                project.UpdatedOn = dictionary.GetValue<DateTime?>("updated_on");
                project.Trackers = dictionary.GetValueAsCollection<ProjectTracker>("trackers");
                project.CustomFields = dictionary.GetValueAsCollection<CustomField>("custom_fields");
                project.Parent = dictionary.GetValueAsIdentifiableName("parent");
                project.IssueCategories = dictionary.GetValueAsCollection<ProjectIssueCategory>("issue_categories");
                return project;
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as Project;
            var root = new Dictionary<string, object>();
            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.Add("name", entity.Name);
                result.Add("identifier", entity.Identifier);
                result.Add("description", entity.Description);
                if (entity.Id != 0)
                {
                    if (entity.Parent != null)
                        result.Add("parent_id", entity.Parent.Id);
                    result.Add("homepage", entity.HomePage);
                }

                root["project"] = result;
                return root;
            }

            return result;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(Project) }); } }

        #endregion
    }
}
//#endif