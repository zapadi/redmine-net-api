/*
   Copyright 2011 - 2016 Adrian Popescu.

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
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Redmine.Net.Api.JSonConverters2;
using Redmine.Net.Api.Types;
using Version = Redmine.Net.Api.Types.Version;

namespace Redmine.Net.Api.Internals
{
    internal static partial class RedmineSerializer
    {
        private static readonly Dictionary<Type, IJsonConverter> jsonConverters = new Dictionary<Type, IJsonConverter>
        {
            {typeof (Issue), new IssueConverter()},
            {typeof (UploadConverter), new UploadConverter()},
            {typeof (IssueCustomField), new IssueCustomFieldConverter()}
            //{typeof (Issue), new IssueConverter()},
            //{typeof (Project), new ProjectConverter()},
            //{typeof (User), new UserConverter()},
            //{typeof (UserGroup), new UserGroupConverter()},
            //{typeof (News), new NewsConverter()},
            //{typeof (Query), new QueryConverter()},
            //{typeof (Version), new VersionConverter()},
            //{typeof (Attachment), new AttachmentConverter()},
            //{typeof (Attachments), new AttachmentsConverter()},
            //{typeof (IssueRelation), new IssueRelationConverter()},
            //{typeof (TimeEntry), new TimeEntryConverter()},
            //{typeof (IssueStatus),new IssueStatusConverter()},
            //{typeof (Tracker),new TrackerConverter()},
            //{typeof (TrackerCustomField),new TrackerCustomFieldConverter()},
            //{typeof (IssueCategory), new IssueCategoryConverter()},
            //{typeof (Role), new RoleConverter()},
            //{typeof (ProjectMembership), new ProjectMembershipConverter()},
            //{typeof (Group), new GroupConverter()},
            //{typeof (GroupUser), new GroupUserConverter()},
            //{typeof (Error), new ErrorConverter()},
            //{typeof (IssueCustomField), new IssueCustomFieldConverter()},
            //{typeof (ProjectTracker), new ProjectTrackerConverter()},
            //{typeof (Journal), new JournalConverter()},
            //{typeof (TimeEntryActivity), new TimeEntryActivityConverter()},
            //{typeof (IssuePriority), new IssuePriorityConverter()},
            //{typeof (WikiPage), new WikiPageConverter()},
            //{typeof (Detail), new DetailConverter()},
            //{typeof (ChangeSet), new ChangeSetConverter()},
            //{typeof (Membership), new MembershipConverter()},
            //{typeof (MembershipRole), new MembershipRoleConverter()},
            //{typeof (IdentifiableName), new IdentifiableNameConverter()},
            //{typeof (Permission), new PermissionConverter()},
            //{typeof (IssueChild), new IssueChildConverter()},
            //{typeof (ProjectIssueCategory), new ProjectIssueCategoryConverter()},
            //{typeof (Watcher), new WatcherConverter()},
            //{typeof (Upload), new UploadConverter()},
            //{typeof (ProjectEnabledModule), new ProjectEnabledModuleConverter()},
            //{typeof (CustomField), new CustomFieldConverter()},
            //{typeof (CustomFieldRole), new CustomFieldRoleConverter()},
            //{typeof (CustomFieldPossibleValue), new CustomFieldPossibleValueConverter()}
        };

        public static Dictionary<Type, IJsonConverter> JsonConverters { get { return jsonConverters; } }

        public static string JsonSerializer<T>(T type) where T : new()
        {
            var sb = new StringBuilder();
            
            using (var sw = new StringWriter(sb))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    var converter = jsonConverters[typeof(T)];
                    var serializer = new JsonSerializer();
                    converter.Serialize(writer, type, serializer);

                    return sb.ToString();
                }
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

        /// <summary>
        /// JSON Deserialization
        /// </summary>
        public static List<T> JsonDeserializeToList<T>(string jsonString, string root, out int totalCount) where T : class,new()
        {
            var result = JsonDeserializeToList(jsonString, root, typeof(T), out totalCount);
            return result == null ? null : ((ArrayList)result).OfType<T>().ToList();
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
            if (string.IsNullOrEmpty(jsonString)) return null;

            var serializer = new JsonSerializer();
            var converter = jsonConverters[type];
            var jObject = JObject.Parse(jsonString);
            var rootName = root ?? type.Name.ToLowerInvariant();
            var rootObject = jObject[rootName];

            if (rootObject == null) return null;

            jObject = JObject.Parse(rootObject.ToString());

            return converter.Deserialize(jObject, serializer);
        }

        private static void AddToList(JsonSerializer serializer, IList list, Type type, JToken obj)
        {
            //foreach (var item in obj.ToObject<ArrayList>())
            //{
            //    if (item is ArrayList)
            //    {
            //        AddToList(serializer, list, type, new JToken(item));
            //    }
            //    else
            //    {
            //        var converter = jsonConverters[type];
            //        var o = converter.Deserialize(obj, serializer);

            //        list.Add(o);
            //    }
            //}
        }

        private static object JsonDeserializeToList(string jsonString, string root, Type type, out int totalCount)
        {
            totalCount = 0;

            if (string.IsNullOrEmpty(jsonString)) return null;

            var serializer = new JsonSerializer();
            var converter = jsonConverters[type];
            var jObject = JObject.Parse(jsonString);

            JToken obj, tc;
            
            if (jObject.TryGetValue(RedmineKeys.TOTAL_COUNT, out tc)) totalCount = tc.Value<int>();
            if (!jObject.TryGetValue(root.ToLowerInvariant(), out obj)) return null;

            jObject = JObject.Parse(obj.ToString());

            var result = converter.Deserialize(jObject, serializer);
            var arrayList = new ArrayList();

            if (type == typeof(Error))
            {
                string info = null;
                
                foreach (var item in jObject.ToObject<ArrayList>())
                {
                    var innerArrayList = item as ArrayList;
                    if (innerArrayList != null)
                    {
                        info = innerArrayList.Cast<object>()
                            .Aggregate(info, (current, item2) => current + (item2 as string + " "));
                    }
                    else
                    {
                        info += item as string + " ";
                    }
                }

                var err = new Error { Info = info };
                arrayList.Add(err);
            }
            else
            {
                AddToList(serializer, arrayList, type, obj);
            }

            return arrayList;
        }
    }
}