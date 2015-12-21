﻿/*
   Copyright 2011 - 2015 Adrian Popescu.

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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.3
    /// </summary>
    [XmlRoot(RedmineKeys.ATTACHMENT)]
    public class Attachment : Identifiable<Attachment>, IXmlSerializable, IEquatable<Attachment>
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [XmlElement(RedmineKeys.FILENAME)]
        public String FileName { get; set; }

        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        /// <value>The size of the file.</value>
        [XmlElement(RedmineKeys.FILESIZE)]
        public int FileSize { get; set; }

        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>The type of the content.</value>
        [XmlElement(RedmineKeys.CONTENT_TYPE)]
        public String ContentType { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlElement(RedmineKeys.DESCRIPTION)]
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the content URL.
        /// </summary>
        /// <value>The content URL.</value>
        [XmlElement(RedmineKeys.CONTENT_URL)]
        public String ContentUrl { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        [XmlElement(RedmineKeys.AUTHOR)]
        public IdentifiableName Author { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement(RedmineKeys.CREATED_ON)]
        public DateTime? CreatedOn { get; set; }

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

                    case RedmineKeys.FILENAME: FileName = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.FILESIZE: FileSize = reader.ReadElementContentAsInt(); break;

                    case RedmineKeys.CONTENT_TYPE: ContentType = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.AUTHOR: Author = new IdentifiableName(reader); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.DESCRIPTION: Description = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.CONTENT_URL: ContentUrl = reader.ReadElementContentAsString(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer) { }

        public bool Equals(Attachment other)
        {
            if (other == null) return false;
            return (Id == other.Id 
                && FileName == other.FileName 
                && FileSize == other.FileSize 
                && ContentType == other.ContentType
                && Author == other.Author
                && CreatedOn == other.CreatedOn 
                && Description == other.Description 
                && ContentUrl == other.ContentUrl);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (string.IsNullOrEmpty(FileName) ? 0 : FileName.GetHashCode());
                hashCode = (hashCode * 397) ^ FileSize.GetHashCode();
                hashCode = (hashCode * 397) ^ (string.IsNullOrEmpty(ContentType) ? 0 : ContentType.GetHashCode());
                hashCode = (hashCode * 397) ^ (Author == null ? 0 : Author.GetHashCode());
                hashCode = (hashCode * 397) ^ (CreatedOn == null ? 0 : CreatedOn.Value.GetHashCode());
                hashCode = (hashCode * 397) ^ (string.IsNullOrEmpty(Description) ? 0 : Description.GetHashCode());
                hashCode = (hashCode * 397) ^ (string.IsNullOrEmpty(ContentUrl) ? 0 : ContentUrl.GetHashCode());

                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, FileName: '{1}', FileSize: {2}, ContentType: '{3}', Author: {{{4}}}, CreatedOn: {5}, Description: '{6}', ContentUrl: '{7}'",
                Id, FileName, FileSize, ContentType, Author, CreatedOn, Description, ContentUrl);
        }
    }
}