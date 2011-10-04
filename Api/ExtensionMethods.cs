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

namespace Redmine.Net.Api
{
    public static class ExtensionMethods
    {
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

                var temp = sr.Deserialize(r.ReadSubtree()) as T;
                if (temp != null) result.Add(temp);
            }
            return result;
        }
    }
}