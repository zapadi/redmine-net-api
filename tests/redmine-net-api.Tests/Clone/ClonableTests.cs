using System;
using System.Collections.Generic;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Clone;

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
            Status = new IdentifiableName(300, "New"),
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

public sealed class AttachmentCloneTests
{
    [Fact]
    public void Clone_WithPopulatedProperties_ReturnsDeepCopy()
    {
        // Arrange
        var attachment = new Attachment
        {
            Id = 1,
            FileName = "test.txt",
            FileSize = 1024,
            ContentType = "text/plain",
            Description = "Test file",
            ContentUrl = "http://example.com/test.txt",
            ThumbnailUrl = "http://example.com/thumb.txt",
            Author = new IdentifiableName(1, "John Doe"),
            CreatedOn = DateTime.Now
        };

        // Act
        var clone = attachment.Clone(false);

        // Assert
        Assert.NotNull(clone);
        Assert.NotSame(attachment, clone);
        Assert.Equal(attachment.Id, clone.Id);
        Assert.Equal(attachment.FileName, clone.FileName);
        Assert.Equal(attachment.FileSize, clone.FileSize);
        Assert.Equal(attachment.ContentType, clone.ContentType);
        Assert.Equal(attachment.Description, clone.Description);
        Assert.Equal(attachment.ContentUrl, clone.ContentUrl);
        Assert.Equal(attachment.ThumbnailUrl, clone.ThumbnailUrl);
        Assert.Equal(attachment.CreatedOn, clone.CreatedOn);

        Assert.NotSame(attachment.Author, clone.Author);
        Assert.Equal(attachment.Author.Id, clone.Author.Id);
        Assert.Equal(attachment.Author.Name, clone.Author.Name);
    }
    
    [Fact]
    public void Clone_With_ResetId_True_Should_Return_A_Copy_With_Id_Set_Zero()
    {
        // Arrange
        var attachment = new Attachment
        {
            Id = 1,
            FileName = "test.txt",
            FileSize = 1024,
            ContentType = "text/plain",
            Description = "Test file",
            ContentUrl = "http://example.com/test.txt",
            ThumbnailUrl = "http://example.com/thumb.txt",
            Author = new IdentifiableName(1, "John Doe"),
            CreatedOn = DateTime.Now
        };

        // Act
        var clone = attachment.Clone(true);

        // Assert
        Assert.NotNull(clone);
        Assert.NotSame(attachment, clone);
        Assert.NotEqual(attachment.Id, clone.Id);
        Assert.Equal(attachment.FileName, clone.FileName);
        Assert.Equal(attachment.FileSize, clone.FileSize);
        Assert.Equal(attachment.ContentType, clone.ContentType);
        Assert.Equal(attachment.Description, clone.Description);
        Assert.Equal(attachment.ContentUrl, clone.ContentUrl);
        Assert.Equal(attachment.ThumbnailUrl, clone.ThumbnailUrl);
        Assert.Equal(attachment.CreatedOn, clone.CreatedOn);

        Assert.NotSame(attachment.Author, clone.Author);
        Assert.Equal(attachment.Author.Id, clone.Author.Id);
        Assert.Equal(attachment.Author.Name, clone.Author.Name);
    }
}

public sealed class JournalCloneTests
{
    [Fact]
    public void Clone_WithPopulatedProperties_ReturnsDeepCopy()
    {
        // Arrange
        var journal = new Journal
        {
            Id = 1,
            User = new IdentifiableName(1, "John Doe"),
            Notes = "Test notes",
            CreatedOn = DateTime.Now,
            PrivateNotes = true,
            Details = (List<Detail>)
            [
                new Detail
                {
                    Property = "status_id",
                    Name = "Status",
                    OldValue = "1",
                    NewValue = "2"
                }
            ]
        };

        // Act
        var clone = journal.Clone(false);

        // Assert
        Assert.NotNull(clone);
        Assert.NotSame(journal, clone);
        Assert.Equal(journal.Id, clone.Id);
        Assert.Equal(journal.Notes, clone.Notes);
        Assert.Equal(journal.CreatedOn, clone.CreatedOn);
        Assert.Equal(journal.PrivateNotes, clone.PrivateNotes);

        Assert.NotSame(journal.User, clone.User);
        Assert.Equal(journal.User.Id, clone.User.Id);
        Assert.Equal(journal.User.Name, clone.User.Name);

        Assert.NotNull(clone.Details);
        Assert.NotSame(journal.Details, clone.Details);
        Assert.Equal(journal.Details.Count, clone.Details.Count);

        var originalDetail = journal.Details[0];
        var clonedDetail = clone.Details[0];
        Assert.NotSame(originalDetail, clonedDetail);
        Assert.Equal(originalDetail.Property, clonedDetail.Property);
        Assert.Equal(originalDetail.Name, clonedDetail.Name);
        Assert.Equal(originalDetail.OldValue, clonedDetail.OldValue);
        Assert.Equal(originalDetail.NewValue, clonedDetail.NewValue);
    }
}
