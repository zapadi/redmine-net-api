using System.Linq;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Tests;

public sealed class JsonPagedResultsTests
{
    private static readonly IRedmineSerializer Serializer = RedmineSerializerFactory.CreateSerializer(SerializationType.Json);

    [Theory]
    [InlineData("{\"issues\":[{\"id\":1},{\"id\":2}],\"total_count\":2,\"offset\":0,\"limit\":25}")]
    [InlineData("{\"issues\":[{\"id\":1},{\"id\":2}],\"total_count\":2,\"offset\":0,\"limit\":25,\"meta\":{\"version\":\"6.0\",\"tags\":[{\"id\":9}]}}")]
    [InlineData("{\"server\":\"x\",\"count\":7,\"issues\":[{\"id\":1},{\"id\":2}],\"flag\":true,\"total_count\":2,\"offset\":0,\"limit\":25}")]
    [InlineData("{\"meta\":{\"a\":{\"b\":[1,2]}},\"issues\":[{\"id\":1},{\"id\":2}],\"extra\":null,\"total_count\":2,\"offset\":0,\"limit\":25}")]
    [InlineData("{\"issues\":[{\"id\":1},{\"id\":2}],\"warnings\":[\"w\"],\"total_count\":2,\"offset\":0,\"limit\":25}")]
    public void Unknown_Top_Level_Properties_Do_Not_Replace_Items(string json)
    {
        var result = Serializer.DeserializeToPagedResults<Issue>(json);

        Assert.Equal([1, 2], result.Items.Select(i => i.Id).ToArray());
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(25, result.PageSize);
    }

    [Theory]
    [InlineData("{\"issues\":[],\"total_count\":0,\"offset\":0,\"limit\":25}")]
    [InlineData("{\"issues\":[],\"warnings\":[{\"id\":5}],\"total_count\":0,\"offset\":0,\"limit\":25}")]
    public void Empty_Array_Yields_No_Items(string json)
    {
        var result = Serializer.DeserializeToPagedResults<Issue>(json);

        Assert.Null(result.Items);
        Assert.Equal(0, result.TotalItems);
    }
}
