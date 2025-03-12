using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class IssueCategoryTests : BaseEqualityTests<IssueCategory>
{
    protected override IssueCategory CreateSampleInstance()
    {
        return new IssueCategory
        {
            Id = 1,
            Name = "Test Category",
            Project = new IdentifiableName { Id = 1, Name = "Project 1" },
            AssignTo = new IdentifiableName { Id = 1, Name = "User 1" }
        };
    }

    protected override IssueCategory CreateDifferentInstance()
    {
        return new IssueCategory
        {
            Id = 2,
            Name = "Different Category",
            Project = new IdentifiableName { Id = 2, Name = "Project 2" },
            AssignTo = new IdentifiableName { Id = 2, Name = "User 2" }
        };
    }
}