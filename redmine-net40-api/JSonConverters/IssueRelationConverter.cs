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
    internal class IssueRelationConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var issueRelation = new IssueRelation();

                issueRelation.Id = dictionary.GetValue<int>("id");
                issueRelation.IssueId = dictionary.GetValue<int>("issue_id");
                issueRelation.IssueToId = dictionary.GetValue<int>("issue_to_id");
                issueRelation.Type = dictionary.GetValue<IssueRelationType>("relation_type");
                issueRelation.Delay = dictionary.GetValue<int?>("delay");

                return issueRelation;
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as IssueRelation;
            var root = new Dictionary<string, object>();
            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.Add("issue_to_id", entity.IssueToId);
                result.Add("relation_type", entity.Type.ToString());
                if (entity.Type == IssueRelationType.precedes || entity.Type == IssueRelationType.follows)
                    result.WriteIfNotDefaultOrNull(entity.Delay, "delay");

                root["relation"] = result;
                return root;
            }

            return result;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(IssueRelation) }); } }

        #endregion
    }
}
