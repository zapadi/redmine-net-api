namespace Redmine.Net.Api
{
    public static class RedmineKeys
    {
        public const string ISSUE_PRIORITIES = "issue_priorities";
        public const string TIME_ENTRY_ACTIVITIES = "time_entry_activities";

        public const string ID = "id";
        public const string NAME = "name";
        public const string CREATED_ON = "created_on";
        public const string UPDATED_ON = "updated_on";
        public const string CUSTOM_FIELD = "custom_field";
        public const string CUSTOM_FIELDS = "custom_fields";
        public const string TRACKER = "tracker";
        public const string TRACKERS = "trackers";
        public const string TRACKER_IDS = "tracker_ids";

        public const string SUBJECT = "subject";

        #region Project
        public const string PROJECT = "project";
        public const string IDENTIFIER = "identifier";
        public const string DESCRIPTION = "description";
        public const string PARENT = "parent";
        public const string PARENT_ID = "parent_id";
        public const string HOMEPAGE = "homepage";
        public const string STATUS = "status";
        public const string IS_PUBLIC = "is_public";
        public const string INHERIT_MEMBERS = "inherit_members";
        
        public const string ENABLED_MODULE = "enabled_module";
        public const string ENABLED_MODULES = "enabled_modules";
        public const string ENABLED_MODULE_NAMES = "enabled_module_names";
        public const string ISSUE_CATEGORY = "issue_category";
        public const string ISSUE_CATEGORIES = "issue_categories";
        #endregion

        #region Issue

        public const string ISSUE = "issue";

        #endregion

        #region IssueStatus

        public const string ISSUE_STATUS = "issue_status";
        public const string IS_CLOSED = "is_closed";
        public const string IS_DEFAULT = "is_default";
        #endregion

        public const string PERMISSION = "permission";
        public const string PERMISSIONS = "permissions";

        public const string ERROR = "error";

        public const string WIKI_PAGE = "wiki_page";
        public const string TITLE = "title";
        public const string TEXT = "text";
        public const string AUTHOR = "author";
        public const string VERSION = "version";
        public const string COMMENTS = "comments";
        public const string ATTACHMENT = "attachment";
        public const string ATTACHMENTS = "attachments";

        public const string USER = "user";
        public const string USERS = "users";
        public const string USER_ID = "user_id";
        public const string USER_IDS = "user_ids";

        public const string DUE_DATE = "due_date";
        public const string SHARING = "sharing";

        public const string GROUP = "group";

        public const string LOGIN = "login";
        public const string PASSWORD = "password";
        public const string FIRSTNAME = "firstname";
        public const string LASTNAME = "lastname";

        public const string AUTH_SOURCE_ID = "auth_source_id";
        public const string LAST_LOGIN_ON = "last_login_on";
        public const string API_KEY = "api_key";
        public const string MUST_CHANGE_PASSWD = "must_change_passwd";

        public const string MEMBERSHIPS = "memberships";
        public const string MEMBERSHIP = "membership";
        public const string GROUPS = "groups";
        public const string MAIL = "mail";

        public const string UPLOAD = "upload";
        public const string TOKEN = "token";
        public const string FILENAME = "filename";
        public const string CONTENT_TYPE = "content_type";

        public const string TIME_ENTRY_ACTIVITY = "time_entry_activity";
        

        public const string TIME_ENTRY = "time_entry";
        public const string SPENT_ON = "spent_on";
        public const string HOURS = "hours";
        public const string ACTIVITY = "activity";
        public const string ACTIVITY_ID = "activity_id";
        public const string PROJECT_ID = "project_id";
        public const string ISSUE_ID = "issue_id";

        public const string ROLE = "role";
        public const string ROLE_ID = "role_id";
        public const string ROLES = "roles";
        public const string ROLE_IDS = "role_ids";

        public const string QUERY = "query";

        public const string NEWS = "news";
        public const string SUMMARY = "summary";

        public const string INHERITED = "inherited";

        public const string JOURNAL = "journal";
        public const string DETAIL = "detail";
        public const string DETAILS = "details";
        public const string NOTES = "notes";

        public const string ISSUE_PRIORITY = "issue_priority";

        public const string VALUE = "value";
        public const string MULTIPLE = "multiple";

        public const string ASSIGNED_TO = "assigned_to";
        public const string ASSIGNED_TO_ID = "assigned_to_id";

        public const string FILESIZE = "filesize";
        public const string CONTENT_URL = "content_url";

        public const string COMMITTED_ON = "committed_on";
        public const string REVISION = "revision";
        public const string CHANGESET = "changeset";

        public const string POSSIBLE_VALUE = "possible_value";
        public const string POSSIBLE_VALUES = "possible_values";

        public const string CUSTOMIZED_TYPE = "customized_type";
        public const string FIELD_FORMAT = "field_format";
        public const string REGEXP = "regexp";
        public const string MIN_LENGTH = "min_length";
        public const string MAX_LENGTH = "max_length";
        public const string IS_REQUIRED = "is_required";
        public const string IS_FILTER = "is_filter";
        public const string SEARCHABLE = "searchable";
        public const string DEFAULT_VALUE = "default_value";

        public const string VISIBLE = "visible";
        public const string PROPERTY = "property";
        public const string OLD_VALUE = "old_value";
        public const string NEW_VALUE = "new_value";
        public const string DELAY = "delay";

        public const string WATCHER_USER_IDS = "watcher_user_ids";
        public const string UPLOADS = "uploads";
        public const string START_DATE = "start_date";
        public const string DONE_RATIO = "done_ratio";
        public const string ESTIMATED_HOURS = "estimated_hours";
        public const string FIXED_VERSION_ID = "fixed_version_id";
        public const string PARENT_ISSUE_ID = "parent_issue_id";
        public const string TRACKER_ID = "tracker_id";
        public const string CATEGORY_ID = "category_id";

        public const string STATUS_ID = "status_id";
        public const string PRIORITY_ID = "priority_id";
        public const string IS_PRIVATE = "is_private";
        public const string PRIVATE_NOTES = "private_notes";
        public const string SPENT_HOURS = "spent_hours";
        public const string CLOSED_ON = "closed_on";
        public const string RELATIONS = "relations";
        public const string JOURNALS = "journals";
        public const string CHANGESETS = "changesets";

        public const string PRIORITY = "priority";
        public const string CATEGORY = "category";
        public const string FIXED_VERSION = "fixed_version";
        public const string RELATION = "relation";
        public const string CHILDREN = "children";
        public const string WATCHERS = "watchers";
        public const string WATCHER = "watcher";
        public const string ISSUE_TO_ID = "issue_to_id";
        public const string RELATION_TYPE = "relation_type";
    }
}