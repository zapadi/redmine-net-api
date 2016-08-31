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
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Collections;
using System.Linq;
using Redmine.Net.Api.JSonConverters;
using Redmine.Net.Api.Types;
using Version = Redmine.Net.Api.Types.Version;

namespace Redmine.Net.Api.Internals
{
    /// <summary>
    /// 
    /// </summary>
    internal static partial class RedmineSerializer
    {
        private static readonly Dictionary<Type, JavaScriptConverter> jsonConverters = new Dictionary<Type, JavaScriptConverter>
        {
            {typeof (Issue), new IssueConverter()},
            {typeof (Project), new ProjectConverter()},
            {typeof (User), new UserConverter()},
            {typeof (UserGroup), new UserGroupConverter()},
            {typeof (News), new NewsConverter()},
            {typeof (Query), new QueryConverter()},
            {typeof (Version), new VersionConverter()},
            {typeof (Attachment), new AttachmentConverter()},
            {typeof (Attachments), new AttachmentsConverter()},
            {typeof (IssueRelation), new IssueRelationConverter()},
            {typeof (TimeEntry), new TimeEntryConverter()},
            {typeof (IssueStatus),new IssueStatusConverter()},
            {typeof (Tracker),new TrackerConverter()},
            {typeof (TrackerCustomField),new TrackerCustomFieldConverter()},
            {typeof (IssueCategory), new IssueCategoryConverter()},
            {typeof (Role), new RoleConverter()},
            {typeof (ProjectMembership), new ProjectMembershipConverter()},
            {typeof (Group), new GroupConverter()},
            {typeof (GroupUser), new GroupUserConverter()},
            {typeof (Error), new ErrorConverter()},
            {typeof (IssueCustomField), new IssueCustomFieldConverter()},
            {typeof (ProjectTracker), new ProjectTrackerConverter()},
            {typeof (Journal), new JournalConverter()},
            {typeof (TimeEntryActivity), new TimeEntryActivityConverter()},
            {typeof (IssuePriority), new IssuePriorityConverter()},
            {typeof (WikiPage), new WikiPageConverter()},
            {typeof (Detail), new DetailConverter()},
            {typeof (ChangeSet), new ChangeSetConverter()},
            {typeof (Membership), new MembershipConverter()},
            {typeof (MembershipRole), new MembershipRoleConverter()},
            {typeof (IdentifiableName), new IdentifiableNameConverter()},
            {typeof (Permission), new PermissionConverter()},
            {typeof (IssueChild), new IssueChildConverter()},
            {typeof (ProjectIssueCategory), new ProjectIssueCategoryConverter()},
            {typeof (Watcher), new WatcherConverter()},
            {typeof (Upload), new UploadConverter()},
            {typeof (ProjectEnabledModule), new ProjectEnabledModuleConverter()},
            {typeof (CustomField), new CustomFieldConverter()},
            {typeof (CustomFieldRole), new CustomFieldRoleConverter()},
            {typeof (CustomFieldPossibleValue), new CustomFieldPossibleValueConverter()}
        };

        /// <summary>
        /// Available json converters.
        /// </summary>
        public static Dictionary<Type, JavaScriptConverter> JsonConverters { get { return jsonConverters; } }

        /// <summary>
        /// Jsons the serializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static string JsonSerializer<T>(T type) where T : new()
        {
            var serializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
            serializer.RegisterConverters(new[] { jsonConverters[typeof(T)] });
            return serializer.Serialize(type);
        }

        /// <summary>
        /// JSON Deserialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">The json string.</param>
        /// <param name="root">The root.</param>
        /// <returns></returns>
        public static List<T> JsonDeserializeToList<T>(string jsonString, string root) where T : class, new()
        {
            int totalCount;
            int offset;
            return JsonDeserializeToList<T>(jsonString, root, out totalCount, out offset);
        }

        /// <summary>
        /// JSON Deserialization
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">The json string.</param>
        /// <param name="root">The root.</param>
        /// <param name="totalCount">The total count.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public static List<T> JsonDeserializeToList<T>(string jsonString, string root, out int totalCount, out int offset) where T : class,new()
        {
            var result = JsonDeserializeToList(jsonString, root, typeof(T), out totalCount, out offset);
            return ((ArrayList)result).OfType<T>().ToList();
        }

        /// <summary>
        /// Jsons the deserialize.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">The json string.</param>
        /// <param name="root">The root.</param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string jsonString, string root) where T : new()
        {
            var type = typeof(T);
            var result = JsonDeserialize(jsonString, type, root);
            return result == null ? default(T) : (T) result;
        }

        /// <summary>
        /// Jsons the deserialize.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <param name="type">The type.</param>
        /// <param name="root">The root.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">jsonString</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static object JsonDeserialize(string jsonString, Type type, string root)
        {
            if (string.IsNullOrEmpty(jsonString)) throw new ArgumentNullException("jsonString");

            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { jsonConverters[type] });

            var dictionary = serializer.Deserialize<Dictionary<string, object>>(jsonString);
            if (dictionary == null) return null;

            object obj;
            return !dictionary.TryGetValue(root ?? type.Name.ToLowerInvariant(), out obj) ? null : serializer.ConvertToType(obj, type);
        }

        /// <summary>
        /// Adds to list.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="list">The list.</param>
        /// <param name="type">The type.</param>
        /// <param name="arrayList">The array list.</param>
        private static void AddToList(JavaScriptSerializer serializer, IList list, Type type, object arrayList)
        {
            foreach (var obj in (ArrayList)arrayList)
            {
                if (obj is ArrayList)
                {
                    AddToList(serializer, list, type, obj);
                }
                else
                {
                    var convertedType = serializer.ConvertToType(obj, type);
                    list.Add(convertedType);
                }
            }
        }

        /// <summary>
        /// Jsons the deserialize to list.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        /// <param name="root">The root.</param>
        /// <param name="type">The type.</param>
        /// <param name="totalCount">The total count.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">jsonString</exception>
        private static object JsonDeserializeToList(string jsonString, string root, Type type, out int totalCount, out int offset)
        {
            totalCount = 0;
            offset = 0;
            if (string.IsNullOrEmpty(jsonString)) throw new ArgumentNullException("jsonString");

            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { jsonConverters[type] });
            var dictionary = serializer.Deserialize<Dictionary<string, object>>(jsonString);
            if (dictionary == null) return null;

            object obj, tc, off;

            if (dictionary.TryGetValue(RedmineKeys.TOTAL_COUNT, out tc)) totalCount = (int)tc;

            if (dictionary.TryGetValue(RedmineKeys.OFFSET, out off)) offset = (int)off;

            if (!dictionary.TryGetValue(root.ToLowerInvariant(), out obj)) return null;
            
            var arrayList = new ArrayList();
            if (type == typeof(Error))
            {
                string info = null;
                foreach (var item in (ArrayList)obj)
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