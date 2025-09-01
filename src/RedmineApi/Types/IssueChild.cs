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
/// 
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class IssueChild : Identifiable<IssueChild>, ICloneable<IssueChild>
{
    #region Properties
    /// <summary>
    /// Gets or sets the tracker.
    /// </summary>
    /// <value>The tracker.</value>
    public IdentifiableName Tracker { get; set; }

    /// <summary>
    /// Gets or sets the subject.
    /// </summary>
    /// <value>The subject.</value>
    public string Subject { get; set; }
    #endregion

    #region Implementation of IEquatable<IssueChild>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(IssueChild other)
    {
        if (other == null) return false;
        return ((Object)this).Equals(other) 
               && Tracker == other.Tracker 
               && string.Equals(Subject, other.Subject, StringComparison.Ordinal);
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
        return Equals(obj as IssueChild);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(Tracker, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Subject, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(IssueChild left, IssueChild right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(IssueChild left, IssueChild right)
    {
        return !Equals(left, right);
    }
    #endregion 

    #region Implementation of IClonable<IssueChild>
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public new IssueChild Clone(bool resetId)
    {
        return new IssueChild
        {
            Id = Id,
            Tracker = Tracker?.Clone(false),
            Subject = Subject
        };
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[IssueChild: Id={Id.ToInvariantString()}]";
}