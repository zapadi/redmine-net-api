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

using System.Diagnostics;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Internals;

namespace Padi.RedmineApi.Types;

/// <summary>
/// Availability 1.3
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class IssueRelation : Identifiable<IssueRelation>, ICloneable<IssueRelation>
{
    #region Properties
    /// <summary>
    /// Gets or sets the issue id.
    /// </summary>
    /// <value>The issue id.</value>
    public int IssueId { get; set; }

    /// <summary>
    /// Gets or sets the related issue id.
    /// </summary>
    /// <value>The issue to id.</value>
    public int IssueToId { get; set; }

    /// <summary>
    /// Gets or sets the type of relation.
    /// </summary>
    /// <value>The type.</value>
    public IssueRelationType Type { get; set; }

    /// <summary>
    /// Gets or sets the delay for a "precedes" or "follows" relation.
    /// </summary>
    /// <value>The delay.</value>
    public int? Delay { get; set; }
    #endregion

    #region Implementation of IEquatable<IssueRelation>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(IssueRelation other)
    {
        if (other == null) return false;
        return Id == other.Id 
               && IssueId == other.IssueId 
               && IssueToId == other.IssueToId 
               && Type == other.Type 
               && Delay == other.Delay;
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
        return Equals(obj as IssueRelation);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(IssueId, hashCode);
        hashCode = HashCodeHelper.GetHashCode(IssueToId, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Type, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Delay, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(IssueRelation left, IssueRelation right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(IssueRelation left, IssueRelation right)
    {
        return !Equals(left, right);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[IssueRelation: Id={Id.ToInvariantString()}, IssueId={IssueId.ToInvariantString()}, Type={Type:G}, Delay={Delay?.ToInvariantString()}]";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public new IssueRelation Clone(bool resetId)
    {
        if (resetId)
        {
            return new IssueRelation
            {
                IssueId = IssueId,
                IssueToId = IssueToId,
                Type = Type,
                Delay = Delay
            };
        }
        return new IssueRelation
        {
            Id = Id,
            IssueId = IssueId,
            IssueToId = IssueToId,
            Type = Type,
            Delay = Delay
        };
    }
}