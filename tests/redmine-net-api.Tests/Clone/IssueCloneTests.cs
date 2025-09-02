using System;
using Padi.RedmineApi.Types;
using Xunit;


namespace Padi.RedmineAPI.Tests.Clone;

public sealed class IssueCloneTests
{
    [Fact]
    public void Clone_WithNullProperties_ReturnsNewInstanceWithNullProperties()
    {
        // Arrange
        var issue = new Issue();

        // Act
        var clone = issue.Clone(true);

        // Assert
        Assert.NotNull(clone);
        Assert.NotSame(issue, clone);
        Assert.Equal(issue.Id, clone.Id);
        Assert.Null(clone.Project);
        Assert.Null(clone.Tracker);
        Assert.Null(clone.Status);
    }

    [Fact]
    public void Clone_WithPopulatedProperties_ReturnsDeepCopy()
    {
        // Arrange
        var issue = CreateSampleIssue();

        // Act
        var clone = issue.Clone(true);

        // Assert
        Assert.NotNull(clone);
        Assert.NotSame(issue, clone);
        
        Assert.NotEqual(issue.Id, clone.Id);
        Assert.Equal(issue.Subject, clone.Subject);
        Assert.Equal(issue.Description, clone.Description);
        Assert.Equal(issue.DoneRatio, clone.DoneRatio);
        Assert.Equal(issue.IsPrivate, clone.IsPrivate);
        Assert.Equal(issue.EstimatedHours, clone.EstimatedHours);
        Assert.Equal(issue.CreatedOn, clone.CreatedOn);
        Assert.Equal(issue.UpdatedOn, clone.UpdatedOn);
        Assert.Equal(issue.ClosedOn, clone.ClosedOn);

        Assert.NotSame(issue.Project, clone.Project);
        Assert.Equal(issue.Project.Id, clone.Project.Id);
        Assert.Equal(issue.Project.Name, clone.Project.Name);

        Assert.NotSame(issue.Tracker, clone.Tracker);
        Assert.Equal(issue.Tracker.Id, clone.Tracker.Id);
        Assert.Equal(issue.Tracker.Name, clone.Tracker.Name);

        Assert.NotSame(issue.CustomFields, clone.CustomFields);
        Assert.Equal(issue.CustomFields.Count, clone.CustomFields.Count);
        for (var i = 0; i < issue.CustomFields.Count; i++)
        {
            Assert.NotSame(issue.CustomFields[i], clone.CustomFields[i]);
            Assert.Equal(issue.CustomFields[i].Id, clone.CustomFields[i].Id);
            Assert.Equal(issue.CustomFields[i].Name, clone.CustomFields[i].Name);
        }

        Assert.NotNull(clone.Attachments);
        Assert.Equal(issue.Attachments.Count, clone.Attachments.Count);
        Assert.All(clone.Attachments, Assert.NotNull);
    }

    [Fact]
    public void Clone_ModifyingClone_DoesNotAffectOriginal()
    {
        // Arrange
        var issue = CreateSampleIssue();
        var clone = issue.Clone(true);

        // Act
        clone.Subject = "Modified Subject";
        clone.Project.Name = "Modified Project";
        clone.CustomFields[0].Values = [new CustomFieldValue("Modified Value")];

        // Assert
        Assert.NotEqual(issue.Subject, clone.Subject);
        Assert.NotEqual(issue.Project.Name, clone.Project.Name);
        Assert.NotEqual(issue.CustomFields[0].Values, clone.CustomFields[0].Values);
    }

    private static Issue CreateSampleIssue()
    {
        return new Issue
        {
            Id = 1,
            Project = new IdentifiableName(100, "Test Project"),
            Tracker = new IdentifiableName(200, "Bug"),
            Status = new IssueStatus(300, "New"),
            Priority = new IdentifiableName(400, "Normal"),
            Author = new IdentifiableName(500, "John Doe"),
            Subject = "Test Issue",
            Description = "Test Description",
            StartDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(7),
            DoneRatio = 50,
            IsPrivate = false,
            EstimatedHours = 8.5f,
            CreatedOn = DateTime.Now.AddDays(-1),
            UpdatedOn = DateTime.Now,
            CustomFields =
            [
                new IssueCustomField
                {
                    Id = 1,
                    Name = "Custom Field 1",
                }
            ],
            Attachments =
            [
                new Attachment
                {
                    Id = 1,
                    FileName = "test.txt",
                    FileSize = 1024,
                    Author = new IdentifiableName(1, "Author")
                }
            ]
        };
    }
}