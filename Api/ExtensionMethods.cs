/*
   Copyright 2011 Dorin Huzum, Adrian Popescu.

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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api
{
    public static class ExtensionMethods
    {
        public static int ReadAttributeAsInt(this XmlReader reader, string attributeName)
        {
            try
            {
                var attribute = reader.GetAttribute(attributeName);
                int result;
                if (String.IsNullOrWhiteSpace(attribute) || !Int32.TryParse(attribute, out result)) return default(int);

                return result;
            }
            catch
            {
                return -1;
            }
        }

        public static bool ReadAttributeAsBoolean(this XmlReader reader, string attributeName)
        {
            try
            {
                var attribute = reader.GetAttribute(attributeName);
                bool result;
                if (String.IsNullOrWhiteSpace(attribute) || !Boolean.TryParse(attribute, out result)) return false;

                return result;
            }
            catch
            {
                return false;
            }
        }

        public static DateTime? ReadElementContentAsNullableDateTime(this XmlReader reader)
        {
            var str = reader.ReadElementContentAsString();

            DateTime result;
            if (String.IsNullOrWhiteSpace(str) || !DateTime.TryParse(str, out result)) return null;

            return result;
        }

        public static float? ReadElementContentAsNullableFloat(this XmlReader reader)
        {
            var str = reader.ReadElementContentAsString();

            float result;
            if (String.IsNullOrWhiteSpace(str) || !float.TryParse(str, out result)) return null;

            return result;
        }

        public static int? ReadElementContentAsNullableInt(this XmlReader reader)
        {
            var str = reader.ReadElementContentAsString();

            int result;
            if (String.IsNullOrWhiteSpace(str) || !int.TryParse(str, out result)) return null;

            return result;
        }

        public static decimal? ReadElementContentAsNullableDecimal(this XmlReader reader)
        {
            var str = reader.ReadElementContentAsString();

            decimal result;
            if (String.IsNullOrWhiteSpace(str) || !decimal.TryParse(str, out result)) return null;

            return result;
        }

        public static List<T> ReadElementContentAsCollection<T>(this XmlReader reader) where T : class
        {
            var result = new List<T>();

            var sr = new XmlSerializer(typeof(T));
            var xml = reader.ReadOuterXml();
            var r = new XmlTextReader(new StringReader(xml));
            r.ReadStartElement();
            while (!r.EOF)
            {
                if (r.NodeType == XmlNodeType.EndElement)
                {
                    r.ReadEndElement();
                    continue;
                }

                var subTree = r.ReadSubtree();
                var temp = sr.Deserialize(subTree) as T;
                if (temp != null) result.Add(temp);

                if (!subTree.EOF) r.Read();
            }
            return result;
        }

        public static void WriteIdIfNotNull(this XmlWriter writer, IdentifiableName ident, String tag)
        {
            if (ident != null)
            {
                writer.WriteElementString(tag, ident.Id.ToString());
            }
        }

        public static void WriteIfNotDefaultOrNull<T>(this XmlWriter writer, T? val, String tag) where T: struct
        {
            if (val.HasValue)
            {
                writer.WriteElementString(tag, val.Value.ToString());
            }
        }
    }
}