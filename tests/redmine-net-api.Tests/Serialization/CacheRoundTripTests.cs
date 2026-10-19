using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

public class CacheRoundTripTests
{
    private const string TypesNamespace = "Redmine.Net.Api.Types";
    private const int MaxDepth = 3;

    private static readonly DateTime BaseDate = new DateTime(2024, 5, 6, 7, 8, 9, DateTimeKind.Utc);

    private static IEnumerable<Type> PublicModelTypes => typeof(Issue).Assembly.GetTypes()
        .Where(t => t.IsPublic && t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition && t.Namespace == TypesNamespace);

    public static IEnumerable<TheoryDataRow<string>> ModelTypes => PublicModelTypes
        .Where(t => t.GetConstructor(Type.EmptyTypes) != null)
        .Select(t => new TheoryDataRow<string>(t.Name));

    [Fact]
    public void Every_Model_Type_Has_A_Public_Parameterless_Constructor()
    {
        var missing = PublicModelTypes.Where(t => t.GetConstructor(Type.EmptyTypes) == null).Select(t => t.Name);

        Assert.Empty(missing);
    }

    [Theory]
    [MemberData(nameof(ModelTypes))]
    public void Should_RoundTrip_With_NewtonsoftJson(string typeName)
    {
        var type = ResolveType(typeName);
        var expected = new InstanceBuilder().Create(type, 0);

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(expected);
        var actual = Newtonsoft.Json.JsonConvert.DeserializeObject(json, type);

        AssertDeepEqual(expected, actual);
    }

#if NET
    [Theory]
    [MemberData(nameof(ModelTypes))]
    public void Should_RoundTrip_With_SystemTextJson(string typeName)
    {
        var type = ResolveType(typeName);
        var expected = new InstanceBuilder().Create(type, 0);

        var json = System.Text.Json.JsonSerializer.Serialize(expected, type);
        var actual = System.Text.Json.JsonSerializer.Deserialize(json, type);

        AssertDeepEqual(expected, actual);
    }
#endif

    private static Type ResolveType(string typeName)
    {
        var type = typeof(Issue).Assembly.GetType(TypesNamespace + "." + typeName);
        Assert.NotNull(type);
        return type;
    }

    private static void AssertDeepEqual(object expected, object actual)
    {
        var differences = new List<string>();
        Compare(expected, actual, expected.GetType().Name, differences);
        Assert.Empty(differences);
    }

    private static void Compare(object expected, object actual, string path, List<string> differences)
    {
        if (expected == null || actual == null)
        {
            if (expected != null || actual != null)
            {
                differences.Add($"{path}: expected '{expected ?? "null"}', actual '{actual ?? "null"}'");
            }

            return;
        }

        var type = expected.GetType();
        if (IsScalar(type))
        {
            if (!expected.Equals(actual))
            {
                differences.Add($"{path}: expected '{expected}', actual '{actual}'");
            }

            return;
        }

        if (expected is IEnumerable expectedItems)
        {
            if (actual is not IEnumerable actualEnumerable)
            {
                differences.Add($"{path}: expected a collection, actual '{actual.GetType().Name}'");
                return;
            }

            var left = expectedItems.Cast<object>().ToList();
            var right = actualEnumerable.Cast<object>().ToList();
            if (left.Count != right.Count)
            {
                differences.Add($"{path}: expected {left.Count} items, actual {right.Count}");
                return;
            }

            for (var i = 0; i < left.Count; i++)
            {
                Compare(left[i], right[i], $"{path}[{i}]", differences);
            }

            return;
        }

        if (type != actual.GetType())
        {
            differences.Add($"{path}: expected type '{type.Name}', actual '{actual.GetType().Name}'");
            return;
        }

        foreach (var property in ReadableProperties(type))
        {
            Compare(property.GetValue(expected), property.GetValue(actual), path + "." + property.Name, differences);
        }
    }

    private static bool IsScalar(Type type)
    {
        return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime);
    }

    private static IEnumerable<PropertyInfo> ReadableProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetGetMethod() != null && p.GetIndexParameters().Length == 0);
    }

    internal sealed class InstanceBuilder
    {
        private int _next;

        public object Create(Type type, int depth)
        {
            var instance = Activator.CreateInstance(type);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.GetGetMethod() != null && p.SetMethod != null && p.GetIndexParameters().Length == 0);

            foreach (var property in properties)
            {
                property.SetValue(instance, CreateValue(property.PropertyType, depth + 1));
            }

            return instance;
        }

        public object CreateValue(Type type, int depth)
        {
            var n = ++_next;
            var valueType = Nullable.GetUnderlyingType(type) ?? type;

            if (valueType == typeof(string)) return "value-" + n.ToString(CultureInfo.InvariantCulture);
            if (valueType == typeof(int)) return n;
            if (valueType == typeof(long)) return (long)n;
            if (valueType == typeof(bool)) return true;
            if (valueType == typeof(float)) return n + 0.5f;
            if (valueType == typeof(double)) return n + 0.5d;
            if (valueType == typeof(decimal)) return n + 0.25m;
            if (valueType == typeof(DateTime)) return BaseDate.AddDays(n);
            if (valueType.IsEnum)
            {
                var values = Enum.GetValues(valueType);
                return values.GetValue(values.Length - 1);
            }

            var itemType = GetListItemType(valueType);
            if (itemType != null)
            {
                var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
                if (depth < MaxDepth)
                {
                    list.Add(CreateValue(itemType, depth));
                    list.Add(CreateValue(itemType, depth));
                }

                return list;
            }

            if (valueType.IsClass && !valueType.IsAbstract && valueType.GetConstructor(Type.EmptyTypes) != null)
            {
                return depth < MaxDepth ? Create(valueType, depth) : null;
            }

            throw new NotSupportedException($"Cannot build a value of type '{valueType}'.");
        }

        private static Type GetListItemType(Type type)
        {
            if (!type.IsGenericType)
            {
                return null;
            }

            var definition = type.GetGenericTypeDefinition();
            if (definition == typeof(List<>) || definition == typeof(IList<>) || definition == typeof(ICollection<>)
                || definition == typeof(IEnumerable<>) || definition == typeof(IReadOnlyList<>) || definition == typeof(IReadOnlyCollection<>))
            {
                return type.GetGenericArguments()[0];
            }

            return null;
        }
    }
}
