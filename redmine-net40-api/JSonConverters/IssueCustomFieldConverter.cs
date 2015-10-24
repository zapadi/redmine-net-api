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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters
{
    internal class IssueCustomFieldConverter : JavaScriptConverter
    {
        #region Overrides of JavaScriptConverter

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary != null)
            {
                var customField = new IssueCustomField();

                customField.Id = dictionary.GetValue<int>(RedmineKeys.ID);
                customField.Name = dictionary.GetValue<string>(RedmineKeys.NAME);
                customField.Multiple = dictionary.GetValue<bool>(RedmineKeys.MULTIPLE);

                var val = dictionary.GetValue<object>(RedmineKeys.VALUE);

                if (val != null)
                {
                    if (customField.Values == null) customField.Values = new List<CustomFieldValue>();
                    var list = val as ArrayList;
                    if (list != null)
                    {
                        foreach (string value in list)
                        {
                            customField.Values.Add(new CustomFieldValue { Info = value });
                        }
                    }
                    else
                    {
                        customField.Values.Add(new CustomFieldValue { Info = val as string });
                    }
                }
                return customField;
            }

            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var entity = obj as IssueCustomField;

            var result = new Dictionary<string, object>();

            if (entity == null) return result;
            if (entity.Values == null) return null;
            var itemsCount = entity.Values.Count;

            result.Add(RedmineKeys.ID, entity.Id);
            if (itemsCount > 1)
            {
                result.Add(RedmineKeys.VALUE, entity.Values.Select(x => x.Info).ToArray());
            }
            else
            {
                result.Add(RedmineKeys.VALUE, itemsCount > 0 ? entity.Values[0].Info : null);
            }

            return result;
        }

        public override IEnumerable<Type> SupportedTypes { get { return new List<Type>(new[] { typeof(IssueCustomField) }); } }

        #endregion
    }
}
