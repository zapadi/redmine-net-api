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
    internal class UploadConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var upload = new Upload();

                upload.ContentType = dictionary.GetValue<string>(RedmineKeys.CONTENT_TYPE);
                upload.FileName = dictionary.GetValue<string>(RedmineKeys.FILENAME);
                upload.Token = dictionary.GetValue<string>(RedmineKeys.TOKEN);
                upload.Description = dictionary.GetValue<string>(RedmineKeys.DESCRIPTION);
                return upload;
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as Upload;
            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.Add(RedmineKeys.CONTENT_TYPE, entity.ContentType);
                result.Add(RedmineKeys.FILENAME, entity.FileName);
                result.Add(RedmineKeys.TOKEN, entity.Token);
                result.Add(RedmineKeys.DESCRIPTION, entity.Description);
            }

            return result;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(Upload) }); } }

        #endregion
    }
}
