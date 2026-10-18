using System.Collections.Generic;
using Redmine.Net.Api.Serialization;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public sealed class PagedResultsTests
{
    [Theory]
    [InlineData(100, 0, 25, 4, 1)]
    [InlineData(101, 0, 25, 5, 1)]
    [InlineData(99, 50, 25, 4, 3)]
    [InlineData(1, 0, 25, 1, 1)]
    [InlineData(25, 0, 25, 1, 1)]
    [InlineData(250, 100, 100, 3, 2)]
    public void TotalPages_Is_Ceiling_Of_Total_Over_Page_Size(int total, int offset, int pageSize, int totalPages, int currentPage)
    {
        var result = new PagedResults<string>(["a"], total, offset, pageSize);

        Assert.Equal(totalPages, result.TotalPages);
        Assert.Equal(currentPage, result.CurrentPage);
        Assert.Equal(total, result.TotalItems);
    }

    [Fact]
    public void Without_Total_Uses_Item_Count()
    {
        var result = new PagedResults<string>(new List<string> { "a", "b" }, 0, 0, 0);

        Assert.Equal(2, result.TotalItems);
        Assert.Equal(25, result.PageSize);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public void Empty_Result_Has_No_Pages()
    {
        var result = new PagedResults<string>(new List<string>(), 0, 0, 25);

        Assert.Equal(0, result.TotalItems);
        Assert.Equal(0, result.TotalPages);
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public void Without_Page_Size_Uses_Default_Page_Size_For_Total()
    {
        var result = new PagedResults<string>(new List<string> { "a" }, 60, 0, 0);

        Assert.Equal(25, result.PageSize);
        Assert.Equal(3, result.TotalPages);
    }
}
