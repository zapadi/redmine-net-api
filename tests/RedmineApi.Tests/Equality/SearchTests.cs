using System;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class SearchTests : BaseEqualityTests<Search>
{
    protected override Search CreateSampleInstance()
    {
        return new Search
        {
            Id = 1,
            Title = "Test Search",
            Type = "issue",
            Url = "http://example.com/search",
            Description = "Test Description",
            DateTime = new DateTime(2023, 1, 1).Date
        };
    }

    protected override Search CreateDifferentInstance()
    {
        return new Search
        {
            Id = 2,
            Title = "Different Search",
            Type = "wiki",
            Url = "http://example.com/different",
            Description = "Different Description",
            DateTime = new DateTime(2023, 1, 2).Date
        };
    }
}