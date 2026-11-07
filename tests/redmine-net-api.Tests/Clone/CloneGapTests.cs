using System;
using System.Collections.Generic;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Clone;

public sealed class CloneGapTests
{
    [Fact]
    public void Issue_Clone_WithoutResetId_KeepsId()
    {
        var issue = new Issue { Id = 5, Subject = "subject" };

        Assert.Equal(5, issue.Clone(false).Id);
        Assert.Equal(0, issue.Clone(true).Id);
    }

    [Fact]
    public void Issue_Clone_CopiesChangeSetsAndAllowedStatuses()
    {
        var issue = new Issue
        {
            Id = 5,
            ChangeSets = [new ChangeSet { Revision = "abc", Comments = "c", User = new IdentifiableName(1, "User") }],
            AllowedStatuses = [new IssueAllowedStatus { Id = 2, Name = "Closed", IsClosed = true }]
        };

        var clone = issue.Clone(false);

        Assert.NotSame(issue.ChangeSets, clone.ChangeSets);
        Assert.Equal(issue.ChangeSets, clone.ChangeSets);
        Assert.NotSame(issue.ChangeSets[0], clone.ChangeSets[0]);
        Assert.NotSame(issue.ChangeSets[0].User, clone.ChangeSets[0].User);

        Assert.NotSame(issue.AllowedStatuses, clone.AllowedStatuses);
        Assert.Equal(issue.AllowedStatuses, clone.AllowedStatuses);
        Assert.NotSame(issue.AllowedStatuses[0], clone.AllowedStatuses[0]);
    }

    [Fact]
    public void TimeEntry_Clone_IsDeepAndHonoursResetId()
    {
        var timeEntry = new TimeEntry
        {
            Id = 7,
            Activity = new IdentifiableName(1, "Dev"),
            Issue = new IdentifiableName(2, "Issue"),
            Project = new IdentifiableName(3, "Project"),
            User = new IdentifiableName(4, "User"),
            Hours = 1.5m,
            Comments = "comments",
            SpentOn = new DateTime(2024, 1, 1),
            CreatedOn = new DateTime(2024, 1, 2),
            UpdatedOn = new DateTime(2024, 1, 3),
            CustomFields = [IssueCustomField.CreateSingle(1, "Field", "value")]
        };

        var clone = timeEntry.Clone(false);

        Assert.Equal(timeEntry, clone);
        Assert.Equal(7, clone.Id);
        Assert.Equal(timeEntry.CreatedOn, clone.CreatedOn);
        Assert.Equal(timeEntry.UpdatedOn, clone.UpdatedOn);
        Assert.NotSame(timeEntry.Activity, clone.Activity);
        Assert.NotSame(timeEntry.Issue, clone.Issue);
        Assert.NotSame(timeEntry.Project, clone.Project);
        Assert.NotSame(timeEntry.User, clone.User);
        Assert.NotSame(timeEntry.CustomFields, clone.CustomFields);
        Assert.NotSame(timeEntry.CustomFields[0], clone.CustomFields[0]);
        Assert.Equal(0, timeEntry.Clone(true).Id);
    }

    [Fact]
    public void Journal_Clone_CopiesUpdatedOnAndUpdatedBy()
    {
        var journal = new Journal
        {
            Id = 1,
            UpdatedOn = new DateTime(2024, 1, 3),
            UpdatedBy = new IdentifiableName(4, "User")
        };

        foreach (var clone in new[] { journal.Clone(false), journal.Clone(true) })
        {
            Assert.Equal(journal.UpdatedOn, clone.UpdatedOn);
            Assert.Equal(journal.UpdatedBy, clone.UpdatedBy);
            Assert.NotSame(journal.UpdatedBy, clone.UpdatedBy);
        }
    }

    [Fact]
    public void IssueChild_Clone_CopiesChildrenAndHonoursResetId()
    {
        var issueChild = new IssueChild
        {
            Id = 3,
            Subject = "subject",
            Children = [new Issue { Id = 4, Subject = "child" }]
        };

        var clone = issueChild.Clone(false);

        Assert.Equal(issueChild, clone);
        Assert.NotSame(issueChild.Children, clone.Children);
        Assert.NotSame(issueChild.Children[0], clone.Children[0]);
        Assert.Equal(0, issueChild.Clone(true).Id);
    }

    [Fact]
    public void IssueCustomField_Clone_DoesNotShareValues()
    {
        var customField = IssueCustomField.CreateMultiple(1, "Field", ["a", "b"]);

        var clone = customField.Clone(false);

        Assert.Equal(customField, clone);
        Assert.NotSame(customField.Values, clone.Values);
        Assert.NotSame(customField.Values[0], clone.Values[0]);
    }

    [Fact]
    public void ChangeSet_Clone_DoesNotShareUser()
    {
        var changeSet = new ChangeSet { Revision = "abc", User = new IdentifiableName(1, "User") };

        var clone = changeSet.Clone(false);

        Assert.Equal(changeSet, clone);
        Assert.NotSame(changeSet.User, clone.User);
    }
}
