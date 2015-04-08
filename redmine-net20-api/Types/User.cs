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
    /// Availability 1.1
    /// </summary>
    [XmlRoot("user")]
    public class User : Identifiable<User>, IXmlSerializable, IEquatable<User>
    {
        /// <summary>
        /// Gets or sets the user login.
        /// </summary>
        /// <value>The login.</value>
        [XmlElement("login")]
        public String Login { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        /// <value>The password.</value>
        [XmlElement("password")]
        public string Password { get; set; }

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
        /// Gets or sets the authentication mode id.
        /// </summary>
        /// <value>
        /// The authentication mode id.
        /// </value>
        [XmlElement("auth_source_id")]
        public Int32? AuthenticationModeId { get; set; }

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
        /// Gets the API key of the user, visible for admins and for yourself (added in 2.3.0)
        /// </summary>
        [XmlElement("api_key")]
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets the status of the user, visible for admins only (added in 2.4.0)
        /// </summary>
        [XmlElement("status")]
        public UserStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        [XmlArray("custom_fields")]
        [XmlArrayItem("custom_field")]
        public List<IssueCustomField> CustomFields { get; set; }

        /// <summary>
        /// Gets or sets the memberships.
        /// </summary>
        /// <value>
        /// The memberships.
        /// </value>
        [XmlArray("memberships")]
        [XmlArrayItem("membership")]
        public List<Membership> Memberships { get; set; }

        /// <summary>
        /// Gets or sets the user's groups.
        /// </summary>
        /// <value>
        /// The groups.
        /// </value>
        [XmlArray("groups")]
        [XmlArrayItem("group")]
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
                    case "id": Id = reader.ReadElementContentAsInt(); break;

                    case "login": Login = reader.ReadElementContentAsString(); break;

                    case "firstname": FirstName = reader.ReadElementContentAsString(); break;

                    case "lastname": LastName = reader.ReadElementContentAsString(); break;

                    case "mail": Email = reader.ReadElementContentAsString(); break;

                    case "auth_source_id": AuthenticationModeId = reader.ReadElementContentAsNullableInt(); break;

                    case "last_login_on": LastLoginOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case "created_on": CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case "api_key": ApiKey = reader.ReadElementContentAsString(); break;

                    case "status":Status = (UserStatus)reader.ReadElementContentAsInt(); break;

                    case "custom_fields": CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;

                    case "memberships": Memberships = reader.ReadElementContentAsCollection<Membership>(); break;

                    case "groups": Groups = reader.ReadElementContentAsCollection<UserGroup>(); break;

                    default: reader.Read(); break;
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("login", Login);
            writer.WriteElementString("firstname", FirstName);
            writer.WriteElementString("lastname", LastName);
            writer.WriteElementString("mail", Email);
            writer.WriteElementString("password", Password);
            writer.WriteElementString("auth_source_id", AuthenticationModeId.ToString());

            if (CustomFields != null)
            {
                writer.WriteStartElement("custom_fields");
                writer.WriteAttributeString("type", "array");
                foreach (var cf in CustomFields)
                {
                    new XmlSerializer(cf.GetType()).Serialize(writer, cf);
                }
                writer.WriteEndElement();
            }
        }

        public bool Equals(User other)
        {
            if (other == null) return false;
            return (Id == other.Id && Login == other.Login && Password == other.Password
                && FirstName == other.FirstName && LastName == other.LastName && Email == other.Email
                && CreatedOn == other.CreatedOn && LastLoginOn == other.LastLoginOn
                && CustomFields == other.CustomFields && Memberships == other.Memberships
                && Groups == other.Groups && ApiKey == other.ApiKey);
        }
    }
}