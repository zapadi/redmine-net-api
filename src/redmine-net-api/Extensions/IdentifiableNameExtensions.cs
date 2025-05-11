using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Types;
    
namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class IdentifiableNameExtensions
    {
        /// <summary>
        /// Converts an object of type <typeparamref name="T"/> into an <see cref="IdentifiableName"/> object.
        /// </summary>
        /// <typeparam name="T">The type of the entity to convert. Expected to be one of the supported Redmine entity types.</typeparam>
        /// <param name="entity">The entity object to be converted into an <see cref="IdentifiableName"/>.</param>
        /// <returns>An <see cref="IdentifiableName"/> object populated with the identifier and name of the specified entity, or null if the entity type is not supported.</returns>
        public static IdentifiableName ToIdentifiableName<T>(this T entity) where T : class
        {
            return entity switch
            {
                CustomField customField => IdentifiableName.Create<CustomField>(customField.Id, customField.Name),
                CustomFieldRole customFieldRole => IdentifiableName.Create<CustomFieldRole>(customFieldRole.Id, customFieldRole.Name),
                DocumentCategory documentCategory => IdentifiableName.Create<DocumentCategory>(documentCategory.Id, documentCategory.Name),
                Group group => IdentifiableName.Create<Group>(group.Id, group.Name),
                GroupUser groupUser => IdentifiableName.Create<GroupUser>(groupUser.Id, groupUser.Name),
                Issue issue => new IdentifiableName(issue.Id, issue.Subject),
                IssueAllowedStatus issueAllowedStatus => IdentifiableName.Create<IssueAllowedStatus>(issueAllowedStatus.Id, issueAllowedStatus.Name),
                IssueCustomField issueCustomField => IdentifiableName.Create<IssueCustomField>(issueCustomField.Id, issueCustomField.Name),
                IssuePriority issuePriority => IdentifiableName.Create<IssuePriority>(issuePriority.Id, issuePriority.Name),
                IssueStatus issueStatus => IdentifiableName.Create<IssueStatus>(issueStatus.Id, issueStatus.Name),
                MembershipRole membershipRole => IdentifiableName.Create<MembershipRole>(membershipRole.Id, membershipRole.Name),
                MyAccountCustomField myAccountCustomField => IdentifiableName.Create<MyAccountCustomField>(myAccountCustomField.Id, myAccountCustomField.Name),
                Project project => IdentifiableName.Create<Project>(project.Id, project.Name),
                ProjectEnabledModule projectEnabledModule => IdentifiableName.Create<ProjectEnabledModule>(projectEnabledModule.Id, projectEnabledModule.Name),
                ProjectIssueCategory projectIssueCategory => IdentifiableName.Create<ProjectIssueCategory>(projectIssueCategory.Id, projectIssueCategory.Name),
                ProjectTimeEntryActivity projectTimeEntryActivity => IdentifiableName.Create<ProjectTimeEntryActivity>(projectTimeEntryActivity.Id, projectTimeEntryActivity.Name),
                ProjectTracker projectTracker => IdentifiableName.Create<ProjectTracker>(projectTracker.Id, projectTracker.Name),
                Query query => IdentifiableName.Create<Query>(query.Id, query.Name),
                Role role => IdentifiableName.Create<Role>(role.Id, role.Name),
                TimeEntryActivity timeEntryActivity => IdentifiableName.Create<TimeEntryActivity>(timeEntryActivity.Id, timeEntryActivity.Name),
                Tracker tracker => IdentifiableName.Create<Tracker>(tracker.Id, tracker.Name),
                UserGroup userGroup => IdentifiableName.Create<UserGroup>(userGroup.Id, userGroup.Name),
                Version version => IdentifiableName.Create<Version>(version.Id, version.Name),
                Watcher watcher => IdentifiableName.Create<Watcher>(watcher.Id, watcher.Name),
                _ => null
            };
        }


        /// <summary>
        /// Converts an integer value to an <see cref="IdentifiableName"/> object.
        /// </summary>
        /// <param name="val">An integer value representing the identifier. Must be greater than zero.</param>
        /// <returns>An <see cref="IdentifiableName"/> object with the specified identifier and a null name.</returns>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineException">Thrown when the given value is less than or equal to zero.</exception>
        public static IdentifiableName ToIdentifier(this int val)
        {
            if (val <= 0)
            {
                throw new RedmineException(nameof(val), "Value must be greater than zero");
            }

            return new IdentifiableName(val, null);
        }

        /// <summary>
        /// Converts an integer value into an <see cref="IssueStatus"/> object.
        /// </summary>
        /// <param name="val">The integer value representing the ID of an issue status.</param>
        /// <returns>An <see cref="IssueStatus"/> object initialized with the specified identifier.</returns>
        /// <exception cref="RedmineException">Thrown when the specified value is less than or equal to zero.</exception>
        public static IssueStatus ToIssueStatusIdentifier(this int val)
        {
            if (val <= 0)
            {
                throw new RedmineException(nameof(val), "Value must be greater than zero");
            }

            return new IssueStatus(val, null);
        }
    }
}