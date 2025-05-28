using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;

public static class TestConstants
{
    public static class Projects
    {
        public const int DefaultProjectId = 1;
        public const string DefaultProjectIdentifier = "1";
        public static readonly IdentifiableName DefaultProject = DefaultProject.ToIdentifiableName();
    }
    
    public static class Users
    {
        public const string DefaultPassword = "password123";
    }
    
}