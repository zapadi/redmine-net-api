using System;
using System.Collections.Generic;
using System.Net;
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Serialization.Xml.Converters;
using Padi.RedmineApi.Types;
using Attachment = System.Net.Mail.Attachment;
using Version = Padi.RedmineApi.Types.Version;

namespace Padi.RedmineApi.Serialization.Xml;

internal static class XmlConverterRegistry
{
    private static readonly Dictionary<Type, object> Converters = new()
    {
        { typeof(Attachment), new AttachmentConverter() },
        { typeof(CustomField), new CustomFieldConverter() },
        { typeof(CustomFieldPossibleValue), new CustomFieldPossibleValueConverter() },
        { typeof(CustomFieldRole), new CustomFieldRoleConverter() },
        { typeof(CustomFieldValue), new CustomFieldValueConverter() },
        { typeof(ChangeSet), new ChangeSetConverter() },
        { typeof(DocumentCategory), new DocumentCategoryConverter() },
        { typeof(Detail), new DetailConverter() },
        { typeof(Error), new ErrorConverter() },
        { typeof(WebRequestMethods.File), new FileConverter() },
        { typeof(Group), new GroupConverter() },
        { typeof(GroupUser), new GroupUserConverter() },
        { typeof(IdentifiableName), new IdentifiableNameConverter() },
        { typeof(Issue), new IssueConverter() },
        { typeof(IssueAllowedStatus), new IssueAllowedStatusConverter() },
        { typeof(IssueCategory), new IssueCategoryConverter() },
        { typeof(IssueChild), new IssueChildConverter() },
        { typeof(IssueCustomField), new IssueCustomFieldConverter() },
        { typeof(IssuePriority), new IssuePriorityConverter() },
        { typeof(IssueRelation), new IssueRelationConverter() },
        { typeof(IssueStatus), new IssueStatusConverter() },
        { typeof(Journal), new JournalConverter() },
        { typeof(Membership), new MembershipConverter() },
        { typeof(MembershipRole), new MembershipRoleConverter() },
        { typeof(MyAccount), new MyAccountConverter() },
        { typeof(MyAccountCustomField), new MyAccountCustomFieldConverter() },
        { typeof(News), new NewsConverter() },
        { typeof(NewsComment), new NewsCommentConverter() },
        { typeof(Permission), new PermissionConverter() },
        { typeof(Project), new ProjectConverter() },
        { typeof(ProjectMembership), new ProjectMembershipConverter() },
        { typeof(ProjectEnabledModule), new ProjectEnabledModuleConverter() },
        { typeof(ProjectIssueCategory), new ProjectIssueCategoryConverter() },
        { typeof(ProjectTimeEntryActivity), new ProjectTimeEntryActivityConverter() },
        { typeof(ProjectTracker), new ProjectTrackerConverter() },
        { typeof(Query), new QueryConverter() },
        { typeof(Role), new RoleConverter() },
        { typeof(Search), new SearchConverter() },
        { typeof(TimeEntryActivity), new TimeEntryActivityConverter() },
        { typeof(TimeEntry), new TimeEntryConverter() },
        { typeof(Tracker), new TrackerConverter() },
        { typeof(TrackerCoreField), new TrackerCoreFieldConverter() },
        { typeof(TrackerCustomField), new TrackerCustomFieldConverter() },
        { typeof(User), new UserConverter() },
        { typeof(UserGroup), new UserGroupConverter() },
        { typeof(Upload), new UploadConverter() },
        { typeof(Watcher), new WatcherConverter() },
        { typeof(WikiPage), new WikiPageConverter() },
        { typeof(Version), new VersionConverter() }
    };

    public static IXmlRedmineConverter<T> GetConverter<T>()
    {
        if (Converters.TryGetValue(typeof(T), out var converter))
        {
            return (IXmlRedmineConverter<T>)converter;
        }

        throw new RedmineException(RedmineApiErrorCode.SerializerError, $"No converter defined for '{typeof(T).Name}'");
    }
}