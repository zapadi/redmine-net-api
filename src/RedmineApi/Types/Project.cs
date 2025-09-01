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
/// Availability 1.0
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Project : IdentifiableName, IEquatable<Project>
{
    #region Properties

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <remarks>Required for create</remarks>
    /// <value>The identifier.</value>
    public string Identifier { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the parent.
    /// </summary>
    /// <value>The parent.</value>
    public IdentifiableName Parent { get; set; }

    /// <summary>
    /// Gets or sets the home page.
    /// </summary>
    /// <value>The home page.</value>
    public string HomePage { get; set; }

    /// <summary>
    /// Gets the created on.
    /// </summary>
    /// <value>The created on.</value>
    public DateTime? CreatedOn { get; set; }

    /// <summary>
    /// Gets the updated on.
    /// </summary>
    /// <value>The updated on.</value>
    public DateTime? UpdatedOn { get; set; }

    /// <summary>
    /// Gets the status.
    /// </summary>
    /// <value>
    /// The status.
    /// </value>
    public ProjectStatus Status { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this project is public.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this project is public; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>Available in Redmine starting with 2.6.0 version.</remarks>
    public bool IsPublic { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [inherit members].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [inherit members]; otherwise, <c>false</c>.
    /// </value>
    public bool InheritMembers { get; set; }

    /// <summary>
    /// Gets or sets the trackers.
    /// </summary>
    /// <value>
    /// The trackers.
    /// </value>
    /// <remarks>Available in Redmine starting with 2.6.0 version.</remarks>
    public List<ProjectTracker> Trackers { get; set; }

    /// <summary>
    /// Gets or sets the enabled modules.
    /// </summary>
    /// <value>
    /// The enabled modules.
    /// </value>
    /// <remarks>Available in Redmine starting with 2.6.0 version.</remarks>
    public List<ProjectEnabledModule> EnabledModules { get; set; }

    /// <summary>
    ///
    /// </summary>
    public List<IssueCustomField> IssueCustomFields { get; set; }

    /// <summary>
    ///
    /// </summary>
    public List<IdentifiableName> CustomFieldValues { get; set; }

    /// <summary>
    /// Gets the issue categories.
    /// </summary>
    /// <value>
    /// The issue categories.
    /// </value>
    /// <remarks>Available in Redmine starting with the 2.6.0 version.</remarks>
    public List<ProjectIssueCategory> IssueCategories { get; set; }

    /// <summary>
    /// Gets the time entry activities.
    /// </summary>
    /// <remarks>Available in Redmine starting with the 3.4.0 version.</remarks>
    public List<ProjectTimeEntryActivity> TimeEntryActivities { get; set; }

    /// <summary>
    ///
    /// </summary>
    public IdentifiableName DefaultVersion { get; set; }

    /// <summary>
    ///
    /// </summary>
    public IdentifiableName DefaultAssignee { get; set; }
    #endregion

    #region Implementation of IEquatable<Project>
    /// <summary>
    ///
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Project other)
    {
        if (other == null)
        {
            return false;
        }

        return base.Equals(other)
               && string.Equals(Identifier, other.Identifier, StringComparison.Ordinal)
               && string.Equals(Description, other.Description, StringComparison.Ordinal)
               && string.Equals(HomePage, other.HomePage, StringComparison.Ordinal)
               && string.Equals(Identifier, other.Identifier, StringComparison.Ordinal)
               && CreatedOn == other.CreatedOn
               && UpdatedOn == other.UpdatedOn
               && Status == other.Status
               && IsPublic == other.IsPublic
               && InheritMembers == other.InheritMembers
               && DefaultAssignee == other.DefaultAssignee
               && DefaultVersion == other.DefaultVersion
               && Parent == other.Parent
               && (Trackers?.Equals<ProjectTracker>(other.Trackers) ?? other.Trackers == null)
               && (IssueCustomFields?.Equals<IssueCustomField>(other.IssueCustomFields) ?? other.IssueCustomFields == null)
               && (CustomFieldValues?.Equals<IdentifiableName>(other.CustomFieldValues) ?? other.CustomFieldValues == null)
               && (IssueCategories?.Equals<ProjectIssueCategory>(other.IssueCategories) ?? other.IssueCategories == null)
               && (EnabledModules?.Equals<ProjectEnabledModule>(other.EnabledModules) ?? other.EnabledModules == null)
               && (TimeEntryActivities?.Equals<ProjectTimeEntryActivity>(other.TimeEntryActivities) ?? other.TimeEntryActivities == null);
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
        return Equals(obj as Project);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(Identifier, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Parent, hashCode);
        hashCode = HashCodeHelper.GetHashCode(HomePage, hashCode);
        hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
        hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Status, hashCode);
        hashCode = HashCodeHelper.GetHashCode(IsPublic, hashCode);
        hashCode = HashCodeHelper.GetHashCode(InheritMembers, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Trackers, hashCode);
        hashCode = HashCodeHelper.GetHashCode(IssueCustomFields, hashCode);
        hashCode = HashCodeHelper.GetHashCode(CustomFieldValues, hashCode);
        hashCode = HashCodeHelper.GetHashCode(IssueCategories, hashCode);
        hashCode = HashCodeHelper.GetHashCode(EnabledModules, hashCode);
        hashCode = HashCodeHelper.GetHashCode(TimeEntryActivities, hashCode);
        hashCode = HashCodeHelper.GetHashCode(DefaultAssignee, hashCode);
        hashCode = HashCodeHelper.GetHashCode(DefaultVersion, hashCode);

        return hashCode;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Project left, Project right)
    {
        return Equals(left, right);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Project left, Project right)
    {
        return !Equals(left, right);
    }
    #endregion

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[Project: Id={Id.ToInvariantString()}, Name={Name}, Identifier={Identifier}, Status={Status:G}, IsPublic={IsPublic.ToInvariantString()}]";
}
