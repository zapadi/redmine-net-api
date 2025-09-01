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
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Internals;

namespace Padi.RedmineApi.Types;

/// <summary>
/// Availability 1.1
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
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
    public DateTime? CreatedOn { get; set; }

    /// <summary>
    /// Gets the last login on.
    /// </summary>
    /// <value>The last login on.</value>
    public DateTime? LastLoginOn { get; set; }

    /// <summary>
    /// Gets the API key of the user, visible for admins and for yourself (added in 2.3.0)
    /// </summary>
    public string ApiKey { get; set; }

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
    public List<Membership> Memberships { get; set; }

    /// <summary>
    /// Gets or sets the user's groups.
    /// </summary>
    /// <value>
    /// The groups.
    /// </value>
    public List<UserGroup> Groups { get; set; }

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
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[User: Id={Id.ToInvariantString()}, Login={Login}, IsAdmin={IsAdmin.ToInvariantString()}, Status={Status:G}]";
}