using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class NewsTests : BaseEqualityTests<News>
{
    protected override News CreateSampleInstance()
    {
        return new News
        {
            Id = 1,
            Project = new IdentifiableName { Id = 1, Name = "Project 1" },
            Author = new IdentifiableName { Id = 1, Name = "Author 1" },
            Title = "Test News",
            Summary = "Test Summary",
            Description = "Test Description",
            CreatedOn = new DateTime(2023, 1, 1, 0, 0, 0).Date,
            Comments = [new NewsComment { Id = 1, Content = "Test Comment" }]
        };
    }

    protected override News CreateDifferentInstance()
    {
        return new News
        {
            Id = 2,
            Project = new IdentifiableName { Id = 2, Name = "Project 2" },
            Author = new IdentifiableName { Id = 2, Name = "Author 2" },
            Title = "Different News",
            Summary = "Different Summary",
            Description = "Different Description",
            CreatedOn = new DateTime(2023, 1, 2).Date,
            Comments = [new NewsComment { Id = 2, Content = "Different Comment" }]
        };
    }
}