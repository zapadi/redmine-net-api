namespace Redmine.Net.Api.Types;

/// <summary>
/// 
/// </summary>
public static class Include
{
    /// <summary>
    /// 
    /// </summary>
    public static class Group
    {
        /// <summary>
        /// 
        /// </summary>
        public const string Users = RedmineKeys.USERS;
        
        /// <summary>
        /// Adds extra information about user's memberships and roles on the projects
        /// </summary>
        public const string Memberships = RedmineKeys.MEMBERSHIPS;
    }

    /// <summary>
    /// Associated data that can be retrieved
    /// </summary>
    public static class Issue
    {
        /// <summary>
        /// Specifies whether to include child issues.
        /// This parameter is applicable when retrieving details for a specific issue.
        /// Corresponds to the Redmine API include parameter: <c>children</c>.
        /// </summary>
        public const string Children = RedmineKeys.CHILDREN;

        /// <summary>
        /// Specifies whether to include attachments.
        /// This parameter is applicable when retrieving a list of issues or details for a specific issue.
        /// Corresponds to the Redmine API include parameter: <c>attachments</c>.
        /// </summary>
        public const string Attachments = RedmineKeys.ATTACHMENTS;

        /// <summary>
        /// Specifies whether to include issue relations.
        /// This parameter is applicable when retrieving a list of issues or details for a specific issue.
        /// Corresponds to the Redmine API include parameter: <c>relations</c>.
        /// </summary>
        public const string Relations = RedmineKeys.RELATIONS;

        /// <summary>
        /// Specifies whether to include associated changesets.
        /// This parameter is applicable when retrieving details for a specific issue.
        /// Corresponds to the Redmine API include parameter: <c>changesets</c>.
        /// </summary>
        public const string Changesets = RedmineKeys.CHANGE_SETS;

        /// <summary>
        /// Specifies whether to include journal entries (notes and history).
        /// This parameter is applicable when retrieving details for a specific issue.
        /// Corresponds to the Redmine API include parameter: <c>journals</c>.
        /// </summary>
        public const string Journals = RedmineKeys.JOURNALS;

        /// <summary>
        /// Specifies whether to include watchers of the issue.
        /// This parameter is applicable when retrieving details for a specific issue.
        /// Corresponds to the Redmine API include parameter: <c>watchers</c>.
        /// </summary>
        public const string Watchers = RedmineKeys.WATCHERS;

        /// <summary>
        /// Specifies whether to include allowed statuses of the issue.
        /// This parameter is applicable when retrieving details for a specific issue.
        /// Corresponds to the Redmine API include parameter: <c>watchers</c>.
        /// Since 5.0.x, Returns the available allowed statuses (the same values as provided in the issue edit form) based on:
        /// the issue's current tracker, the issue's current status, and the member's role (the defined workflow);
        /// the existence of any open subtask(s);
        /// the existence of any open blocking issue(s);
        /// the existence of a closed parent issue.
        /// </summary>
        public const string AllowedStatuses = RedmineKeys.ALLOWED_STATUSES;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public static class Project
    {
        /// <summary>
        /// 
        /// </summary>
        public const string Trackers = RedmineKeys.TRACKERS;

        /// <summary>
        /// since 2.6.0
        /// </summary>
        public const string EnabledModules = RedmineKeys.ENABLED_MODULES;

        /// <summary>
        /// 
        /// </summary>
        public const string IssueCategories = RedmineKeys.ISSUE_CATEGORIES;

        /// <summary>
        /// since 3.4.0
        /// </summary>
        public const string TimeEntryActivities = RedmineKeys.TIME_ENTRY_ACTIVITIES;

        /// <summary>
        /// since 4.2.0
        /// </summary>
        public const string IssueCustomFields = RedmineKeys.ISSUE_CUSTOM_FIELDS;
    }

    /// <summary>
    /// 
    /// </summary>
    public static class User
    {
        /// <summary>
        /// Adds extra information about user's memberships and roles on the projects
        /// </summary>
        public const string Memberships = RedmineKeys.MEMBERSHIPS;

        /// <summary>
        /// Adds extra information about user's groups
        /// added in 2.1
        /// </summary>
        public const string Groups = RedmineKeys.GROUPS;
    }
    
    /// <summary>
    /// 
    /// </summary>
    public static class WikiPage
    {
        /// <summary>
        /// 
        /// </summary>
        public const string Attachments = RedmineKeys.ATTACHMENTS;
    }
}