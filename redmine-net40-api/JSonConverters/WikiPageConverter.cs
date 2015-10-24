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
    internal class WikiPageConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var tracker = new WikiPage();

                tracker.Id = dictionary.GetValue<int>(RedmineKeys.ID);
                tracker.Author = dictionary.GetValueAsIdentifiableName(RedmineKeys.AUTHOR);
                tracker.Comments = dictionary.GetValue<string>(RedmineKeys.COMMENTS);
                tracker.CreatedOn = dictionary.GetValue<DateTime?>(RedmineKeys.CREATED_ON);
                tracker.Text = dictionary.GetValue<string>(RedmineKeys.TEXT);
                tracker.Title = dictionary.GetValue<string>(RedmineKeys.TITLE);
                tracker.UpdatedOn = dictionary.GetValue<DateTime?>(RedmineKeys.UPDATED_ON);
                tracker.Version = dictionary.GetValue<int>(RedmineKeys.VERSION);
                tracker.Attachments = dictionary.GetValueAsCollection<Attachment>(RedmineKeys.ATTACHMENTS);

                return tracker;
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as WikiPage;
            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.Add(RedmineKeys.TEXT, entity.Text);
                result.Add(RedmineKeys.COMMENTS, entity.Comments);
                result.WriteValueOrEmpty<int>(entity.Version, RedmineKeys.VERSION);

                var root = new Dictionary<string, object>();
                root[RedmineKeys.WIKI_PAGE] = result;
                return root;
            }

            return result;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(WikiPage) }); } }
    }
}
