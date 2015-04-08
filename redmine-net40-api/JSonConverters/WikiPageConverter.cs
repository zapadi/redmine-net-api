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
    internal class WikiPageConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var tracker = new WikiPage();

                tracker.Id = dictionary.GetValue<int>("id");
                tracker.Author = dictionary.GetValueAsIdentifiableName("author");
                tracker.Comments = dictionary.GetValue<string>("comments");
                tracker.CreatedOn = dictionary.GetValue<DateTime?>("created_on");
                tracker.Text = dictionary.GetValue<string>("text");
                tracker.Title = dictionary.GetValue<string>("title");
                tracker.UpdatedOn = dictionary.GetValue<DateTime?>("updated_on");
                tracker.Version = dictionary.GetValue<int>("version");
                tracker.Attachments = dictionary.GetValueAsCollection<Attachment>("attachments");

                return tracker;
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as WikiPage;
            var root = new Dictionary<string, object>();
            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.Add("text", entity.Text);
                result.Add("comments", entity.Comments);
                result.WriteIfNotDefaultOrNull<int>(entity.Version, "version");

                root["wiki_page"] = result;
                return root;
            }

            return result;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(WikiPage) }); } }
    }
}
