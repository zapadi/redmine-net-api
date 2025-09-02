using System;
using System.Collections.Generic;
using Padi.RedmineApi.Types;
using Xunit;

namespace Padi.RedmineAPI.Tests.Equality;

public sealed class JournalEqualityTests
{
    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        var journal = CreateSampleJournal();
        Assert.True(journal.Equals(journal));
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var journal = CreateSampleJournal();
        Assert.False(journal.Equals(null));
    }

    [Theory]
    [MemberData(nameof(GetDifferentJournals))]
    public void Equals_DifferentProperties_ReturnsFalse(Journal journal1, Journal journal2, string propertyName)
    {
        Assert.False(journal1.Equals(journal2), $"Journals should not be equal when {propertyName} is different");
    }

    public static IEnumerable<object[]> GetDifferentJournals()
    {
        var baseJournal = CreateSampleJournal();

        // Different Notes
        var differentNotes = CreateSampleJournal();
        differentNotes.Notes = "Different notes";
        yield return [baseJournal, differentNotes, "Notes"];

        // Different User
        var differentUser = CreateSampleJournal();
        differentUser.User = new IdentifiableName { Id = 999, Name = "Different User" };
        yield return [baseJournal, differentUser, "User"];

        // Different Details
        var differentDetails = CreateSampleJournal();
        differentDetails.Details[0].NewValue = "Different value";
        yield return [baseJournal, differentDetails, "Details"];
    }

    private static Journal CreateSampleJournal()
    {
        return new Journal
        {
            Id = 1,
            User = new IdentifiableName { Id = 1, Name = "John Doe" },
            Notes = "Test notes",
            CreatedOn = new DateTime(2025,02,14,14,04,00),
            PrivateNotes = true,
            Details = 
            [
                new Detail
                {
                    Property = "status_id",
                    Name = "Status",
                    OldValue = "1",
                    NewValue = "2"
                }
            ]
        };
    }
}