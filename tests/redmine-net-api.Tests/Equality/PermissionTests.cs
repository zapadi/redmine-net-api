using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class PermissionTests : BaseEqualityTests<Permission>
{
    protected override Permission CreateSampleInstance()
    {
        return new Permission
        {
            Info = "add_issues"
        };
    }

    protected override Permission CreateDifferentInstance()
    {
        return new Permission
        {
            Info = "edit_issues"
        };
    }
}