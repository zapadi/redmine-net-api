using System.Collections.Specialized;
using Redmine.Net.Api;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public sealed class SearchFilterBuilderTests
{
    [Fact]
    public void Build_Should_Omit_False_Flags()
    {
        var builder = new SearchFilterBuilder
        {
            IncludeIssues = false,
            IncludeNews = false,
            IncludeDocuments = false,
            IncludeChangeSets = false,
            IncludeWikiPages = false,
            IncludeMessages = false,
            IncludeProjects = false,
            TitlesOnly = false,
            OpenIssues = false,
        };

        var parameters = builder.Build(new NameValueCollection());

        Assert.Empty(parameters);
    }

    [Fact]
    public void Build_Should_Emit_True_Flags()
    {
        var builder = new SearchFilterBuilder
        {
            IncludeIssues = true,
            IncludeWikiPages = true,
            TitlesOnly = true,
            OpenIssues = true,
        };

        var parameters = builder.Build(new NameValueCollection());

        Assert.Equal(4, parameters.Count);
        Assert.Equal("1", parameters[RedmineKeys.ISSUES]);
        Assert.Equal("1", parameters[RedmineKeys.WIKI_PAGES]);
        Assert.Equal("1", parameters[RedmineKeys.TITLES_ONLY]);
        Assert.Equal("1", parameters[RedmineKeys.OPEN_ISSUES]);
    }

    [Theory]
    [InlineData(true, "1")]
    [InlineData(false, "")]
    public void Build_Should_Emit_AllWords(bool allWords, string expected)
    {
        var parameters = new SearchFilterBuilder { AllWords = allWords }.Build(new NameValueCollection());

        Assert.Equal(expected, parameters[RedmineKeys.ALL_WORDS]);
    }

    [Fact]
    public void Build_Should_Omit_AllWords_When_Not_Set()
    {
        var parameters = new SearchFilterBuilder().Build(new NameValueCollection());

        Assert.Null(parameters[RedmineKeys.ALL_WORDS]);
    }

    [Fact]
    public void Build_Should_Emit_Scope_And_Attachments()
    {
        var parameters = new SearchFilterBuilder { Scope = SearchScope.SubProjects, Attachments = SearchAttachment.OnlyInAttachment }.Build(new NameValueCollection());

        Assert.Equal("subprojects", parameters[RedmineKeys.SCOPE]);
        Assert.Equal("only", parameters[RedmineKeys.ATTACHMENTS]);
    }
}
