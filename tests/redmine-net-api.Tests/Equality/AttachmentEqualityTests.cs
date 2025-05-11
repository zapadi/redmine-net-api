using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class AttachmentEqualityTests
{
    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        var attachment = CreateSampleAttachment();
        Assert.True(attachment.Equals(attachment));
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var attachment = CreateSampleAttachment();
        Assert.False(attachment.Equals(null));
    }

    [Theory]
    [MemberData(nameof(GetDifferentAttachments))]
    public void Equals_DifferentProperties_ReturnsFalse(Attachment attachment1, Attachment attachment2, string propertyName)
    {
        Assert.False(attachment1.Equals(attachment2), $"Attachments should not be equal when {propertyName} is different");
    }

    public static IEnumerable<object[]> GetDifferentAttachments()
    {
        var baseAttachment = CreateSampleAttachment();

        // Different FileName
        var differentFileName = CreateSampleAttachment();
        differentFileName.FileName = "different.txt";
        yield return [baseAttachment, differentFileName, "FileName"];

        // Different FileSize
        var differentFileSize = CreateSampleAttachment();
        differentFileSize.FileSize = 2048;
        yield return [baseAttachment, differentFileSize, "FileSize"];

        // Different Author
        var differentAuthor = CreateSampleAttachment();
        differentAuthor.Author = new IdentifiableName { Id = 999, Name = "Different Author" };
        yield return [baseAttachment, differentAuthor, "Author"];
    }

    private static Attachment CreateSampleAttachment()
    {
        return new Attachment
        {
            Id = 1,
            FileName = "test.txt",
            FileSize = 1024,
            ContentType = "text/plain",
            Description = "Test file",
            ContentUrl = "https://example.com/test.txt",
            ThumbnailUrl = "https://example.com/thumb.txt",
            Author = new IdentifiableName { Id = 1, Name = "John Doe" },
            CreatedOn = DateTime.Now
        };
    }
}