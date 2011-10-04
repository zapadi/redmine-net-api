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
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [Serializable]
    [XmlRoot("user")]
    public class User
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlElement("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the login.
        /// </summary>
        /// <value>The login.</value>
        [XmlElement("login")]
        public String Login { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        [XmlElement("firstname")]
        public String FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        [XmlElement("lastname")]
        public String LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [XmlElement("mail")]
        public String Email { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement("created_on")]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the last login on.
        /// </summary>
        /// <value>The last login on.</value>
        [XmlElement("last_login_on")]
        public DateTime? LastLoginOn { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        [XmlArray("custom_fields")]
        [XmlArrayItem("custom_field")]
        public List<CustomField> CustomFields { get; set; }

        [XmlArray("memberships")]
        [XmlArrayItem("membership")]
        public List<Membership> Memberships { get; set; }
    }
}