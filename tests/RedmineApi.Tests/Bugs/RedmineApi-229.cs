using System;
using Padi.RedmineApi.Types;
using Xunit;

namespace Padi.RedmineAPI.Tests.Bugs;

public sealed class RedmineApi229
{
   [Fact]
    public void Equals_ShouldReturnTrue_WhenComparingWithSelf()
    {
        // Arrange
        var timeEntry = CreateSampleTimeEntry();

        // Act & Assert
        Assert.True(timeEntry.Equals(timeEntry), "TimeEntry should equal itself (reference equality)");
        Assert.True(timeEntry == timeEntry, "TimeEntry should equal itself using == operator");
        Assert.True(timeEntry.Equals((object)timeEntry), "TimeEntry should equal itself when cast to object");
        Assert.Equal(timeEntry.GetHashCode(), timeEntry.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenComparingIdenticalInstances()
    {
        // Arrange
        var timeEntry1 = CreateSampleTimeEntry();
        var timeEntry2 = CreateSampleTimeEntry();

        // Act & Assert
        Assert.True(timeEntry1.Equals(timeEntry2), "Identical TimeEntry instances should be equal");
        Assert.True(timeEntry2.Equals(timeEntry1), "Equality should be symmetric");
        Assert.Equal(timeEntry1.GetHashCode(), timeEntry2.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparingWithNull()
    {
        // Arrange
        var timeEntry = CreateSampleTimeEntry();

        // Act & Assert
        Assert.False(timeEntry.Equals(null));
        Assert.False(timeEntry.Equals((object)null));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparingDifferentTypes()
    {
        // Arrange
        var timeEntry = CreateSampleTimeEntry();
        var differentObject = new object();

        // Act & Assert
        Assert.False(timeEntry.Equals(differentObject));
    }

    [Theory]
    [MemberData(nameof(GetDifferentTimeEntries))]
    public void Equals_ShouldReturnFalse_WhenPropertiesDiffer(TimeEntry different, string propertyName)
    {
        // Arrange
        var baseline = CreateSampleTimeEntry();

        // Act & Assert
        Assert.False(baseline.Equals(different), $"TimeEntries should not be equal when {propertyName} differs");
    }

    private static TimeEntry CreateSampleTimeEntry() => new()
    {
        Id = 1,
        Project = new IdentifiableName { Id = 1, Name = "Project" },
        Issue = new IdentifiableName { Id = 1, Name = "Issue" },
        User = new IdentifiableName { Id = 1, Name = "User" },
        Activity = new IdentifiableName { Id = 1, Name = "Activity" },
        Hours = (decimal)8.0,
        Comments = "Test comment",
        SpentOn = new DateTime(2023, 1, 1),
        CreatedOn = new DateTime(2023, 1, 1),
        UpdatedOn = new DateTime(2023, 1, 1),
        CustomFields =
        [
            new() { Id = 1, Name = "Field1"}
        ]
    };

    public static TheoryData<TimeEntry, string> GetDifferentTimeEntries()
    {
        var data = new TheoryData<TimeEntry, string>();

        // Different ID
        var differentId = CreateSampleTimeEntry();
        differentId.Id = 2;
        data.Add(differentId, "Id");

        // Different Project
        var differentProject = CreateSampleTimeEntry();
        differentProject.Project = new IdentifiableName { Id = 2, Name = "Different Project" };
        data.Add(differentProject, "Project");

        // Different Issue
        var differentIssue = CreateSampleTimeEntry();
        differentIssue.Issue = new IdentifiableName { Id = 2, Name = "Different Issue" };
        data.Add(differentIssue, "Issue");

        // Different Hours
        var differentHours = CreateSampleTimeEntry();
        differentHours.Hours = (decimal)4.0;
        data.Add(differentHours, "Hours");

        // Different CustomFields
        var differentCustomFields = CreateSampleTimeEntry();
        differentCustomFields.CustomFields =
        [
            new() { Id = 2, Name = "Field2" }
        ];
        data.Add(differentCustomFields, "CustomFields");

        // Different SpentOn
        var differentSpentOn = CreateSampleTimeEntry();
        differentSpentOn.SpentOn = new DateTime(2023, 1, 2);
        data.Add(differentSpentOn, "SpentOn");

        return data;
    }
}