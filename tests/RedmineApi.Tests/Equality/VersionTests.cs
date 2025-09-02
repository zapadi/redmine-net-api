using System;
using Padi.RedmineApi.Types;
using Version = Padi.RedmineApi.Types.Version;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class VersionTests : BaseEqualityTests<Version>
{
    protected override Version CreateSampleInstance()
    {
        return new Version
        {
            Id = 1,
            Project = new IdentifiableName { Id = 1, Name = "Project 1" },
            Name = "1.0.0",
            Description = "First Release",
            Status = VersionStatus.Open,
            DueDate = new DateTime(2023, 12, 31).Date,
            CreatedOn = new DateTime(2023, 1, 1).Date,
            UpdatedOn = new DateTime(2023, 1, 1).Date,
            Sharing = VersionSharing.None,
            CustomFields = 
            [
                new IssueCustomField
                {
                    Id = 1, Name = "Field 1", Values = [new CustomFieldValue("Value 1")]
                }
            ]
        };
    }

    protected override Version CreateDifferentInstance()
    {
        return new Version
        {
            Id = 2,
            Project = new IdentifiableName { Id = 2, Name = "Project 2" },
            Name = "2.0.0",
            Description = "Second Release",
            Status = VersionStatus.Closed,
            DueDate = new DateTime(2024, 12, 31).Date,
            CreatedOn = new DateTime(2023, 1, 2).Date,
            UpdatedOn = new DateTime(2023, 1, 2).Date,
            Sharing = VersionSharing.System
        };
    }
}