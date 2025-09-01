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
using System.Diagnostics;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Internals;

namespace Padi.RedmineApi.Types;

/// <summary>
/// Availability 1.3
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Query : IdentifiableName, IEquatable<Query>
{
    #region Properties
    /// <summary>
    /// Gets a value indicating whether this instance is public.
    /// </summary>
    /// <value><c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
    public bool IsPublic { get; set; }

    /// <summary>
    /// Gets  the project id.
    /// </summary>
    /// <value>The project id.</value>
    public int? ProjectId { get; set; }
    #endregion

    #region Implementation of IEquatable<Query>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Query other)
    {
        if (other == null) return false;

        return base.Equals(other)
               && IsPublic == other.IsPublic
               && ProjectId == other.ProjectId;
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
        return Equals(obj as Query);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(IsPublic, hashCode);
        hashCode = HashCodeHelper.GetHashCode(ProjectId, hashCode);
        return hashCode;
    }
            
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Query left, Query right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Query left, Query right)
    {
        return !Equals(left, right);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[Query: Id={Id.ToInvariantString()}, Name={Name}, IsPublic={IsPublic.ToInvariantString()}, ProjectId={ProjectId?.ToInvariantString()}]";
}