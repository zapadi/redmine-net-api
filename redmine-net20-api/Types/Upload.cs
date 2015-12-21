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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Support for adding attachments through the REST API is added in Redmine 1.4.0.
    /// </summary>
    [XmlRoot(RedmineKeys.UPLOAD)]
    public class Upload : IEquatable<Upload>
    {
        /// <summary>
        /// Gets or sets the uploaded token.
        /// </summary>
        /// <value>The name of the file.</value>
        [XmlElement(RedmineKeys.TOKEN)]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// Maximum allowed file size (1024000).
        /// </summary>
        /// <value>The name of the file.</value>
        [XmlElement(RedmineKeys.FILENAME)]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [XmlElement(RedmineKeys.CONTENT_TYPE)]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the file description. (Undocumented feature)
        /// </summary>
        /// <value>The file descroütopm.</value>
        [XmlElement(RedmineKeys.DESCRIPTION)]
        public string Description { get; set; }

        public XmlSchema GetSchema() { return null; }

        public bool Equals(Upload other)
        {
            return other != null 
                && Token.Equals(other.Token) 
                && FileName.Equals(other.FileName) 
                && Description.Equals(other.Description)
                && ContentType.Equals(other.ContentType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Upload);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 13;
                hashCode = Utils.GetHashCode(Token, hashCode);
                hashCode = Utils.GetHashCode(FileName, hashCode);
                hashCode = Utils.GetHashCode(Description, hashCode);
                hashCode = Utils.GetHashCode(ContentType, hashCode);

                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Token: {0}, FileName: '{1}', Description: '{2}', ContentType: {3}", Token, FileName, Description, ContentType);
        }
    }
}