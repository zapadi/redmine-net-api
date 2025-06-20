/*
   Copyright 2011 - 2025 Adrian Popescu

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
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Serialization.Json;
using Redmine.Net.Api.Serialization.Json.Extensions;
using Redmine.Net.Api.Serialization.Xml.Extensions;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Availability 4.1</remarks>
    [DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
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

        /// <inheritdoc />
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.FIRST_NAME, FirstName);
            writer.WriteElementString(RedmineKeys.LAST_NAME, LastName);
            writer.WriteElementString(RedmineKeys.MAIL, Email);
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

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer)
        {
            using (new JsonObject(writer, RedmineKeys.USER))
            {
                writer.WriteProperty(RedmineKeys.FIRST_NAME, FirstName);
                writer.WriteProperty(RedmineKeys.LAST_NAME, LastName);
                writer.WriteProperty(RedmineKeys.MAIL, Email);
            }
        }

        #endregion

        /// <inheritdoc />
        public override bool Equals(MyAccount other)
        {
            if (other == null) return false;
            return Id == other.Id
                   && string.Equals(Login, other.Login, StringComparison.Ordinal)
                   && string.Equals(FirstName, other.FirstName, StringComparison.Ordinal)
                   && string.Equals(LastName, other.LastName, StringComparison.Ordinal)
                   && string.Equals(ApiKey, other.ApiKey, StringComparison.Ordinal)
                   && string.Equals(Email, other.Email, StringComparison.Ordinal)
                   && IsAdmin == other.IsAdmin
                   && CreatedOn == other.CreatedOn
                   && LastLoginOn == other.LastLoginOn
                   && (CustomFields?.Equals<MyAccountCustomField>(other.CustomFields) ?? other.CustomFields == null);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as MyAccount);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 17;
            hashCode = HashCodeHelper.GetHashCode(Id, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Login, hashCode);
            hashCode = HashCodeHelper.GetHashCode(FirstName, hashCode);
            hashCode = HashCodeHelper.GetHashCode(LastName, hashCode);
            hashCode = HashCodeHelper.GetHashCode(ApiKey, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Email, hashCode);
            hashCode = HashCodeHelper.GetHashCode(IsAdmin, hashCode);
            hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
            hashCode = HashCodeHelper.GetHashCode(LastLoginOn, hashCode);
            hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);
            return hashCode;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(MyAccount left, MyAccount right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(MyAccount left, MyAccount right)
        {
            return !Equals(left, right);
        }

        private string DebuggerDisplay => $"[MyAccount: Id={Id.ToInvariantString()}, Login={Login}, IsAdmin={IsAdmin.ToInvariantString()}]";
    }
}