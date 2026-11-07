using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class UserAuthSourceTests
{
    [Theory]
    [MemberData(nameof(UserWithAuthSource))]
    public void Should_Deserialize_AuthSource(string input, SerializerKind kind)
    {
        var output = SerializerFactory.Create(kind).Deserialize<User>(input);

        Assert.NotNull(output.AuthSource);
        Assert.Equal(2, output.AuthSource.Id);
        Assert.Equal("LDAP <corp>", output.AuthSource.Name);
        Assert.Equal(UserStatus.StatusActive, output.Status);
        Assert.Equal("jplang", output.Login);
    }

    [Theory]
    [MemberData(nameof(UsersWithAuthSource))]
    public void Should_Deserialize_AuthSource_In_List(string input, SerializerKind kind)
    {
        var output = SerializerFactory.Create(kind).DeserializeToPagedResults<User>(input).Items.ToList();

        Assert.Equal(2, output.Count);
        Assert.Equal(2, output[0].AuthSource?.Id);
        Assert.Null(output[1].AuthSource);
        Assert.Equal("second", output[1].Login);
    }

    [Fact]
    public void Should_Deserialize_AuthSource_Attribute_Form()
    {
        var output = SerializerFactory.Create(SerializerKind.Xml).Deserialize<User>("""<user><id>3</id><auth_source id="2" name="LDAP"/><login>jplang</login></user>""");

        Assert.Equal(2, output.AuthSource.Id);
        Assert.Equal("LDAP", output.AuthSource.Name);
        Assert.Equal("jplang", output.Login);
    }

    [Fact]
    public void Users_With_Different_AuthSource_Should_Not_Be_Equal()
    {
        var left = new User { Id = 1, AuthSource = new IdentifiableName(2, "a") };
        var right = new User { Id = 1, AuthSource = new IdentifiableName(3, "a") };

        Assert.NotEqual(left, right);
        Assert.Equal(left, new User { Id = 1, AuthSource = new IdentifiableName(2, "a") });
        Assert.Equal(left.GetHashCode(), new User { Id = 1, AuthSource = new IdentifiableName(2, "a") }.GetHashCode());
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> UserWithAuthSource()
    {
        const string json = """
                            {"user":{"id":3,"auth_source":{"id":2,"name":"LDAP <corp>"},"login":"jplang","status":1}}
                            """;
        const string xml = """
                           <user>
                             <id>3</id>
                             <auth_source>
                               <id>2</id>
                               <name>LDAP &lt;corp&gt;</name>
                             </auth_source>
                             <login>jplang</login>
                             <status>1</status>
                           </user>
                           """;
        yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
        yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
    }

    public static IEnumerable<TheoryDataRow<string, SerializerKind>> UsersWithAuthSource()
    {
        const string json = """
                            {"users":[{"id":3,"login":"jplang","auth_source":{"id":2,"name":"LDAP"}},{"id":4,"login":"second"}],"total_count":2,"offset":0,"limit":25}
                            """;
        const string xml = """
                           <users total_count="2" offset="0" limit="25" type="array">
                             <user><id>3</id><login>jplang</login><auth_source><id>2</id><name>LDAP</name></auth_source></user>
                             <user><id>4</id><login>second</login></user>
                           </users>
                           """;
        yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
        yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
    }
}
