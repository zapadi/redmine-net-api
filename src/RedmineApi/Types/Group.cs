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
/// Availability 2.1
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Group : IdentifiableName, IEquatable<Group>
{
    #region Properties
    /// <summary>
    /// Represents the group's users.
    /// </summary>
    public List<GroupUser> Users { get;  set; }

    /// <summary>
    /// Gets or sets the custom fields.
    /// </summary>
    /// <value>The custom fields.</value>
    public List<IssueCustomField> CustomFields { get; set; }

    /// <summary>
    /// Gets or sets the custom fields.
    /// </summary>
    /// <value>The custom fields.</value>
    public List<Membership> Memberships { get; set; }
    #endregion

    #region Implementation of IEquatable<Group>

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public bool Equals(Group other)
    {
        if (other == null) return false;
        return base.Equals(other)
               && Users != null ? Users.Equals<GroupUser>(other.Users) : other.Users == null
                                                                         && CustomFields != null ? CustomFields.Equals<IssueCustomField>(other.CustomFields) : other.CustomFields == null
                                                                                                                                                               && Memberships != null ? Memberships.Equals<Membership>(other.Memberships) : other.Memberships == null;
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
        return Equals(obj as Group);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(Users, hashCode);
        hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Memberships, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Group left, Group right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Group left, Group right)
    {
        return !Equals(left, right);
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[Group: Id={Id.ToInvariantString()}, Name={Name}]";
}