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
using System.Web.Script.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters
{
    internal class IssueRelationConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var issueRelation = new IssueRelation();

                issueRelation.Id = dictionary.GetValue<int>(RedmineKeys.ID);
                issueRelation.IssueId = dictionary.GetValue<int>(RedmineKeys.ISSUE_ID);
                issueRelation.IssueToId = dictionary.GetValue<int>(RedmineKeys.ISSUE_TO_ID);
                issueRelation.Type = dictionary.GetValue<IssueRelationType>(RedmineKeys.RELATION_TYPE);
                issueRelation.Delay = dictionary.GetValue<int?>(RedmineKeys.DELAY);

                return issueRelation;
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as IssueRelation;
         
            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.Add(RedmineKeys.ISSUE_TO_ID, entity.IssueToId);
                result.Add(RedmineKeys.RELATION_TYPE, entity.Type.ToString());
                if (entity.Type == IssueRelationType.precedes || entity.Type == IssueRelationType.follows)
                    result.WriteValueOrEmpty(entity.Delay, RedmineKeys.DELAY);

                var root = new Dictionary<string, object>();
                root[RedmineKeys.RELATION] = result;
                return root;
            }

            return result;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(IssueRelation) }); } }

        #endregion
    }
}
