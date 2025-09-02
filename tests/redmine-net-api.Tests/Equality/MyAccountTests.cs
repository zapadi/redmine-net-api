using System;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class MyAccountTests : BaseEqualityTests<MyAccount>
{
    protected override MyAccount CreateSampleInstance()
    {
        return new MyAccount
        {
            Id = 1,
            Login = "testaccount",
            FirstName = "Test",
            LastName = "Account",
            Email = "test@example.com",
            CreatedOn = new DateTime(2023, 1, 1).Date,
            LastLoginOn = new DateTime(2023, 1, 1).Date,
            ApiKey = "abc123",
            CustomFields = [
                new MyAccountCustomField() { Value = "Value 1" }
            ]
        };
    }

    protected override MyAccount CreateDifferentInstance()
    {
        return new MyAccount
        {
            Id = 2,
            Login = "differentaccount",
            FirstName = "Different",
            LastName = "Account",
            Email = "different@example.com",
            CreatedOn = new DateTime(2023, 1, 2).Date,
            LastLoginOn = new DateTime(2023, 1, 2).Date,
            ApiKey = "xyz789"
        };
    }
}