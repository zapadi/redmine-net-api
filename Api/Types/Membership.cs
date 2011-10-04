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
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [Serializable]
    [XmlRoot("membership")]
    public class Membership
    {
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        [XmlElement("project")]
        public IdentifiableName Project { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [XmlArray("roles")]
        [XmlArrayItem("role")]
        public Collection<Role> Type { get; set; }
    }
}