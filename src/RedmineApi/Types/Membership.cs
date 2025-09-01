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

using System.Collections.Generic;
using System.Diagnostics;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Internals;

namespace Padi.RedmineApi.Types;

/// <summary>
/// Only the roles can be updated, the project and the user of a membership are read-only.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Membership : Identifiable<Membership>
{
    #region Properties
    /// <summary>
    /// Gets the group.
    /// </summary>
    public IdentifiableName Group { get; set; }
    /// <summary>
    /// Gets or sets the project.
    /// </summary>
    /// <value>The project.</value>
    public IdentifiableName Project { get; set; }
        
    /// <summary>
    /// Gets the user.
    /// </summary>
    public IdentifiableName User { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public List<MembershipRole> Roles { get; set; }
    #endregion

    #region Implementation of IEquatable<Membership>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(Membership other)
    {
        if (other == null) return false;
        return Id == other.Id
               && Project == other.Project 
               && Roles != null ? Roles.Equals<MembershipRole>(other.Roles) : other.Roles == null;
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
        return Equals(obj as Membership);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(Group, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
        hashCode = HashCodeHelper.GetHashCode(User, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Roles, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Membership left, Membership right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Membership left, Membership right)
    {
        return !Equals(left, right);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[Membership: Id={Id.ToInvariantString()}]";
}