using System;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class TimeEntryTests : BaseEqualityTests<TimeEntry>
{
    protected override TimeEntry CreateSampleInstance()
    {
        return new TimeEntry
        {
            Id = 1,
            Project = new IdentifiableName { Id = 1, Name = "Project 1" },
            Issue = new IdentifiableName { Id = 1, Name = "Issue 1" },
            User = new IdentifiableName { Id = 1, Name = "User 1" },
            Activity = new IdentifiableName { Id = 1, Name = "Development" },
            Hours = (decimal)8.0,
            Comments = "Work done",
            SpentOn = new DateTime(2023, 1, 1).Date,
            CreatedOn = new DateTime(2023, 1, 1).Date,
            UpdatedOn = new DateTime(2023, 1, 1).Date,
            CustomFields = 
            [
                new IssueCustomField
                {
                    Id = 1, 
                    Name = "Field 1", 
                    Values = 
                    [
                        new CustomFieldValue("value")
                    ]
                }
            ]
        };
    }

    protected override TimeEntry CreateDifferentInstance()
    {
        return new TimeEntry
        {
            Id = 2,
            Project = new IdentifiableName { Id = 2, Name = "Project 2" },
            Issue = new IdentifiableName { Id = 2, Name = "Issue 2" },
            User = new IdentifiableName { Id = 2, Name = "User 2" },
            Activity = new IdentifiableName { Id = 2, Name = "Testing" },
            Hours = (decimal)4.0,
            Comments = "Different work",
            SpentOn = new DateTime(2023, 1, 2).Date,
            CreatedOn = new DateTime(2023, 1, 2).Date,
            UpdatedOn = new DateTime(2023, 1, 2).Date
        };
    }
}