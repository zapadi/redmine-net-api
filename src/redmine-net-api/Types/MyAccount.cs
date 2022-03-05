/*
   Copyright 2011 - 2022 Adrian Popescu

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
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Availability 4.1</remarks>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [XmlRoot(RedmineKeys.USER)]
    public sealed class MyAccount : Identifiable<MyAccount>
    {
        #region Properties

        /// <summary>
        /// Gets the user login.
        /// </summary>
        /// <value>The login.</value>
        public string Login { get; internal set; }

        /// <summary>
        /// Gets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; internal set; }

        /// <summary>
        /// Gets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; internal set; }

        /// <summary>
        /// Gets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; internal set; }

        /// <summary>
        /// Returns true if user is admin.
        /// </summary>
        /// <value>
        /// The authentication mode id.
        /// </value>
        public bool IsAdmin { get; internal set; }

        /// <summary>
        /// Gets the created on.
        /// </summary>
        /// <value>The created on.</value>
        public DateTime? CreatedOn { get; internal set; }

        /// <summary>
        /// Gets the last login on.
        /// </summary>
        /// <value>The last login on.</value>
        public DateTime? LastLoginOn { get; internal set; }

        /// <summary>
        /// Gets the API key
        /// </summary>
        public string ApiKey { get; internal set; }
        
        /// <summary>
        /// Gets or sets the custom fields
        /// </summary>
        public List<MyAccountCustomField> CustomFields { get;  set; }
        
        #endregion
        
        #region Implementation of IXmlSerializable

        /// <inheritdoc />
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
                    case RedmineKeys.ADMIN: IsAdmin = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.API_KEY: ApiKey = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadElementContentAsCollection<MyAccountCustomField>(); break;
                    case RedmineKeys.FIRST_NAME: FirstName = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.LAST_LOGIN_ON: LastLoginOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.LAST_NAME: LastName = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.LOGIN: Login = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.MAIL: Email = reader.ReadElementContentAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }
        
        #endregion
        
        #region Implementation of IJsonSerializable

        /// <inheritdoc />
        public override void ReadJson(JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    return;
                }

                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                switch (reader.Value)
                {
                    case RedmineKeys.ID: Id = reader.ReadAsInt(); break;
                    case RedmineKeys.ADMIN: IsAdmin = reader.ReadAsBool(); break;
                    case RedmineKeys.API_KEY: ApiKey = reader.ReadAsString(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadAsCollection<MyAccountCustomField>(); break;
                    case RedmineKeys.FIRST_NAME: FirstName = reader.ReadAsString(); break;
                    case RedmineKeys.LAST_LOGIN_ON: LastLoginOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.LAST_NAME: LastName = reader.ReadAsString(); break;
                    case RedmineKeys.LOGIN: Login = reader.ReadAsString(); break;
                    case RedmineKeys.MAIL: Email = reader.ReadAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }
        
        #endregion

        /// <inheritdoc />
        public override bool Equals(MyAccount other)
        {
            if (other == null) return false;
            return Id == other.Id
                   && string.Equals(Login, other.Login, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(FirstName, other.FirstName, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(LastName, other.LastName, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(ApiKey, other.ApiKey, StringComparison.OrdinalIgnoreCase)
                   && IsAdmin == other.IsAdmin
                   && CreatedOn == other.CreatedOn
                   && LastLoginOn == other.LastLoginOn
                   && (CustomFields?.Equals<MyAccountCustomField>(other.CustomFields) ?? other.CustomFields == null);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = HashCodeHelper.GetHashCode(Login, hashCode);
                hashCode = HashCodeHelper.GetHashCode(FirstName, hashCode);
                hashCode = HashCodeHelper.GetHashCode(LastName, hashCode);
                hashCode = HashCodeHelper.GetHashCode(Email, hashCode);
                hashCode = HashCodeHelper.GetHashCode(IsAdmin, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(LastLoginOn, hashCode);
                hashCode = HashCodeHelper.GetHashCode(ApiKey, hashCode);
                hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);
                return hashCode;
            }
        }

        private string DebuggerDisplay => $@"[ {nameof(MyAccount)}:
Id={Id.ToString(CultureInfo.InvariantCulture)},
Login={Login}, 
ApiKey={ApiKey}, 
FirstName={FirstName}, 
LastName={LastName}, 
Email={Email}, 
IsAdmin={IsAdmin.ToString(CultureInfo.InvariantCulture).ToLowerInv()}, 
CreatedOn={CreatedOn?.ToString("u", CultureInfo.InvariantCulture)}, 
LastLoginOn={LastLoginOn?.ToString("u", CultureInfo.InvariantCulture)}, 
CustomFields={CustomFields.Dump()}]";
    }
}