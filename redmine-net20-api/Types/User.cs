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
    /// Availability 1.1
    /// </summary>
    [XmlRoot(RedmineKeys.USER)]
    public class User : Identifiable<User>, IXmlSerializable, IEquatable<User>
    {
        /// <summary>
        /// Gets or sets the user login.
        /// </summary>
        /// <value>The login.</value>
        [XmlElement(RedmineKeys.LOGIN)]
        public String Login { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        /// <value>The password.</value>
        [XmlElement(RedmineKeys.PASSWORD)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        [XmlElement(RedmineKeys.FIRSTNAME)]
        public String FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        [XmlElement(RedmineKeys.LASTNAME)]
        public String LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [XmlElement(RedmineKeys.MAIL)]
        public String Email { get; set; }

        /// <summary>
        /// Gets or sets the authentication mode id.
        /// </summary>
        /// <value>
        /// The authentication mode id.
        /// </value>
        [XmlElement(RedmineKeys.AUTH_SOURCE_ID, IsNullable = true)]
        public Int32? AuthenticationModeId { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement(RedmineKeys.CREATED_ON, IsNullable = true)]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the last login on.
        /// </summary>
        /// <value>The last login on.</value>
        [XmlElement(RedmineKeys.LAST_LOGIN_ON, IsNullable = true)]
        public DateTime? LastLoginOn { get; set; }

        /// <summary>
        /// Gets the API key of the user, visible for admins and for yourself (added in 2.3.0)
        /// </summary>
        [XmlElement(RedmineKeys.API_KEY, IsNullable = true)]
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets the status of the user, visible for admins only (added in 2.4.0)
        /// </summary>
        [XmlElement(RedmineKeys.STATUS, IsNullable = true)]
        public UserStatus Status { get; set; }

        [XmlElement(RedmineKeys.MUST_CHANGE_PASSWD, IsNullable = true)]
        public bool MustChangePassword { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        [XmlArray(RedmineKeys.CUSTOM_FIELDS)]
        [XmlArrayItem(RedmineKeys.CUSTOM_FIELD)]
        public List<IssueCustomField> CustomFields { get; set; }

        /// <summary>
        /// Gets or sets the memberships.
        /// </summary>
        /// <value>
        /// The memberships.
        /// </value>
        [XmlArray(RedmineKeys.MEMBERSHIPS)]
        [XmlArrayItem(RedmineKeys.MEMBERSHIP)]
        public List<Membership> Memberships { get; set; }

        /// <summary>
        /// Gets or sets the user's groups.
        /// </summary>
        /// <value>
        /// The groups.
        /// </value>
        [XmlArray(RedmineKeys.GROUPS)]
        [XmlArrayItem(RedmineKeys.GROUP)]
        public List<UserGroup> Groups { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

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

                    case RedmineKeys.LOGIN: Login = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.FIRSTNAME: FirstName = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.LASTNAME: LastName = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.MAIL: Email = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.MUST_CHANGE_PASSWD: MustChangePassword = reader.ReadElementContentAsBoolean(); break;

                    case RedmineKeys.AUTH_SOURCE_ID: AuthenticationModeId = reader.ReadElementContentAsNullableInt(); break;

                    case RedmineKeys.LAST_LOGIN_ON: LastLoginOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case RedmineKeys.API_KEY: ApiKey = reader.ReadElementContentAsString(); break;

                    case RedmineKeys.STATUS: Status = (UserStatus)reader.ReadElementContentAsInt(); break;

                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;

                    case RedmineKeys.MEMBERSHIPS: Memberships = reader.ReadElementContentAsCollection<Membership>(); break;

                    case RedmineKeys.GROUPS: Groups = reader.ReadElementContentAsCollection<UserGroup>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.LOGIN, Login);
            writer.WriteElementString(RedmineKeys.FIRSTNAME, FirstName);
            writer.WriteElementString(RedmineKeys.LASTNAME, LastName);
            writer.WriteElementString(RedmineKeys.MAIL, Email);
            writer.WriteElementString(RedmineKeys.PASSWORD, Password);
            writer.WriteValueOrEmpty(AuthenticationModeId, RedmineKeys.AUTH_SOURCE_ID);
            writer.WriteElementString(RedmineKeys.MUST_CHANGE_PASSWD, MustChangePassword.ToString());

            writer.WriteArray(CustomFields, RedmineKeys.CUSTOM_FIELDS);
        }

        public bool Equals(User other)
        {
            if (other == null) return false;
            return ( Login == other.Login);
        }
    }
}