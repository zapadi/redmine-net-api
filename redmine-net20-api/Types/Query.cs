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
using System.Xml;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// Availability 1.3
    /// </summary>
    [XmlRoot(RedmineKeys.QUERY)]
    public class Query : IdentifiableName, IEquatable<Query>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is public.
        /// </summary>
        /// <value><c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
        [XmlElement(RedmineKeys.IS_PUBLIC)]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        /// <value>The project id.</value>
        [XmlElement(RedmineKeys.PROJECT_ID)]
        public int? ProjectId { get; set; }

        public override void ReadXml(XmlReader reader)
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

                    case RedmineKeys.NAME: Name = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.IS_PUBLIC: IsPublic = reader.ReadElementContentAsBoolean(); break;

                    case RedmineKeys.PROJECT_ID: ProjectId = reader.ReadElementContentAsNullableInt(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer) { }

        public bool Equals(Query other)
        {
            if (other == null) return false;

            return (other.Id == Id && other.Name == Name && other.IsPublic == IsPublic && other.ProjectId == ProjectId);
        }
    }
}