namespace Redmine.Net.Api.Types
{
    public enum IssueRelationType
    {
        relates = 1,
        duplicates,
        duplicated,
        blocks,
        blocked,
        precedes,
        follows,
        copied_to,
        copied_from
    }
}