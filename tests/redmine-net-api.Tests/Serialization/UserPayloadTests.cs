using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class UserPayloadTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void User_Xml_Should_Send_Admin_Only_When_True(bool isAdmin)
    {
        var user = new User { Login = "u", IsAdmin = isAdmin, Status = UserStatus.StatusActive };

        var output = XElement.Parse(SerializerFactory.Create(SerializerKind.Xml).Serialize(user));

        Assert.Equal(isAdmin ? "true" : null, output.Element("admin")?.Value);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void User_Json_Should_Send_Admin_Only_When_True(bool isAdmin)
    {
        var user = new User { Login = "u", IsAdmin = isAdmin, Status = UserStatus.StatusActive };

        var output = JObject.Parse(SerializerFactory.Create(SerializerKind.NewtonsoftJson).Serialize(user));

        Assert.Equal(isAdmin ? "true" : null, (string)output["user"]?["admin"]);
    }

    [Fact]
    public void User_Xml_Should_Not_Send_Default_Status()
    {
        var user = new User { Login = "u" };

        var output = XElement.Parse(SerializerFactory.Create(SerializerKind.Xml).Serialize(user));

        Assert.Null(output.Element("status"));
    }

    [Fact]
    public void User_Json_Should_Not_Send_Default_Status()
    {
        var user = new User { Login = "u" };

        var output = JObject.Parse(SerializerFactory.Create(SerializerKind.NewtonsoftJson).Serialize(user));

        Assert.Null(output["user"]?["status"]);
    }

    [Theory]
    [InlineData(SerializerKind.Xml)]
    [InlineData(SerializerKind.NewtonsoftJson)]
    public void User_Should_Send_Explicit_Status(SerializerKind kind)
    {
        var user = new User { Login = "u", Status = UserStatus.StatusLocked };

        var output = SerializerFactory.Create(kind).Serialize(user);

        var status = kind == SerializerKind.Xml
            ? XElement.Parse(output).Element("status")?.Value
            : (string)JObject.Parse(output)["user"]?["status"];
        Assert.Equal("3", status);
    }
}
