using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Redmine.Net.Api.JSonConverters;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Extensions
{
    public static class JObjectExtensions
    {
        public static IdentifiableName GetValueAsIdentifiableName(this JObject obj, string key)
        {
            JToken val;

            if (!obj.TryGetValue(key, out val)) return null;

            //var ser = new JavaScriptSerializer();
            //ser.RegisterConverters(new[] { new IdentifiableNameConverter() });

            //var result = ser.ConvertToType<IdentifiableName>(val);
            //return result;

            return val.ToObject<IdentifiableName>();
        }

        public static List<T> GetValueAsCollection<T>(this JObject obj, string key) where T : new()
        {
            JToken val;

            if (!obj.TryGetValue(key, out val)) return null;

            //var ser = new JavaScriptSerializer();
            //ser.RegisterConverters(new[] { RedmineSerializer.JsonConverters[typeof(T)] });

            //var list = new List<T>();

            //var arrayList = val as ArrayList;
            //if (arrayList != null)
            //{
            //    list.AddRange(from object item in arrayList select ser.ConvertToType<T>(item));
            //}
            //else
            //{
            //    var dict = val as Dictionary<string, object>;
            //    if (dict != null)
            //    {
            //        list.AddRange(dict.Select(pair => ser.ConvertToType<T>(pair.Value)));
            //    }
            //}
            //return list;

            return null;
        }
    }
}
