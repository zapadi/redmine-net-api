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
    /// Availability 1.1
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
    [XmlRoot(RedmineKeys.USER)]
    public sealed class User : Identifiable<User>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the user avatar url.
        /// </summary>
        public string AvatarUrl { get; set; }
        
        /// <summary>
        /// Gets or sets the user login.
        /// </summary>
        /// <value>The login.</value>
        public string Login { get;  set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// twofa_scheme
        /// </summary>
        public string TwoFactorAuthenticationScheme { get; set; }
        
        /// <summary>
        /// Gets or sets the authentication mode id.
        /// </summary>
        /// <value>
        /// The authentication mode id.
        /// </value>
        public int? AuthenticationModeId { get; set; }

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
        /// Gets the API key of the user, visible for admins and for yourself (added in 2.3.0)
        /// </summary>
        public string ApiKey { get; internal set; }

        /// <summary>
        /// Gets the status of the user, visible for admins only (added in 2.4.0)
        /// </summary>
        public UserStatus Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool MustChangePassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool GeneratePassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? PasswordChangedOn { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets the custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        public List<IssueCustomField> CustomFields { get; set; }

        /// <summary>
        /// Gets or sets the memberships.
        /// </summary>
        /// <value>
        /// The memberships.
        /// </value>
        public List<Membership> Memberships { get; internal set; }

        /// <summary>
        /// Gets or sets the user's groups.
        /// </summary>
        /// <value>
        /// The groups.
        /// </value>
        public List<UserGroup> Groups { get; internal set; }

        /// <summary>
        /// Gets or sets the user's mail_notification.
        /// </summary>
        /// <value>
        /// only_my_events, only_assigned, only_owner
        /// </value>
        public string MailNotification { get; set; }
        
        /// <summary>
        /// Send account information to the user
        /// </summary>
        public bool SendInformation { get; set; }
        
        #endregion

        #region Implementation of IXmlSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
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
                    case RedmineKeys.AUTH_SOURCE_ID: AuthenticationModeId = reader.ReadElementContentAsNullableInt(); break;
                    case RedmineKeys.AVATAR_URL: AvatarUrl = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.FIRST_NAME: FirstName = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.GROUPS: Groups = reader.ReadElementContentAsCollection<UserGroup>(); break;
                    case RedmineKeys.LAST_LOGIN_ON: LastLoginOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.LAST_NAME: LastName = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.LOGIN: Login = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.MAIL: Email = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.MAIL_NOTIFICATION: MailNotification = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.MEMBERSHIPS: Memberships = reader.ReadElementContentAsCollection<Membership>(); break;
                    case RedmineKeys.MUST_CHANGE_PASSWORD: MustChangePassword = reader.ReadElementContentAsBoolean(); break;
                    case RedmineKeys.PASSWORD_CHANGED_ON: PasswordChangedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    case RedmineKeys.STATUS: Status = (UserStatus)reader.ReadElementContentAsInt(); break;
                    case RedmineKeys.TWO_FA_SCHEME: TwoFactorAuthenticationScheme = reader.ReadElementContentAsString(); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString(RedmineKeys.LOGIN, Login);
            
            if (!Password.IsNullOrWhiteSpace())
            {
                writer.WriteElementString(RedmineKeys.PASSWORD, Password);
            }
            
            writer.WriteElementString(RedmineKeys.FIRST_NAME, FirstName);
            writer.WriteElementString(RedmineKeys.LAST_NAME, LastName);
            writer.WriteElementString(RedmineKeys.MAIL, Email);
            
            if(AuthenticationModeId.HasValue)
            { 
                writer.WriteValueOrEmpty(RedmineKeys.AUTH_SOURCE_ID, AuthenticationModeId);
            }
            
            if(!MailNotification.IsNullOrWhiteSpace())
            {
                writer.WriteElementString(RedmineKeys.MAIL_NOTIFICATION, MailNotification);
            }

            writer.WriteBoolean(RedmineKeys.MUST_CHANGE_PASSWORD, MustChangePassword);
            writer.WriteBoolean(RedmineKeys.GENERATE_PASSWORD, GeneratePassword);
            writer.WriteBoolean(RedmineKeys.SEND_INFORMATION, SendInformation);
            
            writer.WriteElementString(RedmineKeys.STATUS, ((int)Status).ToInvariantString());
            
            writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, CustomFields);
        }
        #endregion

        #region Implementation of IJsonSerialization
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
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
                    case RedmineKeys.AUTH_SOURCE_ID: AuthenticationModeId = reader.ReadAsInt32(); break;
                    case RedmineKeys.AVATAR_URL: AvatarUrl = reader.ReadAsString(); break;
                    case RedmineKeys.CREATED_ON: CreatedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.CUSTOM_FIELDS: CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                    case RedmineKeys.LAST_LOGIN_ON: LastLoginOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.LAST_NAME: LastName = reader.ReadAsString(); break;
                    case RedmineKeys.LOGIN: Login = reader.ReadAsString(); break;
                    case RedmineKeys.FIRST_NAME: FirstName = reader.ReadAsString(); break;
                    case RedmineKeys.GROUPS: Groups = reader.ReadAsCollection<UserGroup>(); break;
                    case RedmineKeys.MAIL: Email = reader.ReadAsString(); break;
                    case RedmineKeys.MAIL_NOTIFICATION: MailNotification = reader.ReadAsString(); break;
                    case RedmineKeys.MEMBERSHIPS: Memberships = reader.ReadAsCollection<Membership>(); break;
                    case RedmineKeys.MUST_CHANGE_PASSWORD: MustChangePassword = reader.ReadAsBool(); break;
                    case RedmineKeys.PASSWORD_CHANGED_ON: PasswordChangedOn = reader.ReadAsDateTime(); break;
                    case RedmineKeys.STATUS: Status = (UserStatus)reader.ReadAsInt(); break;
                    case RedmineKeys.TWO_FA_SCHEME: TwoFactorAuthenticationScheme = reader.ReadAsString(); break;
                    case RedmineKeys.UPDATED_ON: UpdatedOn = reader.ReadAsDateTime(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void WriteJson(JsonWriter writer)
        {
            using (new JsonObject(writer, RedmineKeys.USER))
            {
                writer.WriteProperty(RedmineKeys.LOGIN, Login);
                
                if (!string.IsNullOrEmpty(Password))
                {
                    writer.WriteProperty(RedmineKeys.PASSWORD, Password);
                }
                
                writer.WriteProperty(RedmineKeys.FIRST_NAME, FirstName);
                writer.WriteProperty(RedmineKeys.LAST_NAME, LastName);
                writer.WriteProperty(RedmineKeys.MAIL, Email);
                
                if(AuthenticationModeId.HasValue)
                { 
                    writer.WriteValueOrEmpty(RedmineKeys.AUTH_SOURCE_ID, AuthenticationModeId);
                }
                
                if(!MailNotification.IsNullOrWhiteSpace())
                {
                    writer.WriteProperty(RedmineKeys.MAIL_NOTIFICATION, MailNotification);
                }
            
                writer.WriteBoolean(RedmineKeys.MUST_CHANGE_PASSWORD, MustChangePassword);
                writer.WriteBoolean(RedmineKeys.GENERATE_PASSWORD, GeneratePassword);
                writer.WriteBoolean(RedmineKeys.SEND_INFORMATION, SendInformation);
                
                writer.WriteProperty(RedmineKeys.STATUS, ((int)Status).ToInvariantString());
            
                writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, CustomFields);
            }
        }
        #endregion

        #region Implementation of IEquatable<User>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(User other)
        {
            if (other == null) return false;
            return Id == other.Id
                && string.Equals(AvatarUrl,other.AvatarUrl, StringComparison.Ordinal)
                && string.Equals(Login,other.Login, StringComparison.Ordinal)
                && string.Equals(FirstName,other.FirstName, StringComparison.Ordinal)
                && string.Equals(LastName,other.LastName, StringComparison.Ordinal)
                && string.Equals(Email,other.Email, StringComparison.Ordinal)
                && string.Equals(MailNotification,other.MailNotification, StringComparison.Ordinal)
                && string.Equals(ApiKey,other.ApiKey, StringComparison.Ordinal)
                && string.Equals(TwoFactorAuthenticationScheme,other.TwoFactorAuthenticationScheme, StringComparison.Ordinal)
                && AuthenticationModeId == other.AuthenticationModeId
                && CreatedOn == other.CreatedOn
                && LastLoginOn == other.LastLoginOn
                && Status == other.Status
                && MustChangePassword == other.MustChangePassword
                && GeneratePassword == other.GeneratePassword
                && SendInformation == other.SendInformation
                && IsAdmin == other.IsAdmin
                && PasswordChangedOn == other.PasswordChangedOn
                && UpdatedOn == other.UpdatedOn
                && CustomFields != null ? CustomFields.Equals<IssueCustomField>(other.CustomFields) : other.CustomFields == null
                && Memberships != null ? Memberships.Equals<Membership>(other.Memberships) : other.Memberships == null
                && Groups != null ? Groups.Equals<UserGroup>(other.Groups) : other.Groups == null;
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
            return Equals(obj as User);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 17;
            hashCode = HashCodeHelper.GetHashCode(Id, hashCode);
            hashCode = HashCodeHelper.GetHashCode(AvatarUrl, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Login, hashCode);
            hashCode = HashCodeHelper.GetHashCode(FirstName, hashCode);
            hashCode = HashCodeHelper.GetHashCode(LastName, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Email, hashCode);
            hashCode = HashCodeHelper.GetHashCode(MailNotification, hashCode);
            hashCode = HashCodeHelper.GetHashCode(ApiKey, hashCode);
            hashCode = HashCodeHelper.GetHashCode(TwoFactorAuthenticationScheme, hashCode);
            hashCode = HashCodeHelper.GetHashCode(AuthenticationModeId, hashCode);
            hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
            hashCode = HashCodeHelper.GetHashCode(LastLoginOn, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Status, hashCode);
            hashCode = HashCodeHelper.GetHashCode(MustChangePassword, hashCode);
            hashCode = HashCodeHelper.GetHashCode(IsAdmin, hashCode);
            hashCode = HashCodeHelper.GetHashCode(PasswordChangedOn, hashCode);
            hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);
            hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Memberships, hashCode);
            hashCode = HashCodeHelper.GetHashCode(Groups, hashCode);
            hashCode = HashCodeHelper.GetHashCode(GeneratePassword, hashCode);
            hashCode = HashCodeHelper.GetHashCode(SendInformation, hashCode);
            return hashCode;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(User left, User right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(User left, User right)
        {
            return !Equals(left, right);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[User: Id={Id.ToInvariantString()}, Login={Login}, IsAdmin={IsAdmin.ToInvariantString()}, Status={Status:G}]";
    }
}