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
    [XmlRoot(RedmineKeys.WIKI_PAGE)]
    public class WikiPage : Identifiable<WikiPage>, IXmlSerializable, IEquatable<WikiPage>
    {
        [XmlElement(RedmineKeys.TITLE)]
        public string Title { get; set; }

        [XmlElement(RedmineKeys.TEXT)]
        public string Text { get; set; }

        [XmlElement(RedmineKeys.COMMENTS)]
        public string Comments { get; set; }

        [XmlElement(RedmineKeys.VERSION)]
        public int Version { get; set; }

        [XmlElement(RedmineKeys.AUTHOR)]
        public IdentifiableName Author { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement(RedmineKeys.CREATED_ON)]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        [XmlElement(RedmineKeys.UPDATED_ON)]
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        /// <value>
        /// The attachments.
        /// </value>
        [XmlArray(RedmineKeys.ATTACHMENTS)]
        [XmlArrayItem(RedmineKeys.ATTACHMENT)]
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
                    case RedmineKeys.ID: Id = reader.ReadElementContentAsInt(); break;

                    case RedmineKeys.TITLE: Title = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.TEXT: Text = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.COMMENTS: Comments = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.VERSION: Version = reader.ReadElementContentAsInt(); break;

                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.ATTACHMENTS: Attachments = reader.ReadElementContentAsCollection<Attachment>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.TEXT, Text);
            writer.WriteElementString(RedmineKeys.COMMENTS, Comments);
            writer.WriteValueOrEmpty<int>(Version, RedmineKeys.VERSION);
        }

        #endregion

        #region Implementation of IEquatable<WikiPage>

        public bool Equals(WikiPage other)
        {
            if (other == null) return false;

            return Id == other.Id
                && Title == other.Title
                && Text == other.Text
                && Comments == other.Comments
                && Version == other.Version
                && Author == other.Author
                && CreatedOn == other.CreatedOn
                && UpdatedOn == other.UpdatedOn;
        }

        #endregion
    }
}