using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Redmine.Net.Api
{
    public class RedmineSerialization
    {
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

        public static string ToJSON(object obj)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(obj);
        }

        public static string ToJSON(object obj, int recursionDepth)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            jss.RecursionLimit = recursionDepth;
            return jss.Serialize(obj);
        }

        public static T FromJSON<T>(string text)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(text);
        }

        public static Dictionary<string, object> FromJSONToDictionary(string jsonText)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var dictionaryWithStringKey = jss.Deserialize<Dictionary<string, object>>(jsonText);
            // var dictionary = dictWithStringKey.ToDictionary(de => jss.ConvertToType<TKey>(de.Key), de => de.Value);
            return dictionaryWithStringKey;
        }

        public static Dictionary<string, object> FromDictionaryToJSON<TKey, TValue>(Dictionary<TKey, TValue> input)
        {
            var output = new Dictionary<string, object>(input.Count);
            foreach (KeyValuePair<TKey, TValue> pair in input)
                output.Add(pair.Key.ToString(), pair.Value);
            return output;
        }
    }
}