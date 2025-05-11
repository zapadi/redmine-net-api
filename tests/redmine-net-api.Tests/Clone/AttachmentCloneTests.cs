using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Clone;

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