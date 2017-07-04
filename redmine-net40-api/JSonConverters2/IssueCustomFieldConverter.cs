using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.JSonConverters2
{
    public class IssueCustomFieldConverter : IJsonConverter
    {
        public void Serialize(JsonWriter writer, object obj, JsonSerializer serializer)
        {
            if (obj == null) return;

            var item = (IssueCustomField)obj;

            if (item.Values == null) return;

            var count = item.Values.Count;

            if (count > 1)
            {
                writer.WriteProperty(RedmineKeys.VALUE, item.Values.Select(x => x.Info).ToArray());
            }
            else
            {
                writer.WriteProperty(RedmineKeys.VALUE, count > 0 ? item.Values[0].Info : null);
            }
        }

        public object Deserialize(JObject obj, JsonSerializer serializer)
        {
            if (obj == null) return null;

            var customField = new IssueCustomField
            {
                Id = obj.Value<int>(RedmineKeys.ID),
                Name = obj.Value<string>(RedmineKeys.NAME),
                Multiple = obj.Value<bool>(RedmineKeys.MULTIPLE)
            };

            var val = obj.Value<object>(RedmineKeys.VALUE);
            var items = serializer.Deserialize(new JTokenReader(val.ToString()), typeof(List<CustomFieldValue>)) as List<CustomFieldValue>;

            customField.Values = items;

            if (items == null) return customField;
            if (customField.Values == null) customField.Values = new List<CustomFieldValue>();
            
            var list = val as ArrayList;
            
            if (list != null)
            {
                foreach (string value in list)
                {
                    customField.Values.Add(new CustomFieldValue {Info = value});
                }
            }
            else
            {
                customField.Values.Add(new CustomFieldValue {Info = val as string});
            }

            return customField;
        }
    }
}
