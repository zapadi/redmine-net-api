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
/// <remarks>
/// Available as of 1.1:
/// include: fetch associated data (optional). 
/// Possible values: children, attachments, relations, changesets and journals. To fetch multiple associations use comma (e.g ?include=relations,journals). 
/// See Issue journals for more information.
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Issue : Identifiable<Issue>, ICloneable<Issue>
{
    #region Properties
    /// <summary>
    /// Gets or sets the project.
    /// </summary>
    /// <value>The project.</value>
    public IdentifiableName Project { get; set; }

    /// <summary>
    /// Gets or sets the tracker.
    /// </summary>
    /// <value>The tracker.</value>
    public IdentifiableName Tracker { get; set; }

    /// <summary>
    /// Gets or sets the status. Possible values: open, closed, * to get open and closed issues, status id
    /// </summary>
    /// <value>The status.</value>
    public IssueStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    /// <value>The priority.</value>
    public IdentifiableName Priority { get; set; }

    /// <summary>
    /// Gets or sets the author.
    /// </summary>
    /// <value>The author.</value>
    public IdentifiableName Author { get; set; }

    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    /// <value>The category.</value>
    public IdentifiableName Category { get; set; }

    /// <summary>
    /// Gets or sets the subject.
    /// </summary>
    /// <value>The subject.</value>
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the start date.
    /// </summary>
    /// <value>The start date.</value>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the due date.
    /// </summary>
    /// <value>The due date.</value>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Gets or sets the done ratio.
    /// </summary>
    /// <value>The done ratio.</value>
    public float? DoneRatio { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [private notes].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [private notes]; otherwise, <c>false</c>.
    /// </value>
    public bool PrivateNotes { get; set; }

    /// <summary>
    /// Gets or sets the estimated hours.
    /// </summary>
    /// <value>The estimated hours.</value>
    public float? EstimatedHours { get; set; }

    /// <summary>
    /// Gets or sets the hours spent on the issue.
    /// </summary>
    /// <value>The hours spent on the issue.</value>
    public float? SpentHours { get; set; }

    /// <summary>
    /// Gets or sets the custom fields.
    /// </summary>
    /// <value>The custom fields.</value>
    public List<IssueCustomField> CustomFields { get; set; }

    /// <summary>
    /// Gets or sets the created on.
    /// </summary>
    /// <value>The created on.</value>
    public DateTime? CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the updated on.
    /// </summary>
    /// <value>The updated on.</value>
    public DateTime? UpdatedOn { get; set; }

    /// <summary>
    /// Gets or sets the closed on.
    /// </summary>
    /// <value>The closed on.</value>
    public DateTime? ClosedOn { get; set; }

    /// <summary>
    /// Gets or sets the notes.
    /// </summary>
    public string Notes { get; set; }

    /// <summary>
    /// Gets or sets the ID of the user to assign the issue to (currently no mechanism to assign by name).
    /// </summary>
    /// <value>
    /// The assigned to.
    /// </value>
    public IdentifiableName AssignedTo { get; set; }

    /// <summary>
    /// Gets or sets the parent issue id. Only when a new issue is created this property shall be used.
    /// </summary>
    /// <value>
    /// The parent issue id.
    /// </value>
    public IdentifiableName ParentIssue { get; set; }

    /// <summary>
    /// Gets or sets the fixed version.
    /// </summary>
    /// <value>
    /// The fixed version.
    /// </value>
    public IdentifiableName FixedVersion { get; set; }

    /// <summary>
    /// indicate whether the issue is private or not
    /// </summary>
    /// <value>
    /// <c>true</c> if this issue is private; otherwise, <c>false</c>.
    /// </value>
    public bool IsPrivate { get; set; }

    /// <summary>
    /// Returns the sum of spent hours of the task and all the sub tasks.
    /// </summary>
    /// <remarks>Availability starting with redmine version 3.3</remarks>
    public float? TotalSpentHours { get; set; }

    /// <summary>
    /// Returns the sum of estimated hours of task and all the sub tasks.
    /// </summary>
    /// <remarks>Availability starting with redmine version 3.3</remarks>
    public float? TotalEstimatedHours { get; set; }

    /// <summary>
    /// Gets or sets the journals.
    /// </summary>
    /// <value>
    /// The journals.
    /// </value>
    public List<Journal> Journals { get; set; }

    /// <summary>
    /// Gets or sets the change sets.
    /// </summary>
    /// <value>
    /// The change sets.
    /// </value>
    public List<ChangeSet> ChangeSets { get; set; }

    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    /// <value>
    /// The attachments.
    /// </value>
    public List<Attachment> Attachments { get; set; }

    /// <summary>
    /// Gets or sets the issue relations.
    /// </summary>
    /// <value>
    /// The issue relations.
    /// </value>
    public List<IssueRelation> Relations { get; set; }

    /// <summary>
    /// Gets or sets the issue children.
    /// </summary>
    /// <value>
    /// The issue children.
    /// NOTE: Only Id, tracker and subject are filled.
    /// </value>
    public List<IssueChild> Children { get; set; }

    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    /// <value>
    /// The attachment.
    /// </value>
    public List<Upload> Uploads { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public List<Watcher> Watchers { get; set; }
        
    /// <summary>
    /// 
    /// </summary>
    public List<IssueAllowedStatus> AllowedStatuses { get; set; }
    #endregion

    #region Implementation of IEquatable<Tracker>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(Issue other)
    {
        if (other == null) return false;
        return Id == other.Id
               && Project == other.Project
               && Tracker == other.Tracker
               && Status == other.Status
               && Priority == other.Priority
               && Author == other.Author
               && Category == other.Category
               && string.Equals(Subject, other.Subject, StringComparison.Ordinal)
               && string.Equals(Description, other.Description, StringComparison.Ordinal)
               && StartDate == other.StartDate
               && DueDate == other.DueDate
               && DoneRatio == other.DoneRatio
               && EstimatedHours == other.EstimatedHours
               && SpentHours == other.SpentHours
               && CreatedOn == other.CreatedOn
               && UpdatedOn == other.UpdatedOn
               && AssignedTo == other.AssignedTo
               && FixedVersion == other.FixedVersion
               && string.Equals(Notes, other.Notes, StringComparison.Ordinal)
               && ClosedOn == other.ClosedOn
               && PrivateNotes == other.PrivateNotes
               && (Attachments?.Equals<Attachment>(other.Attachments) ?? other.Attachments == null)
               && (CustomFields?.Equals<IssueCustomField>(other.CustomFields) ?? other.CustomFields == null)
               && (ChangeSets?.Equals<ChangeSet>(other.ChangeSets) ?? other.ChangeSets == null)
               && (Children?.Equals<IssueChild>(other.Children) ?? other.Children == null)
               && (Journals?.Equals<Journal>(other.Journals) ?? other.Journals == null)
               && (Relations?.Equals<IssueRelation>(other.Relations) ?? other.Relations == null)
               && (Watchers?.Equals<Watcher>(other.Watchers) ?? other.Watchers == null);
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
        return Equals(obj as Issue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();

        hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
          
        hashCode = HashCodeHelper.GetHashCode(Tracker, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Status, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Priority, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Category, hashCode);
            
        hashCode = HashCodeHelper.GetHashCode(Subject, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
        hashCode = HashCodeHelper.GetHashCode(StartDate, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
        hashCode = HashCodeHelper.GetHashCode(DueDate, hashCode);
        hashCode = HashCodeHelper.GetHashCode(DoneRatio, hashCode);
        hashCode = HashCodeHelper.GetHashCode(PrivateNotes, hashCode);
        hashCode = HashCodeHelper.GetHashCode(EstimatedHours, hashCode);
        hashCode = HashCodeHelper.GetHashCode(SpentHours, hashCode);
        hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
        hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);

        hashCode = HashCodeHelper.GetHashCode(Notes, hashCode);
        hashCode = HashCodeHelper.GetHashCode(AssignedTo, hashCode);
        hashCode = HashCodeHelper.GetHashCode(ParentIssue, hashCode);
        hashCode = HashCodeHelper.GetHashCode(FixedVersion, hashCode);
        hashCode = HashCodeHelper.GetHashCode(IsPrivate, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Journals, hashCode);
        hashCode = HashCodeHelper.GetHashCode(CustomFields, hashCode);

        hashCode = HashCodeHelper.GetHashCode(ChangeSets, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Attachments, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Relations, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Children, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Uploads, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Watchers, hashCode);

        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Issue left, Issue right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Issue left, Issue right)
    {
        return !Equals(left, right);
    }
    #endregion

    #region Implementation of IClonable<T>
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public new Issue Clone(bool resetId)
    {
        var issue = new Issue
        {
            Project = Project?.Clone(false),
            Tracker = Tracker?.Clone(false),
            Status = Status?.Clone(false),
            Priority = Priority?.Clone(false),
            Author = Author?.Clone(false),
            Category = Category?.Clone(false),
            Subject = Subject,
            Description = Description,
            StartDate = StartDate,
            DueDate = DueDate,
            DoneRatio = DoneRatio,
            IsPrivate = IsPrivate,
            EstimatedHours = EstimatedHours,
            TotalEstimatedHours = TotalEstimatedHours,
            SpentHours = SpentHours,
            TotalSpentHours = TotalSpentHours,
            AssignedTo = AssignedTo?.Clone(false),
            FixedVersion = FixedVersion?.Clone(false),
            Notes = Notes,
            PrivateNotes = PrivateNotes,
            CreatedOn = CreatedOn,
            UpdatedOn = UpdatedOn,
            ClosedOn = ClosedOn,
            ParentIssue = ParentIssue?.Clone(false),
            CustomFields = CustomFields?.Clone(false),
            Journals = Journals?.Clone(false),
            Attachments = Attachments?.Clone(false),
            Relations = Relations?.Clone(false),
            Children = Children?.Clone(false),
            Watchers = Watchers?.Clone(false),
            Uploads = Uploads?.Clone(false),
        };

        return issue;
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IdentifiableName AsParent()
    {
        return IdentifiableName.Create<IdentifiableName>(Id);
    }

    /// <summary>
    /// Provides a string representation of the object for use in debugging.
    /// </summary>
    /// <value>
    /// A string that represents the object, formatted for debugging purposes.
    /// </value>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[Issue:Id={Id.ToInvariantString()}, Status={Status?.Name}, Priority={Priority?.Name}, DoneRatio={DoneRatio?.ToString("F", CultureInfo.InvariantCulture)},IsPrivate={IsPrivate.ToInvariantString()}]";
}