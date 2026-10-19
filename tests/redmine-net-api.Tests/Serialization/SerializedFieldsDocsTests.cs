using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class SerializedFieldsDocsTests
{
    private const string TypesNamespace = "Redmine.Net.Api.Types";

    private const string Sent = "Sent to Redmine when creating or updating.";
    private const string SentWhenSet = "Sent to Redmine when creating or updating, only when set.";
    private const string SentOnlyJson = "Sent to Redmine only with the JSON serializer.";
    private const string SentOnlyXml = "Sent to Redmine only with the XML serializer.";
    private const string ReadOnly = "Read-only: set by Redmine and not sent when creating or updating.";
    private const string SentWhenCreating = "Sent to Redmine only when creating.";
    private const string SentWhenCreatingWhenSet = "Sent to Redmine only when creating, only when set.";
    private const string SentWhenUpdating = "Sent to Redmine only when updating.";
    private const string SentWhenUpdatingWhenSet = "Sent to Redmine only when updating, only when set.";
    private const string AlwaysSentWithJson = "With the JSON serializer it is always sent";
    private const string AlwaysSentWithXml = "With the XML serializer it is always sent";
    private const string IdentifiableId = "P:Redmine.Net.Api.Types.Identifiable`1.Id";

    private static readonly string[] Sentences =
    [
        Sent, SentWhenSet, SentOnlyJson, SentOnlyXml, ReadOnly,
        SentWhenCreating, SentWhenCreatingWhenSet, SentWhenUpdating, SentWhenUpdatingWhenSet
    ];

    private static readonly string[] Checked = [.. Sentences, AlwaysSentWithJson, AlwaysSentWithXml];

    [Fact]
    public void Every_Property_Remarks_Match_What_The_Writers_Send()
    {
        var remarks = LoadRemarks();
        var failures = new List<string>();

        foreach (var type in ModelTypes())
        {
            foreach (var property in SettableProperties(type).Where(p => p.DeclaringType == type))
            {
                var expected = Classify(type, property);
                remarks.TryGetValue(MemberId(property), out var actual);
                if (actual == null || expected.Any(e => !actual.Contains(e))
                    || Checked.Any(c => actual.Contains(c) && !expected.Contains(c)))
                {
                    failures.Add($"{type.Name}.{property.Name}: expected '{string.Join(" ", expected)}', actual remarks '{actual ?? "<none>"}'");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void Every_Property_Remarks_State_Exactly_One_Sending_Rule()
    {
        var remarks = LoadRemarks();
        var failures = new List<string>();

        foreach (var property in ModelTypes().SelectMany(SettableProperties).Select(MemberId).Distinct())
        {
            remarks.TryGetValue(property, out var actual);
            var expectedCount = property == IdentifiableId ? 0 : 1;
            if (Sentences.Count(s => actual?.Contains(s) == true) != expectedCount)
            {
                failures.Add($"{property}: actual remarks '{actual ?? "<none>"}'");
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    private static IEnumerable<Type> ModelTypes()
    {
        return typeof(Issue).Assembly.GetTypes()
            .Where(t => t.IsPublic && t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition
                        && t.Namespace == TypesNamespace && t.GetConstructor(Type.EmptyTypes) != null)
            .OrderBy(t => t.Name);
    }

    private static IEnumerable<PropertyInfo> SettableProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetSetMethod() != null && p.GetIndexParameters().Length == 0)
            .OrderBy(p => p.Name);
    }

    private static string[] Classify(Type type, PropertyInfo property)
    {
        var xmlCreate = Compare(SerializerKind.Xml, type, property, false);
        var xmlUpdate = Compare(SerializerKind.Xml, type, property, true);
        var jsonCreate = Compare(SerializerKind.NewtonsoftJson, type, property, false);
        var jsonUpdate = Compare(SerializerKind.NewtonsoftJson, type, property, true);

        var xml = xmlCreate > xmlUpdate ? xmlCreate : xmlUpdate;
        var json = jsonCreate > jsonUpdate ? jsonCreate : jsonUpdate;

        if (xml == Change.None && json == Change.None)
        {
            return [ReadOnly];
        }

        if (xml == Change.None)
        {
            return [SentOnlyJson];
        }

        if (json == Change.None)
        {
            return [SentOnlyXml];
        }

        var whenSet = xml == Change.Added || json == Change.Added;
        var creatingOnly = xmlUpdate == Change.None && jsonUpdate == Change.None;
        var updatingOnly = xmlCreate == Change.None && jsonCreate == Change.None;

        var rule = creatingOnly ? (whenSet ? SentWhenCreatingWhenSet : SentWhenCreating)
            : updatingOnly ? (whenSet ? SentWhenUpdatingWhenSet : SentWhenUpdating)
            : whenSet ? SentWhenSet
            : Sent;

        return xml == Change.Added && json == Change.Value ? [rule, AlwaysSentWithJson]
            : json == Change.Added && xml == Change.Value ? [rule, AlwaysSentWithXml]
            : [rule];
    }

    private enum Change
    {
        None,
        Value,
        Added
    }

    private static Change Compare(SerializerKind kind, Type type, PropertyInfo property, bool updating)
    {
        var baseline = CreateBaseline(type);
        var changed = CreateBaseline(type);
        var id = type.GetProperty("Id");
        if (updating && property.Name != "Id" && id?.PropertyType == typeof(int) && id.GetSetMethod() != null)
        {
            id.SetValue(baseline, 1);
            id.SetValue(changed, 1);
        }

        property.SetValue(changed, CreateDifferentValue(property, property.GetValue(baseline)));

        var before = Serialize(kind, type, baseline);
        var after = Serialize(kind, type, changed);
        if (before == after)
        {
            return Change.None;
        }

        var beforeKeys = Keys(kind, before);
        return Keys(kind, after).All(beforeKeys.Contains) ? Change.Value : Change.Added;
    }

    private static object CreateBaseline(Type type)
    {
        var instance = Activator.CreateInstance(type);
        if (instance is IssueRelation relation)
        {
            relation.Type = IssueRelationType.Precedes;
        }

        return instance;
    }

    private static object CreateDifferentValue(PropertyInfo property, object current)
    {
        var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        if (type == typeof(bool))
        {
            return !(current is true);
        }

        if (type.IsEnum)
        {
            return Enum.GetValues(type).Cast<object>().Last(v => !v.Equals(current));
        }

        return new CacheRoundTripTests.InstanceBuilder().CreateValue(property.PropertyType, 0);
    }

    private static string Serialize(SerializerKind kind, Type type, object instance)
    {
        var serializer = SerializerFactory.Create(kind);
        var method = serializer.GetType().GetMethod(nameof(serializer.Serialize)).MakeGenericMethod(type);
        try
        {
            return (string)method.Invoke(serializer, [instance]);
        }
        catch (TargetInvocationException ex)
        {
            throw ex.InnerException ?? ex;
        }
    }

    private static HashSet<string> Keys(SerializerKind kind, string payload)
    {
        var keys = new HashSet<string>();
        if (kind == SerializerKind.Xml)
        {
            foreach (var element in XElement.Parse(payload).DescendantsAndSelf())
            {
                var path = string.Join("/", element.AncestorsAndSelf().Reverse().Select(e => e.Name.LocalName));
                keys.Add(path);
                foreach (var attribute in element.Attributes())
                {
                    keys.Add(path + "/@" + attribute.Name.LocalName);
                }
            }
        }
        else
        {
            foreach (var token in ParseJson(payload).Descendants().OfType<JProperty>())
            {
                keys.Add(string.Join("/", token.AncestorsAndSelf().OfType<JProperty>().Reverse().Select(p => p.Name)) + (token.Value is JArray ? "[]" : ""));
            }
        }

        return keys;
    }

    private static JContainer ParseJson(string payload)
    {
        try
        {
            return (JContainer)JToken.Parse(payload);
        }
        catch (Newtonsoft.Json.JsonReaderException)
        {
            return JObject.Parse("{" + payload + "}");
        }
    }

    private static string MemberId(PropertyInfo property)
    {
        var declaring = property.DeclaringType;
        if (declaring.IsGenericType)
        {
            declaring = declaring.GetGenericTypeDefinition();
        }

        return "P:" + declaring.FullName + "." + property.Name;
    }

    private static Dictionary<string, string> LoadRemarks()
    {
        var path = Path.ChangeExtension(typeof(Issue).Assembly.Location, ".xml");
        Assert.True(System.IO.File.Exists(path), $"Documentation file '{path}' was not found.");

        return XDocument.Load(path).Descendants("member")
            .Where(m => ((string)m.Attribute("name")).StartsWith("P:" + TypesNamespace + ".", StringComparison.Ordinal))
            .ToDictionary(m => (string)m.Attribute("name"), m => Normalize(m.Element("remarks")?.Value));
    }

    private static string Normalize(string text)
    {
        return text == null ? null : string.Join(" ", text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
    }
}
