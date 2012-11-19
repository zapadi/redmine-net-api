using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//#if RUNNING_ON_35_OR_ABOVE
using System.Linq;
using System.Web.Script.Serialization;
using Redmine.Net.Api.JSonConverters;
//#endif
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Types;
using Version = Redmine.Net.Api.Types.Version;

namespace Redmine.Net.Api
{
    public static class RedmineSerialization
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
            {typeof (ProjectMembership), new MembershipConverter()},
            {typeof (Group), new GroupConverter()},
            {typeof (Error), new ErrorConverter()},
            {typeof (CustomField), new CustomFieldConverter()},
            {typeof (ProjectTracker), new ProjectTrackerConverter()},
            {typeof (Journal), new JournalConverter()},
            {typeof (TimeEntryActivity), new TimeEntryActivityConverter()},
            {typeof (IssuePriority), new IssuePriorityConverter()},
            {typeof (WikiPage), new WikiPageConverter()}
        };

        public static Dictionary<Type, JavaScriptConverter> Converters { get { return converters; } }

        // #endif
        /// <summary>
        /// Serializes the specified System.Object and writes the XML document to a string.
        /// </summary>
        /// <typeparam name="T">The type of objects to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The System.String that contains the XML document.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string ToXML<T>(T obj) where T : class
        {
            var xws = new XmlWriterSettings { OmitXmlDeclaration = true };

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, xws))
                {
                    var sr = new XmlSerializer(typeof(T));
                    sr.Serialize(xmlWriter, obj);
                    return stringWriter.ToString();
                }
            }
        }

        /// <summary>
        /// Deserializes the XML document contained by the specific System.String.
        /// </summary>
        /// <typeparam name="T">The type of objects to deserialize.</typeparam>
        /// <param name="xml">The System.String that contains the XML document to deserialize.</param>
        /// <returns>The System.Object being deserialized.</returns>
        /// <exception cref="System.InvalidOperationException"> An error occurred during deserialization. The original exception is available
        /// using the System.Exception.InnerException property.</exception>
        public static T FromXML<T>(string xml) where T : class
        {
            using (var text = new StringReader(xml))
            {
                var sr = new XmlSerializer(typeof(T));
                return sr.Deserialize(text) as T;
            }
        }

        public static object FromXML(string xml, Type type)
        {
            using (var text = new StringReader(xml))
            {
                var sr = new XmlSerializer(type);
                return sr.Deserialize(text);
            }
        }

        //public static string ToJSON(object obj)
        //{
        //    var jss = new JavaScriptSerializer();
        //    return jss.Serialize(obj);
        //}

        //public static string ToJSON(object obj, int recursionDepth)
        //{
        //    var jss = new JavaScriptSerializer { RecursionLimit = recursionDepth };
        //    return jss.Serialize(obj);
        //}

        //public static T FromJSON<T>(string text)
        //{
        //    var serializer = new JavaScriptSerializer();
        //    return serializer.Deserialize<T>(text);
        //}

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
                string jsonString = ser.Serialize(type);
                return jsonString;
            }
            catch (Exception ex)
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

            if (dic.TryGetValue(root, out obj))
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

            return result == null ? null : ((ArrayList) result).OfType<T>().ToList();
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

        public static T JsonDeserialize<T>(string jsonString) where T : new()
        {
            var type = typeof(T);
            var result = JsonDeserialize(jsonString, type);
            if (result == null) return default(T);

            return (T)result;
        }

        public static object JsonDeserialize(string jsonString, Type type)
        {
            if (String.IsNullOrEmpty(jsonString)) return null;

            var ser = new JavaScriptSerializer();
            ser.RegisterConverters(new[] { converters[type] });

            var dic = ser.Deserialize<Dictionary<string, object>>(jsonString);
            if (dic == null) return null;

            object obj;
            if (dic.TryGetValue(type.Name.ToLower(), out obj))
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
                    result.Add(ser.ConvertToType<T>(item));

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