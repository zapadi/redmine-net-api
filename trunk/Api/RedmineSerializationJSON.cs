/*
   Copyright 2011 - 2013 Adrian Popescu, Dorin Huzum.

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
using Redmine.Net.Api.Types;

//#if RUNNING_ON_35_OR_ABOVE
using System.Linq;
using System.Web.Script.Serialization;
using Redmine.Net.Api.JSonConverters;
using Version = Redmine.Net.Api.Types.Version;

//#endif

namespace Redmine.Net.Api
{
    public static partial class RedmineSerialization
    {
        //  #if RUNNING_ON_35_OR_ABOVE
        private static readonly Dictionary<Type, JavaScriptConverter> converters = new Dictionary<Type, JavaScriptConverter>
        {
            {typeof (Issue), new IssueConverter()},
            {typeof (Project), new ProjectConverter()},
            {typeof (User), new UserConverter()},
            {typeof (News), new NewsConverter()},
            {typeof (Query), new QueryConverter()},
            {typeof (Version), new VersionConverter()},
            {typeof (Attachment), new AttachmentConverter()},
            {typeof (IssueRelation), new IssueRelationConverter()},
            {typeof (TimeEntry), new TimeEntryConverter()},
            {typeof (IssueStatus),new IssueStatusConverter()},
            {typeof (Tracker),new TrackerConverter()},
            {typeof (IssueCategory), new IssueCategoryConverter()},
            {typeof (Role), new RoleConverter()},
            {typeof (ProjectMembership), new ProjectMembershipConverter()},
            {typeof (Group), new GroupConverter()},
            {typeof (Error), new ErrorConverter()},
            {typeof (CustomField), new CustomFieldConverter()},
            {typeof (ProjectTracker), new ProjectTrackerConverter()},
            {typeof (Journal), new JournalConverter()},
            {typeof (TimeEntryActivity), new TimeEntryActivityConverter()},
            {typeof (IssuePriority), new IssuePriorityConverter()},
            {typeof (WikiPage), new WikiPageConverter()},
            {typeof (Detail), new DetailConverter()},
            {typeof (ChangeSet), new ChangeSetConverter()},
            {typeof (Membership), new MembershipConverter()},
            {typeof (MembershipRole), new MembershipRoleConverter()},
            {typeof (UserGroup), new UserGroupConverter()},
            {typeof (IdentifiableName), new IdentifiableNameConverter()},
            {typeof (Permission), new PermissionConverter()},
            {typeof (IssueChild), new IssueChildConverter()},
            {typeof (Watcher), new WatcherConverter()}
        };

        public static Dictionary<Type, JavaScriptConverter> Converters { get { return converters; } }

        // #endif

        //public static Dictionary<string, object> FromJSONToDictionary(string jsonText)
        //{
        //    var jss = new JavaScriptSerializer();
        //    var dictionaryWithStringKey = jss.Deserialize<Dictionary<string, object>>(jsonText);
        //    // var dictionary = dictWithStringKey.ToDictionary(de => jss.ConvertToType<TKey>(de.Key), de => de.Value);
        //    return dictionaryWithStringKey;
        //}

        //public static Dictionary<string, object> FromDictionaryToJSON<TKey, TValue>(Dictionary<TKey, TValue> input)
        //{
        //    var output = new Dictionary<string, object>(input.Count);
        //    foreach (KeyValuePair<TKey, TValue> pair in input)
        //        output.Add(pair.Key.ToString(), pair.Value);
        //    return output;
        //}

        //  #if RUNNING_ON_35_OR_ABOVE
        /// <summary>
        /// JSON Serialization
        /// </summary>
        public static string JsonSerializer<T>(T type) where T : new()
        {
            try
            {
                var ser = new JavaScriptSerializer();
                ser.RegisterConverters(new[] { converters[typeof(T)] });
                var jsonString = ser.Serialize(type);
                return jsonString;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// JSON Deserialization
        /// </summary>
        public static List<T> JsonDeserializeToList<T>(string jsonString, string root) where T : class, new()
        {
            int totalCount;
            return JsonDeserializeToList<T>(jsonString, root, out totalCount);
        }

        public static object JsonDeserializeToList(string jsonString, string root, Type type, out int totalCount)
        {
            totalCount = 0;
            if (String.IsNullOrEmpty(jsonString)) return null;

            var ser = new JavaScriptSerializer();
            ser.RegisterConverters(new[] { converters[type] });
            var dic = ser.Deserialize<Dictionary<string, object>>(jsonString);
            if (dic == null) return null;

            object obj, tc;

            if (dic.TryGetValue("total_count", out tc)) totalCount = (int)tc;

            if (dic.TryGetValue(root.ToLower(), out obj))
            {
                var list = new ArrayList();
                if (type == typeof(Error))
                {
                    string info = null;
                    foreach (var item in (ArrayList)obj)
                    {
                        var arrayList = item as ArrayList;
                        if (arrayList != null)
                        {
                            foreach (var item2 in arrayList) info += item2 as string + " ";
                        }
                        else
                            info += item as string + " ";
                    }
                    var err = new Error { Info = info };
                    list.Add(err);
                }
                else
                {
                    AddToList(ser, list, type, obj);
                }
                return list;
            }
            return null;
        }

        /// <summary>
        /// JSON Deserialization
        /// </summary>
        public static List<T> JsonDeserializeToList<T>(string jsonString, string root, out int totalCount) where T : class,new()
        {
            var result = JsonDeserializeToList(jsonString, root, typeof(T), out totalCount);

            return result == null ? null : ((ArrayList)result).OfType<T>().ToList();
        }

        private static void AddToList(JavaScriptSerializer ser, IList list, Type type, object obj)
        {
            foreach (var item in (ArrayList)obj)
            {
                if (item is ArrayList)
                {
                    AddToList(ser, list, type, item);
                }
                else
                {
                    var o = ser.ConvertToType(item, type);
                    list.Add(o);
                }
            }
        }

        public static T JsonDeserialize<T>(string jsonString, string root) where T : new()
        {
            var type = typeof(T);
            var result = JsonDeserialize(jsonString, type, root);
            if (result == null) return default(T);

            return (T)result;
        }

        public static object JsonDeserialize(string jsonString, Type type, string root)
        {
            if (String.IsNullOrEmpty(jsonString)) return null;

            var ser = new JavaScriptSerializer();
            ser.RegisterConverters(new[] { converters[type] });

            var dic = ser.Deserialize<Dictionary<string, object>>(jsonString);
            if (dic == null) return null;

            object obj;
            if (dic.TryGetValue(root ?? type.Name.ToLower(), out obj))
            {
                var deserializedObject = ser.ConvertToType(obj, type);

                return deserializedObject;
            }
            return null;
        }

        public static List<T> GetValueAsCollection<T>(this IDictionary<string, object> dictionary, string key) where T : new()
        {
            object val;

            if (dictionary.TryGetValue(key, out val))
            {
                var ser = new JavaScriptSerializer();
                ser.RegisterConverters(new[] { Converters[typeof(T)] });

                var result = new List<T>();

                foreach (var item in (ArrayList)val)
                {
                    result.Add(ser.ConvertToType<T>(item));
                }
                return result;
            }

            return null;
        }

        public static IdentifiableName GetValueAsIdentifiableName(this IDictionary<string, object> dictionary, string key)
        {
            object val;

            if (dictionary.TryGetValue(key, out val))
            {
                var ser = new JavaScriptSerializer();
                ser.RegisterConverters(new[] { new IdentifiableNameConverter() });

                var result = ser.ConvertToType<IdentifiableName>(val);
                return result;
            }
            return null;
        }

        //   #endif
    }
}