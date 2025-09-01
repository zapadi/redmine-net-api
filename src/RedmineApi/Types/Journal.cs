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
using System.Globalization;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Internals;

namespace Padi.RedmineApi.Types;

/// <summary>
/// 
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Journal : Identifiable<Journal>, ICloneable<Journal>
{
    #region Properties
    /// <summary>
    /// Gets the user.
    /// </summary>
    /// <value>
    /// The user.
    /// </value>
    public IdentifiableName User { get; set; }

    /// <summary>
    /// Gets or sets the notes.
    /// </summary>
    /// <value>
    /// The notes.
    /// </value>
    /// <remarks> Setting Notes to string.empty or null will destroy the journal
    /// </remarks>
    public string Notes { get; set; }

    /// <summary>
    /// Gets the created on.
    /// </summary>
    /// <value>
    /// The created on.
    /// </value>
    public DateTime? CreatedOn { get;  set; }
        
    /// <summary>
    /// Gets the updated on.
    /// </summary>
    /// <value>
    /// The updated on.
    /// </value>
    public DateTime? UpdatedOn { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool PrivateNotes { get; set; }

    /// <summary>
    /// Gets the details.
    /// </summary>
    /// <value>
    /// The details.
    /// </value>
    public List<Detail> Details { get; set; }
        
    /// <summary>
    /// 
    /// </summary>
    public IdentifiableName UpdatedBy { get; set; }
    #endregion

    #region Implementation of IEquatable<Journal>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(Journal other)
    {
        if (other == null) return false;
        var result = ((Object)this).Equals(other);
        result = result && User == other.User;
        result = result && UpdatedBy == other.UpdatedBy;
        result = result && (Details?.Equals<Detail>(other.Details) ?? other.Details == null);
        result = result && string.Equals(Notes, other.Notes, StringComparison.Ordinal);
        result = result && CreatedOn == other.CreatedOn;
        result = result && UpdatedOn == other.UpdatedOn;
        result = result && PrivateNotes == other.PrivateNotes;
        return result;
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
        return Equals(obj as Journal);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(User, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Notes, hashCode);
        hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Details, hashCode);
        hashCode = HashCodeHelper.GetHashCode(PrivateNotes, hashCode);
        hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);
        hashCode = HashCodeHelper.GetHashCode(UpdatedBy, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Journal left, Journal right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Journal left, Journal right)
    {
        return !Equals(left, right);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[Journal: Id={Id.ToInvariantString()}, CreatedOn={CreatedOn?.ToString("u", CultureInfo.InvariantCulture)}]";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public new Journal Clone(bool resetId)
    {
        if (resetId)
        {
            return new Journal
            {
                User = User?.Clone(false),
                Notes = Notes,
                CreatedOn = CreatedOn,
                PrivateNotes = PrivateNotes,
                Details = Details?.Clone(false)
            };
        }
        return new Journal
        {
            Id = Id,
            User = User?.Clone(false),
            Notes = Notes,
            CreatedOn = CreatedOn,
            PrivateNotes = PrivateNotes,
            Details = Details?.Clone(false)
        };
    }
}