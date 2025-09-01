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
/// Availability 1.3
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class Tracker : IdentifiableName, IEquatable<Tracker>
{        
    /// <summary>
    /// Gets the default (issue) status for this tracker.
    /// </summary>
    public IdentifiableName DefaultStatus { get; set; }

    /// <summary>
    /// Gets the description of this tracker.
    /// </summary>
    public string Description { get; set; }
        
    /// <summary>
    /// Gets the list of enabled tracker's core fields
    /// </summary>
    public List<TrackerCoreField> EnabledStandardFields { get; set; }

    #region Implementation of IEquatable<Tracker>
    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
    /// </returns>
    public bool Equals(Tracker other)
    {
        if (other == null) return false;

        return base.Equals(other)
               && DefaultStatus == other.DefaultStatus
               && string.Equals(Description, other.Description, StringComparison.Ordinal)
               && EnabledStandardFields != null ? EnabledStandardFields.Equals<TrackerCoreField>(other.EnabledStandardFields) : other.EnabledStandardFields != null;
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
        return Equals(obj as Tracker);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        int hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(DefaultStatus, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
        hashCode = HashCodeHelper.GetHashCode(EnabledStandardFields, hashCode);
        return hashCode;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Tracker left, Tracker right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Tracker left, Tracker right)
    {
        return !Equals(left, right);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[Tracker: Id={Id.ToInvariantString()}, Name={Name}]";
}