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

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [XmlRoot(RedmineKeys.ERROR)]
    public class Error: IXmlSerializable
    {
        [XmlText]
        public string Info { get; set; }

        public override string ToString()
        {
            return Info;
        }

        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            while (!reader.EOF)
            {
                if (reader.IsEmptyElement && !reader.HasAttributes)
                {
                    reader.Read();
                    continue;
                }

                switch (reader.Name)
                {
                    case RedmineKeys.ERROR: Info = reader.ReadElementContentAsString(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer) { }
    }
}