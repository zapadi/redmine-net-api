using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class UploadTests : BaseEqualityTests<Upload>
{
    protected override Upload CreateSampleInstance()
    {
        return new Upload
        {
            Token = "abc123",
            FileName = "test.pdf",
            ContentType = "application/pdf",
            Description = "Test Upload"
        };
    }

    protected override Upload CreateDifferentInstance()
    {
        return new Upload
        {
            Token = "xyz789",
            FileName = "different.pdf",
            ContentType = "application/pdf",
            Description = "Different Upload"
        };
    }
}