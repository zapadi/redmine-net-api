using Padi.RedmineAPI.Integration.Tests.Fixtures;
using Padi.RedmineAPI.Integration.Tests.Helpers;
using Padi.RedmineAPI.Integration.Tests.Infrastructure;
using Redmine.Net.Api;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Net;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineAPI.Integration.Tests.Tests.Types.Async;

[Collection(Constants.RedmineTestContainerCollection)]
public class ApiCoverageTestsAsync(RedmineTestContainerFixture fixture)
{
    private static User NewUser(bool isAdmin = false, UserStatus status = UserStatus.StatusActive)
    {
        return new User
        {
            Login = RandomHelper.GenerateText(12),
            FirstName = RandomHelper.GenerateText(8),
            LastName = RandomHelper.GenerateText(10),
            Email = $"{RandomHelper.GenerateText(5)}.{RandomHelper.GenerateText(4)}@gmail.com",
            Password = "password123",
            IsAdmin = isAdmin,
            Status = status
        };
    }

    [Fact]
    public async Task CreateUser_With_IsAdmin_Should_Create_Admin()
    {
        var ct = TestContext.Current.CancellationToken;

        var created = await fixture.RedmineManager.CreateAsync(NewUser(isAdmin: true), cancellationToken: ct);
        var user = await fixture.RedmineManager.GetAsync<User>(created.Id.ToInvariantString(), cancellationToken: ct);

        Assert.True(user.IsAdmin);
    }

    [Fact]
    public async Task UpdateUser_Should_Revoke_Admin()
    {
        var ct = TestContext.Current.CancellationToken;
        var created = await fixture.RedmineManager.CreateAsync(NewUser(isAdmin: true), cancellationToken: ct);
        var user = await fixture.RedmineManager.GetAsync<User>(created.Id.ToInvariantString(), cancellationToken: ct);

        user.IsAdmin = false;
        await fixture.RedmineManager.UpdateAsync(user.Id.ToInvariantString(), user, cancellationToken: ct);
        var updated = await fixture.RedmineManager.GetAsync<User>(user.Id.ToInvariantString(), cancellationToken: ct);

        Assert.False(updated.IsAdmin);
    }

    [Fact]
    public async Task CreateUser_Without_Status_Should_Get_Redmine_Default_Status()
    {
        var ct = TestContext.Current.CancellationToken;

        var created = await fixture.RedmineManager.CreateAsync(NewUser(status: UserStatus.StatusAnonymous), cancellationToken: ct);
        var user = await fixture.RedmineManager.GetAsync<User>(created.Id.ToInvariantString(), cancellationToken: ct);

        Assert.Equal(UserStatus.StatusActive, user.Status);
    }

    private async Task<(Issue Issue, Journal Journal)> CreateIssueWithJournalAsync(CancellationToken ct)
    {
        var issue = await fixture.RedmineManager.CreateAsync(IssueTestHelper.CreateIssue(), cancellationToken: ct);
        issue.Notes = RandomHelper.GenerateText(20);
        await fixture.RedmineManager.UpdateAsync(issue.Id.ToInvariantString(), issue, cancellationToken: ct);
        var journal = await GetJournalAsync(issue.Id, null, ct);
        return (issue, journal);
    }

    private async Task<Journal> GetJournalAsync(int issueId, int? journalId, CancellationToken ct)
    {
        var issue = await fixture.RedmineManager.GetAsync<Issue>(issueId.ToInvariantString(), RequestOptions.Include(RedmineKeys.JOURNALS), ct);
        return journalId.HasValue ? Assert.Single(issue.Journals, j => j.Id == journalId.Value) : Assert.Single(issue.Journals);
    }

    [Fact]
    public async Task UpdateJournal_Should_Change_Notes_And_PrivateNotes()
    {
        var ct = TestContext.Current.CancellationToken;
        var (issue, journal) = await CreateIssueWithJournalAsync(ct);

        await fixture.RedmineManager.UpdateJournalAsync(journal.Id, "edited notes", true, cancellationToken: ct);
        var updated = await GetJournalAsync(issue.Id, journal.Id, ct);
        Assert.Equal("edited notes", updated.Notes);
        Assert.True(updated.PrivateNotes);

        fixture.RedmineManager.UpdateJournal(journal.Id, null, false);
        updated = await GetJournalAsync(issue.Id, journal.Id, ct);
        Assert.Equal("edited notes", updated.Notes);
        Assert.False(updated.PrivateNotes);
    }

    [Fact]
    public async Task Update_Journal_Entity_Should_Change_Notes_And_PrivateNotes()
    {
        var ct = TestContext.Current.CancellationToken;
        var (issue, journal) = await CreateIssueWithJournalAsync(ct);

        await fixture.RedmineManager.UpdateAsync(journal.Id.ToInvariantString(), new Journal { Notes = "entity notes", PrivateNotes = true }, cancellationToken: ct);
        var updated = await GetJournalAsync(issue.Id, journal.Id, ct);

        Assert.Equal("entity notes", updated.Notes);
        Assert.True(updated.PrivateNotes);
    }

    [Fact]
    public async Task DeleteJournalNotes_Should_Clear_Notes()
    {
        var ct = TestContext.Current.CancellationToken;
        var (issue, journal) = await CreateIssueWithJournalAsync(ct);

        await fixture.RedmineManager.DeleteJournalNotesAsync(journal.Id, cancellationToken: ct);

        var refreshed = await fixture.RedmineManager.GetAsync<Issue>(issue.Id.ToInvariantString(), RequestOptions.Include(RedmineKeys.JOURNALS), ct);
        Assert.True(refreshed.Journals == null || refreshed.Journals.All(j => string.IsNullOrEmpty(j.Notes)));
    }

    [Fact]
    public async Task UpdateMyAccount_Should_Change_Account()
    {
        var ct = TestContext.Current.CancellationToken;
        var account = await fixture.RedmineManager.GetMyAccountAsync(cancellationToken: ct);
        var originalLastName = account.LastName;
        var lastName = RandomHelper.GenerateText(10);

        try
        {
            account.LastName = lastName;
            await fixture.RedmineManager.UpdateMyAccountAsync(account, cancellationToken: ct);
            Assert.Equal(lastName, (await fixture.RedmineManager.GetMyAccountAsync(cancellationToken: ct)).LastName);
        }
        finally
        {
            account.LastName = originalLastName;
            fixture.RedmineManager.UpdateMyAccount(account);
        }

        Assert.Equal(originalLastName, (await fixture.RedmineManager.GetMyAccountAsync(cancellationToken: ct)).LastName);
    }
}
