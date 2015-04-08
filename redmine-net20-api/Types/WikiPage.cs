/*
   Copyright 2011 - 2015 Adrian Popescu

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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 2.2
    /// </summary>
    [XmlRoot("wiki_page")]
    public class WikiPage : Identifiable<WikiPage>, IXmlSerializable, IEquatable<WikiPage>
    {
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("text")]
        public string Text { get; set; }

        [XmlElement("comments")]
        public string Comments { get; set; }

        [XmlElement("version")]
        public int Version { get; set; }

        [XmlElement("author")]
        public IdentifiableName Author { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement("created_on")]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        [XmlElement("updated_on")]
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        /// <value>
        /// The attachments.
        /// </value>
        [XmlArray("attachments")]
        [XmlArrayItem("attachment")]
        public IList<Attachment> Attachments { get; set; }

        #region Implementation of IXmlSerializable

        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            reader.Read();
            while (!reader.EOF)
            {
                if (reader.IsEmptyElement && !reader.HasAttributes)
                {
                    reader.Read();
                    continue;
                }

                switch (reader.Name)
                {
                    case "id": Id = reader.ReadElementContentAsInt(); break;

                    case "title": Title = reader.ReadElementContentAsString(); break;

                    case "text": Text = reader.ReadElementContentAsString(); break;
                    
                    case "comments": Comments = reader.ReadElementContentAsString(); break;

                    case "version": Version = reader.ReadElementContentAsInt(); break;

                    case "author": Author = new IdentifiableName(reader); break;

                    case "created_on": CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case "updated_on": UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case "attachments": Attachments = reader.ReadElementContentAsCollection<Attachment>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("text", Text);
            writer.WriteElementString("comments", Comments);
            writer.WriteIfNotDefaultOrNull<int>(Version,"version");
        }

        #endregion

        #region Implementation of IEquatable<WikiPage>

        public bool Equals(WikiPage other)
        {
            if (other == null) return false;

            return Id == other.Id && Title == other.Title && Text == other.Text && Comments == other.Comments && Version == other.Version && Author == other.Author && CreatedOn == other.CreatedOn && UpdatedOn == other.UpdatedOn;
        }

        #endregion
    }
}