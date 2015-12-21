﻿/*
   Copyright 2011 - 2015 Adrian Popescu.

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
    internal class ProjectConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var project = new Project();

                project.Id = dictionary.GetValue<int>(RedmineKeys.ID);
                project.Description = dictionary.GetValue<string>(RedmineKeys.DESCRIPTION);
                project.HomePage = dictionary.GetValue<string>(RedmineKeys.HOMEPAGE);
                project.Name = dictionary.GetValue<string>(RedmineKeys.NAME);
                project.Identifier = dictionary.GetValue<string>(RedmineKeys.IDENTIFIER);
                project.Status = dictionary.GetValue<ProjectStatus>(RedmineKeys.STATUS);
                project.CreatedOn = dictionary.GetValue<DateTime?>(RedmineKeys.CREATED_ON);
                project.UpdatedOn = dictionary.GetValue<DateTime?>(RedmineKeys.UPDATED_ON);
                project.Trackers = dictionary.GetValueAsCollection<ProjectTracker>(RedmineKeys.TRACKERS);
                project.CustomFields = dictionary.GetValueAsCollection<IssueCustomField>(RedmineKeys.CUSTOM_FIELDS);
                project.IsPublic = dictionary.GetValue<bool>(RedmineKeys.IS_PUBLIC);
                project.Parent = dictionary.GetValueAsIdentifiableName(RedmineKeys.PARENT);
                project.IssueCategories = dictionary.GetValueAsCollection<ProjectIssueCategory>(RedmineKeys.ISSUE_CATEGORIES);
                project.EnabledModules = dictionary.GetValueAsCollection<ProjectEnabledModule>(RedmineKeys.ENABLED_MODULES);
                return project;
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as Project;
            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.Add(RedmineKeys.NAME, entity.Name);
                result.Add(RedmineKeys.IDENTIFIER, entity.Identifier);
                result.Add(RedmineKeys.DESCRIPTION, entity.Description);
                result.Add(RedmineKeys.HOMEPAGE, entity.HomePage);
                result.Add(RedmineKeys.INHERIT_MEMBERS, entity.InheritMembers);
                result.Add(RedmineKeys.IS_PUBLIC, entity.IsPublic);

                result.WriteIdOrEmpty(entity.Parent, RedmineKeys.PARENT_ID, string.Empty);
                result.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields, new IssueCustomFieldConverter(), serializer);

                result.WriteIdsArray(RedmineKeys.TRACKER_IDS, entity.Trackers);
                result.WriteNamesArray(RedmineKeys.ENABLED_MODULE_NAMES, entity.EnabledModules);
              
                var root = new Dictionary<string, object>();
                root[RedmineKeys.PROJECT] = result;
                return root;
            }

            return result;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(Project) }); } }

        #endregion
    }
}
