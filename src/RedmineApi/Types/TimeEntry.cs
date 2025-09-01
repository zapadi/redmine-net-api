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
/// Availability 1.1
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class TimeEntry : Identifiable<TimeEntry>
    , ICloneable<TimeEntry>
{
    #region Properties
    private string comments;

    /// <summary>
    /// Gets or sets the issue id to log time on.
    /// </summary>
    /// <value>The issue id.</value>
    public IdentifiableName Issue { get; set; }

    /// <summary>
    /// Gets or sets the project id to log time on.
    /// </summary>
    /// <value>The project id.</value>
    public IdentifiableName Project { get; set; }

    /// <summary>
    /// Gets or sets the date the time was spent (default to the current date).
    /// </summary>
    /// <value>The spent on.</value>
    public DateTime? SpentOn { get; set; }

    /// <summary>
    /// Gets or sets the number of spent hours.
    /// </summary>
    /// <value>The hours.</value>
    public decimal Hours { get; set; }

    /// <summary>
    /// Gets or sets the activity id of the time activity. This parameter is required unless a default activity is defined in Redmine.
    /// </summary>
    /// <value>The activity id.</value>
    public IdentifiableName Activity { get; set; }

    /// <summary>
    /// Gets the user.
    /// </summary>
    /// <value>
    /// The user.
    /// </value>
    public IdentifiableName User { get; set; }

    /// <summary>
    /// Gets or sets the short description for the entry (255 characters max).
    /// </summary>
    /// <value>The comments.</value>
    public string Comments
    {
        get => comments;
        set => comments = value.Truncate(255);
    }

    /// <summary>
    /// Gets the created on.
    /// </summary>
    /// <value>The created on.</value>
    public DateTime? CreatedOn { get; set; }

    /// <summary>
    /// Gets the updated on.
    /// </summary>
    /// <value>The updated on.</value>
    public DateTime? UpdatedOn { get;  set; }

    /// <summary>
    /// Gets or sets the custom fields.
    /// </summary>
    /// <value>The custom fields.</value>
    public List<IssueCustomField> CustomFields { get; set; }
    #endregion

    #region Implementation of IEquatable<TimeEntry>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(TimeEntry other)
    {
        if (other == null) return false;
        return Id == other.Id
               && Issue == other.Issue
               && Project == other.Project
               && SpentOn == other.SpentOn
               && Hours == other.Hours
               && Activity == other.Activity
               && string.Equals(Comments, other.Comments, StringComparison.Ordinal)
               && User == other.User
               && CreatedOn == other.CreatedOn
               && UpdatedOn == other.UpdatedOn
               && (CustomFields?.Equals<IssueCustomField>(other.CustomFields) ?? other.CustomFields == null);
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
        return Equals(obj as TimeEntry);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(Issue, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
        hashCode = HashCodeHelper.GetHashCode(SpentOn, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Hours, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Activity, hashCode);
        hashCode = HashCodeHelper.GetHashCode(User, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Comments, hashCode);
        hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
        hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);
        hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(TimeEntry left, TimeEntry right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(TimeEntry left, TimeEntry right)
    {
        return !Equals(left, right);
    }
    #endregion

    #region Implementation of ICloneable 
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public new TimeEntry Clone(bool resetId)
    {
        var timeEntry = new TimeEntry
        {
            Activity = Activity,
            Comments = Comments,
            Hours = Hours,
            Issue = Issue,
            Project = Project,
            SpentOn = SpentOn,
            User = User,
            CustomFields = CustomFields
        };
        return timeEntry;
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[TimeEntry: Id={Id.ToInvariantString()}, SpentOn={SpentOn?.ToString("u", CultureInfo.InvariantCulture)}, Hours={Hours.ToString("F", CultureInfo.InvariantCulture)}]";

}