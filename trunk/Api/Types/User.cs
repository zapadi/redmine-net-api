using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [Serializable]
    [XmlRoot("user")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
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
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        [XmlElement("firstname")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        [XmlElement("lastname")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [XmlElement("mail")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement("created_on")]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the last login on.
        /// </summary>
        /// <value>The last login on.</value>
        [XmlElement("last_login_on")]
        public DateTime LastLoginOn { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        [XmlElement("custom_fields",Type = typeof(CustomField))]
        public IList<CustomField> CustomFields { get; set; }
    }
}