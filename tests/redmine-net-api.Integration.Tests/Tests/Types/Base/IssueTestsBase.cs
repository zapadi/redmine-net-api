using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Base;

public abstract class IssueTestsBase
{
    protected readonly RedmineTestContainerFixture Fixture;
    protected static readonly IdentifiableName ProjectIdName = IdentifiableName.Create<Project>(1);

    protected IssueTestsBase(RedmineTestContainerFixture fixture)
    {
        Fixture = fixture;
    }

    protected static Issue CreateTestIssueData(bool withCustomFields = false)
    {
        var issue = new Issue
        {
            Project = ProjectIdName,
            Subject = RandomHelper.GenerateText(9),
            Description = RandomHelper.GenerateText(18),
            Tracker = 1.ToIdentifier(),
            Status = 1.ToIssueStatusIdentifier(),
            Priority = 2.ToIdentifier(),
        };
        
        if(withCustomFields)
        {
            issue.CustomFields = [IssueCustomField.CreateSingle(1, RandomHelper.GenerateText(8), RandomHelper.GenerateText(4))];
            // [
            //     IssueCustomField.CreateMultiple(1,
            //         RandomHelper.GenerateText(8), [
            //             RandomHelper.GenerateText(4),
            //             RandomHelper.GenerateText(4)
            //         ])
            // ];

        }

        return issue;
    }

    protected static void AssertIssueEquals(Issue expected, Issue actual)
    {
        Assert.NotNull(actual);
        Assert.True(actual.Id > 0);
        Assert.Equal(expected.Subject, actual.Subject);
        Assert.Equal(expected.Description, actual.Description);
        Assert.Equal(expected.Project.Id, actual.Project.Id);
        Assert.Equal(expected.Tracker.Id, actual.Tracker.Id);
        Assert.Equal(expected.Status.Id, actual.Status.Id);
        Assert.Equal(expected.Priority.Id, actual.Priority.Id);
        
        if (expected.StartDate.HasValue)
        {
            Assert.Equal(expected.StartDate, actual.StartDate);
        }
        if (expected.DueDate.HasValue)
        {
            Assert.Equal(expected.DueDate, actual.DueDate);
        }
        if (expected.EstimatedHours.HasValue)
        {
            Assert.Equal(expected.EstimatedHours, actual.EstimatedHours);
        }
    }

    protected static Issue CreateFullIssueData()
    {
        return new Issue
        {
            Project = ProjectIdName,
            Subject = RandomHelper.GenerateText(9),
            Description = RandomHelper.GenerateText(18),
            Tracker = 2.ToIdentifier(),
            Status = 1.ToIssueStatusIdentifier(),
            Priority = 3.ToIdentifier(),
            StartDate = DateTime.Now.Date,
            DueDate = DateTime.Now.Date.AddDays(7),
            EstimatedHours = 8,
            CustomFields =
            [
                IssueCustomField.CreateSingle(1, RandomHelper.GenerateText(8), RandomHelper.GenerateText(4)) 
            ]
        };
    }
}
