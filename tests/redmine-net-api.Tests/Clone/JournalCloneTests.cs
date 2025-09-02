using System;
using System.Collections.Generic;
using Padi.RedmineApi.Types;
using Xunit;

namespace Padi.RedmineAPI.Tests.Clone;

public sealed class JournalCloneTests
{
    [Fact]
    public void Clone_WithPopulatedProperties_ReturnsDeepCopy()
    {
        // Arrange
        var journal = new Journal
        {
            Id = 1,
            User = new IdentifiableName(1, "John Doe"),
            Notes = "Test notes",
            CreatedOn = DateTime.Now,
            PrivateNotes = true,
            Details = (List<Detail>)
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

        // Act
        var clone = journal.Clone(false);

        // Assert
        Assert.NotNull(clone);
        Assert.NotSame(journal, clone);
        Assert.Equal(journal.Id, clone.Id);
        Assert.Equal(journal.Notes, clone.Notes);
        Assert.Equal(journal.CreatedOn, clone.CreatedOn);
        Assert.Equal(journal.PrivateNotes, clone.PrivateNotes);

        Assert.NotSame(journal.User, clone.User);
        Assert.Equal(journal.User.Id, clone.User.Id);
        Assert.Equal(journal.User.Name, clone.User.Name);

        Assert.NotNull(clone.Details);
        Assert.NotSame(journal.Details, clone.Details);
        Assert.Equal(journal.Details.Count, clone.Details.Count);

        var originalDetail = journal.Details[0];
        var clonedDetail = clone.Details[0];
        Assert.NotSame(originalDetail, clonedDetail);
        Assert.Equal(originalDetail.Property, clonedDetail.Property);
        Assert.Equal(originalDetail.Name, clonedDetail.Name);
        Assert.Equal(originalDetail.OldValue, clonedDetail.OldValue);
        Assert.Equal(originalDetail.NewValue, clonedDetail.NewValue);
    }
}