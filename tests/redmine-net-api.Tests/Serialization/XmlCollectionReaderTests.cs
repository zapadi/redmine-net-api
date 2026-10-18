using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class XmlCollectionReaderTests
{
    [Fact(Timeout = Constants.TestTimeout)]
    public void Should_Read_Collection_With_Attributeless_Empty_Item()
    {
        const string input = """<journals type="array"><journal/><journal id="2"><notes>n</notes></journal></journals>""";

        var result = RunWithTimeout(() => Read(input, r => r.ReadElementContentAsCollection<Journal>()));

        Assert.Equal(2, result.Count);
        Assert.Equal(2, result[1].Id);
        Assert.Equal("n", result[1].Notes);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Should_Read_Collection_With_Attribute_Item_Followed_By_Element_Item()
    {
        const string input = """<watchers type="array"><user id="1" name="A"/><user id="2" name="B"></user><user id="3" name="C"/></watchers>""";

        var result = RunWithTimeout(() => Read(input, r => r.ReadElementContentAsCollection<Watcher>()));

        Assert.Equal([1, 2, 3], result.Select(w => w.Id));
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Should_Read_Collection_Of_Attribute_Only_Relations()
    {
        const string input = """<relations type="array"><relation id="1" issue_id="2" issue_to_id="3" relation_type="relates"/><relation id="4" issue_id="2" issue_to_id="5" relation_type="blocks"/></relations>""";

        var result = RunWithTimeout(() => Read(input, r => r.ReadElementContentAsCollection<IssueRelation>()));

        Assert.Equal([1, 4], result.Select(r => r.Id));
        Assert.Equal(IssueRelationType.Blocks, result[1].Type);
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Should_Read_Enumerable()
    {
        const string input = """<watchers type="array"><user id="1" name="A"/><user id="2" name="B"/></watchers>""";

        var result = RunWithTimeout(() => Read(input, r => r.ReadElementContentAsEnumerable<Watcher>().ToList()));

        Assert.Equal([1, 2], result.Select(w => w.Id));
    }

    [Fact(Timeout = Constants.TestTimeout)]
    public void Should_Read_Enumerable_From_Exhausted_Reader_As_Empty()
    {
        const string input = """<watchers type="array"></watchers>""";

        var result = RunWithTimeout(() => Read(input, r =>
        {
            r.ReadOuterXml();
            return r.ReadElementContentAsEnumerable<Watcher>().ToList();
        }));

        Assert.Empty(result);
    }

    private static T Read<T>(string xml, Func<System.Xml.XmlReader, T> read)
    {
        using var reader = XmlTextReaderBuilder.Create(new StringReader(xml));
        reader.MoveToContent();
        return read(reader);
    }

    private static T RunWithTimeout<T>(Func<T> func)
    {
        var task = Task.Run(func);
        Assert.True(task.Wait(TimeSpan.FromSeconds(5)), "Reading did not complete.");
        return task.Result;
    }
}
