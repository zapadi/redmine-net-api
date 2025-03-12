using System;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class UserTests : BaseEqualityTests<User>
{
    protected override User CreateSampleInstance()
    {
        return new User
        {
            Id = 1,
            Login = "testuser",
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            CreatedOn = new DateTime(2023, 1, 1).Date,
            LastLoginOn = new DateTime(2023, 1, 1).Date,
            ApiKey = "abc123",
            Status = UserStatus.StatusActive,
            IsAdmin = false,
            CustomFields =
            [
                new IssueCustomField
                {
                    Id = 1, 
                    Name = "Field 1", 
                    Values = 
                    [
                        new CustomFieldValue("Value 1")
                    ]
                }
            ],
            Memberships =
            [
                new Membership
                {
                    Id = 1,
                    Project = new IdentifiableName { Id = 1, Name = "Project 1" }
                }
            ],
            Groups = 
            [
                new UserGroup { Id = 1, Name = "Group 1" }
            ]
        };
    }

    protected override User CreateDifferentInstance()
    {
        return new User
        {
            Id = 2,
            Login = "differentuser",
            FirstName = "Different",
            LastName = "User",
            Email = "different@example.com",
            CreatedOn = new DateTime(2023, 1, 2).Date,
            LastLoginOn = new DateTime(2023, 1, 2).Date,
            ApiKey = "xyz789",
            Status = UserStatus.StatusLocked,
            IsAdmin = true
        };
    }
}