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
/// Availability 1.4
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Role : IdentifiableName, IEquatable<Role>
{
    #region Properties
    /// <summary>
    /// Gets the permissions.
    /// </summary>
    /// <value>
    /// The issue relations.
    /// </value>
    public List<Permission> Permissions { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string IssuesVisibility { get; set; }
        
    /// <summary>
    /// 
    /// </summary>
    public string TimeEntriesVisibility { get; set; }
        
    /// <summary>
    /// 
    /// </summary>
    public string UsersVisibility { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? IsAssignable { get; set; }
    #endregion

    #region Implementation of IEquatable<Role>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Role other)
    {
        if (other == null) return false;
        return Id == other.Id 
               && string.Equals(Name, other.Name, StringComparison.Ordinal) 
               && IsAssignable == other.IsAssignable 
               && IssuesVisibility == other.IssuesVisibility 
               && TimeEntriesVisibility == other.TimeEntriesVisibility
               && UsersVisibility == other.UsersVisibility 
               && Permissions != null ? Permissions.Equals<Permission>(other.Permissions) : other.Permissions == null;

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
        return Equals(obj as Role);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(IsAssignable, hashCode);
        hashCode = HashCodeHelper.GetHashCode(IssuesVisibility, hashCode);
        hashCode = HashCodeHelper.GetHashCode(TimeEntriesVisibility, hashCode);
        hashCode = HashCodeHelper.GetHashCode(UsersVisibility, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Permissions, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Role left, Role right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Role left, Role right)
    {
        return !Equals(left, right);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[Role: Id={Id.ToInvariantString()}, Name={Name}]";

}