/*
   Copyright 2011 - 2017 Adrian Popescu.

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
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api.JSonConverters
{
    internal class FileConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        /// <summary>
        ///     When overridden in a derived class, converts the provided dictionary into an object of the specified type.
        /// </summary>
        /// <param name="dictionary">
        ///     An <see cref="T:System.Collections.Generic.IDictionary`2" /> instance of property data stored
        ///     as name/value pairs.
        /// </param>
        /// <param name="type">The type of the resulting object.</param>
        /// <param name="serializer">The <see cref="T:System.Web.Script.Serialization.JavaScriptSerializer" /> instance.</param>
        /// <returns>
        ///     The deserialized object.
        /// </returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type,
            JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var file = new File {  };

                file.Author = dictionary.GetValueAsIdentifiableName(RedmineKeys.AUTHOR);
                file.ContentType = dictionary.GetValue<string>(RedmineKeys.CONTENT_TYPE);
                file.ContentUrl = dictionary.GetValue<string>(RedmineKeys.CONTENT_URL); 
                file.CreatedOn = dictionary.GetValue<DateTime?>(RedmineKeys.CREATED_ON);
                file.Description = dictionary.GetValue<string>(RedmineKeys.DESCRIPTION);
                file.Digest = dictionary.GetValue<string>(RedmineKeys.DIGEST);
                file.Downloads = dictionary.GetValue<int>(RedmineKeys.DOWNLOADS);
                file.Filename = dictionary.GetValue<string>(RedmineKeys.FILENAME);
                file.Filesize = dictionary.GetValue<int>(RedmineKeys.FILESIZE);
                file.Id = dictionary.GetValue<int>(RedmineKeys.ID);
                file.Token = dictionary.GetValue<string>(RedmineKeys.TOKEN);
                var versionId = dictionary.GetValue<int?>(RedmineKeys.VERSION_ID);
                if (versionId.HasValue)
                {
                    file.Version = new IdentifiableName { Id = versionId.Value };
                }
                else
                {
                    file.Version = dictionary.GetValueAsIdentifiableName(RedmineKeys.VERSION);
                }
                return file;
            }

            return null;
        }

        /// <summary>
        ///     When overridden in a derived class, builds a dictionary of name/value pairs.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="serializer">The object that is responsible for the serialization.</param>
        /// <returns>
        ///     An object that contains key/value pairs that represent the object’s data.
        /// </returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as File;
            var result = new Dictionary<string, object>();

            if (entity != null)
            {
                result.Add(RedmineKeys.TOKEN, entity.Token);
                result.WriteIdIfNotNull(entity.Version, RedmineKeys.VERSION_ID);
                result.Add(RedmineKeys.FILENAME, entity.Filename);
                result.Add(RedmineKeys.DESCRIPTION, entity.Description);

                var root = new Dictionary<string, object>();
                root[RedmineKeys.FILE] = result;
                return root;
            }
            return result;
        }

        /// <summary>
        ///     When overridden in a derived class, gets a collection of the supported types.
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new List<Type>(new[] { typeof(File) }); }
        }

        #endregion
    }
}