using System;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class IssueEqualityTests
{
    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        var issue = CreateSampleIssue();
        Assert.True(issue.Equals(issue));
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var issue = CreateSampleIssue();
        Assert.False(issue.Equals(null));
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var issue = CreateSampleIssue();
        var differentObject = new object();
        Assert.False(issue.Equals(differentObject));
    }

    [Fact]
    public void Equals_IdenticalProperties_ReturnsTrue()
    {
        var issue1 = CreateSampleIssue();
        var issue2 = CreateSampleIssue();
        Assert.True(issue1.Equals(issue2));
        Assert.True(issue2.Equals(issue1));
    }

    [Fact]
    public void GetHashCode_SameProperties_ReturnsSameValue()
    {
        var issue1 = CreateSampleIssue();
        var issue2 = CreateSampleIssue();
        Assert.Equal(issue1.GetHashCode(), issue2.GetHashCode());
    }

    [Fact]
    public void OperatorEquals_SameObjects_ReturnsTrue()
    {
        var issue1 = CreateSampleIssue();
        var issue2 = CreateSampleIssue();
        Assert.True(issue1 == issue2);
    }

    [Fact]
    public void OperatorNotEquals_DifferentObjects_ReturnsTrue()
    {
        var issue1 = CreateSampleIssue();
        var issue2 = CreateSampleIssue();
        issue2.Subject = "Different Subject";
        Assert.True(issue1 != issue2);
    }

    [Fact]
    public void Equals_NullCollections_ReturnsTrue()
    {
        var issue1 = CreateSampleIssue();
        var issue2 = CreateSampleIssue();
        issue1.CustomFields = null;
        issue2.CustomFields = null;
        Assert.True(issue1.Equals(issue2));
    }

    [Fact]
    public void Equals_DifferentCollectionSizes_ReturnsFalse()
    {
        var issue1 = CreateSampleIssue();
        var issue2 = CreateSampleIssue();
        issue2.CustomFields.Add(new IssueCustomField { Id = 2, Name = "Additional Field" });
        Assert.False(issue1.Equals(issue2));
    }
    
    private static Issue CreateSampleIssue()
    {
        return new Issue
        {
            Id = 1,
            Project = new IdentifiableName { Id = 100, Name = "Test Project" },
            Tracker = new IdentifiableName { Id = 1, Name = "Bug" },
            Status = new IssueStatus { Id = 1, Name = "New" },
            Priority = new IdentifiableName { Id = 1, Name = "Normal" },
            Author = new IdentifiableName { Id = 1, Name = "John Doe" },
            Subject = "Test Issue",
            Description = "Test Description",
            StartDate = new DateTime(2025, 02,02,10,10,10).Date,
            DueDate = new DateTime(2025, 02,02,10,10,10).Date.AddDays(7),
            DoneRatio = 0,
            IsPrivate = false,
            EstimatedHours = 8.5f,
            CreatedOn = new DateTime(2025, 02,02,10,10,10),
            UpdatedOn = new DateTime(2025, 02,04,15,10,5),
            CustomFields =
            [
                new IssueCustomField
                {
                    Id = 1,
                    Name = "Custom Field 1",
                    Values = [new CustomFieldValue("Value 1")]
                }
            ]
        };
    }
}