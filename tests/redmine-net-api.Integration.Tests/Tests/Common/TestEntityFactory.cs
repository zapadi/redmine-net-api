using Padi.DotNet.RedmineAPI.Integration.Tests.Helpers;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Integration.Tests.Tests.Common;

public static class TestEntityFactory
{
    public static Issue CreateRandomIssuePayload(
        int projectId = TestConstants.Projects.DefaultProjectId,
        int trackerId = 1,
        int priorityId = 2,
        int statusId = 1,
        string subject = null,
        List<IssueCustomField> customFields = null,
        List<Watcher> watchers = null,
        List<Upload> uploads = null)
        => new()
        {
            Project = projectId.ToIdentifier(),
            Subject = subject ?? RandomHelper.GenerateText(9),
            Description = RandomHelper.GenerateText(18),
            Tracker = trackerId.ToIdentifier(),
            Status = statusId.ToIssueStatusIdentifier(),
            Priority = priorityId.ToIdentifier(),
            CustomFields = customFields,
            Watchers = watchers,
            Uploads = uploads
        };

    public static User CreateRandomUserPayload(UserStatus status = UserStatus.StatusActive, int? authenticationModeId = null,
        EmailNotificationType emailNotificationType = null)
    {
        var user = new Redmine.Net.Api.Types.User
        {
            Login = RandomHelper.GenerateText(12),
            FirstName = RandomHelper.GenerateText(8),
            LastName = RandomHelper.GenerateText(10),
            Email = RandomHelper.GenerateEmail(),
            Password = TestConstants.Users.DefaultPassword,
            AuthenticationModeId = authenticationModeId,
            MailNotification = emailNotificationType?.Name,
            MustChangePassword = false,
            Status = status,
        };

        return user;
    }

    public static Group CreateRandomGroupPayload(string name = null, List<int> userIds = null)
    {
        var group = new Redmine.Net.Api.Types.Group(name ?? RandomHelper.GenerateText(9));
        if (userIds == null || userIds.Count == 0)
        {
            return group;
        }
        foreach (var userId in userIds)
        {
            group.Users = [IdentifiableName.Create<GroupUser>(userId)];
        }
        return group;
    }
    
    public static Group CreateRandomGroupPayload(string name = null, List<GroupUser> userGroups = null)
    {
        var group = new Redmine.Net.Api.Types.Group(name ?? RandomHelper.GenerateText(9));
        if (userGroups == null || userGroups.Count == 0)
        {
            return group;
        }
      
        group.Users = userGroups;
        return group;
    }

    public static (string pageName, WikiPage wikiPage) CreateRandomWikiPagePayload(string pageName = null, int version = 0, List<Upload> uploads = null)
    {
        pageName =  (pageName ?? RandomHelper.GenerateText(8));
        if (char.IsLower(pageName[0]))
        {
            pageName =  char.ToUpper(pageName[0]) + pageName[1..];
        }
        var wikiPage = new WikiPage 
        { 
            Text = RandomHelper.GenerateText(10), 
            Comments = RandomHelper.GenerateText(15),
            Version = version,
            Uploads = uploads,
        };
        
        return (pageName, wikiPage);
    }

    public static Redmine.Net.Api.Types.Version CreateRandomVersionPayload(string name = null,
        VersionStatus status = VersionStatus.Open,
        VersionSharing sharing = VersionSharing.None, 
        int dueDateDays = 30, 
        string wikiPageName = null, 
        float? estimatedHours = null, 
        float? spentHours = null)
    {
        var version = new Redmine.Net.Api.Types.Version
        {
            Name = name ?? RandomHelper.GenerateText(10),
            Description = RandomHelper.GenerateText(15),
            Status = status,
            Sharing = sharing,
            DueDate = DateTime.Now.Date.AddDays(dueDateDays),
            EstimatedHours = estimatedHours,
            SpentHours = spentHours,
            WikiPageTitle = wikiPageName,
        };

        return version;
    }

    public static Redmine.Net.Api.Types.News CreateRandomNewsPayload(string title = null, List<Upload> uploads = null)
    {
        return new Redmine.Net.Api.Types.News()
        {
            Title = title ?? RandomHelper.GenerateText(5),
            Summary = RandomHelper.GenerateText(10),
            Description = RandomHelper.GenerateText(20),
            Uploads = uploads
        };
    }

    public static IssueCustomField CreateRandomIssueCustomFieldWithMultipleValuesPayload()
    {
        return IssueCustomField.CreateMultiple(1, RandomHelper.GenerateText(8),
            [RandomHelper.GenerateText(4), RandomHelper.GenerateText(4)]);
    }
    
    public static IssueCustomField CreateRandomIssueCustomFieldWithSingleValuePayload()
    {
        return IssueCustomField.CreateSingle(1, RandomHelper.GenerateText(8), RandomHelper.GenerateText(4));
    }
    
    public static IssueRelation CreateRandomIssueRelationPayload(int issueId, int issueToId, IssueRelationType issueRelationType = IssueRelationType.Relates)
    {
        return new IssueRelation { IssueId = issueId, IssueToId = issueToId, Type = issueRelationType };;
    }

    public static Redmine.Net.Api.Types.TimeEntry CreateRandomTimeEntryPayload(int projectId, int issueId, DateTime? spentOn = null, decimal hours = 1.5m, int? activityId = null)
    {
        var timeEntry = new Redmine.Net.Api.Types.TimeEntry
        {
            Project = projectId.ToIdentifier(),
            Issue = issueId.ToIdentifier(),
            SpentOn = spentOn ?? DateTime.Now.Date,
            Hours = hours,
            Comments = RandomHelper.GenerateText(10),
        };

        if (activityId != null)
        {
            timeEntry.Activity = activityId.Value.ToIdentifier();
        }
        
        return timeEntry;
    }
}