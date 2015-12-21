/*
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
    internal class NewsConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var news = new News();

                news.Id = dictionary.GetValue<int>(RedmineKeys.ID);
                news.Author = dictionary.GetValueAsIdentifiableName(RedmineKeys.AUTHOR);
                news.CreatedOn = dictionary.GetValue<DateTime?>(RedmineKeys.CREATED_ON);
                news.Description = dictionary.GetValue<string>(RedmineKeys.DESCRIPTION);
                news.Project = dictionary.GetValueAsIdentifiableName(RedmineKeys.PROJECT);
                news.Summary = dictionary.GetValue<string>(RedmineKeys.SUMMARY);
                news.Title = dictionary.GetValue<string>(RedmineKeys.TITLE);

                return news;
            }
            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer) { return null; }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(News) }); } }

        #endregion
    }
}
