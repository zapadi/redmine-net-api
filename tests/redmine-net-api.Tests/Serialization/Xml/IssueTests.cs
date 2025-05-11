using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure.Fixtures;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization.Xml;

[Collection(Constants.XmlRedmineSerializerCollection)]
public class IssueTests(XmlSerializerFixture fixture)
{
    [Fact]
    public void Should_Deserialize_Issues()
    {
        const string input = """ 
        <?xml version="1.0" encoding="UTF-8"?>
        <issues type="array" total_count="1640">
            <issue>
                <id>4326</id>
                <project name="Redmine" id="1"/>
                <tracker name="Feature" id="2"/>
                <status name="New" id="1"/>
                <priority name="Normal" id="4"/>
                <author name="John Smith" id="10106"/>
                <category name="Email notifications" id="9"/>
                <subject>Aggregate Multiple Issue Changes for Email Notifications</subject>
                <description>
                 This is not to be confused with another useful proposed feature that
                would do digest emails for notifications.
                </description>
                <start_date>2009-12-03</start_date>
                <due_date></due_date>
                <done_ratio>0</done_ratio>
                <estimated_hours></estimated_hours>
                <created_on>Thu Dec 03 15:02:12 +0100 2009</created_on>
                <updated_on>Sun Jan 03 12:08:41 +0100 2010</updated_on>
            </issue>
            <issue>
                <id>4325</id>
                <journals type="array">
                </journals>
                <changesets type="array">
                </changesets>
                <custom_fields type="array">
                </custom_fields>
            </issue>
        </issues>
        """;
        
        var output = fixture.Serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.Issue>(input);
        
        Assert.NotNull(output);
        Assert.Equal(1640, output.TotalItems);

        var issues = output.Items.ToList();
        Assert.Equal(4326, issues[0].Id);
        Assert.Equal("Redmine", issues[0].Project.Name);
        Assert.Equal(1, issues[0].Project.Id);
        Assert.Equal("Feature", issues[0].Tracker.Name);
        Assert.Equal(2, issues[0].Tracker.Id);
        Assert.Equal("New", issues[0].Status.Name);
        Assert.Equal(1, issues[0].Status.Id);
        Assert.Equal("Normal", issues[0].Priority.Name);
        Assert.Equal(4, issues[0].Priority.Id);
        Assert.Equal("John Smith", issues[0].Author.Name);
        Assert.Equal(10106, issues[0].Author.Id);
        Assert.Equal("Email notifications", issues[0].Category.Name);
        Assert.Equal(9, issues[0].Category.Id);
        Assert.Equal("Aggregate Multiple Issue Changes for Email Notifications", issues[0].Subject);
        Assert.Contains("This is not to be confused with another useful proposed feature", issues[0].Description);
        Assert.Equal(new DateTime(2009, 12, 3), issues[0].StartDate);
        Assert.Null(issues[0].DueDate);
        Assert.Equal(0, issues[0].DoneRatio);
        Assert.Null(issues[0].EstimatedHours);
        // Assert.Equal(new DateTime(2009, 12, 3, 14, 2, 12, DateTimeKind.Utc).ToLocalTime(), issues[0].CreatedOn);
        // Assert.Equal(new DateTime(2010, 1, 3, 11, 8, 41, DateTimeKind.Utc).ToLocalTime(), issues[0].UpdatedOn);

        Assert.Equal(4325, issues[1].Id);
        Assert.Null(issues[1].Journals);
        Assert.Null(issues[1].ChangeSets);
        Assert.Null(issues[1].CustomFields);
    }
    
    [Fact]
    public void Should_Deserialize_Issues_With_CustomFields()
    {
        const string input = """ 
        <?xml version="1.0" encoding="UTF-8"?>
        <issues type="array" count="1640">
            <issue>
                <id>4326</id>
                <project name="Redmine" id="1"/>
                <tracker name="Feature" id="2"/>
                <status name="New" id="1"/>
                <priority name="Normal" id="4"/>
                <author name="John Smith" id="10106"/>
                <category name="Email notifications" id="9"/>
                <subject>
                Aggregate Multiple Issue Changes for Email Notifications
                </subject>
                <description>
                 This is not to be confused with another useful proposed feature that
                would do digest emails for notifications.
                </description>
                <start_date>2009-12-03</start_date>
                <due_date></due_date>
                <done_ratio>0</done_ratio>
                <estimated_hours></estimated_hours>
                <custom_fields>
                    <custom_field name="Resolution" id="2">Duplicate</custom_field>
                    <custom_field name="Texte" id="5">Test</custom_field>
                    <custom_field name="Boolean" id="6">1</custom_field>
                    <custom_field name="Date" id="7">2010-01-12</custom_field>
                </custom_fields>
                <created_on>Thu Dec 03 15:02:12 +0100 2009</created_on>
                <updated_on>Sun Jan 03 12:08:41 +0100 2010</updated_on>
            </issue>
            <issue>
                <id>4325</id>
                <journals type="array"/>
                <changesets type="array"/>
                <custom_fields type="array">
                    <custom_field name="Affected version" id="1">
                        <value>1.0.1</value>
                    </custom_field>
                    <custom_field name="Resolution" id="2">
                        <value>Fixed</value>
                    </custom_field>
                </custom_fields>
            </issue>
        </issues>
        """;
        
        var output = fixture.Serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.Issue>(input);
        
        Assert.NotNull(output);
        Assert.Equal(2, output.TotalItems);

        var issues = output.Items.ToList();
        Assert.Equal(4326, issues[0].Id);
        Assert.Equal("Redmine", issues[0].Project.Name);
        Assert.Equal(1, issues[0].Project.Id);
        Assert.Equal("Feature", issues[0].Tracker.Name);
        Assert.Equal(2, issues[0].Tracker.Id);
        Assert.Equal("New", issues[0].Status.Name);
        Assert.Equal(1, issues[0].Status.Id);
        Assert.Equal("Normal", issues[0].Priority.Name);
        Assert.Equal(4, issues[0].Priority.Id);
        Assert.Equal("John Smith", issues[0].Author.Name);
        Assert.Equal(10106, issues[0].Author.Id);
        Assert.Equal("Email notifications", issues[0].Category.Name);
        Assert.Equal(9, issues[0].Category.Id);
        Assert.Contains("Aggregate Multiple Issue Changes for Email Notifications", issues[0].Subject);
        Assert.Contains("This is not to be confused with another useful proposed feature", issues[0].Description);
        Assert.Equal(new DateTime(2009, 12, 3), issues[0].StartDate);
        Assert.Null(issues[0].DueDate);
        Assert.Equal(0, issues[0].DoneRatio);
        Assert.Null(issues[0].EstimatedHours);
        
        Assert.NotNull(issues[0].CustomFields);
        var issueCustomFields = issues[0].CustomFields.ToList();
        Assert.Equal(4, issueCustomFields.Count);
        
        Assert.Equal(2,issueCustomFields[0].Id);
        Assert.Equal("Duplicate",issueCustomFields[0].Values[0].Info);
        Assert.False(issueCustomFields[0].Multiple);
        Assert.Equal("Resolution",issueCustomFields[0].Name);
        
        Assert.NotNull(issues[1].CustomFields);
        issueCustomFields = issues[1].CustomFields.ToList();
        Assert.Equal(2, issueCustomFields.Count);
        
        Assert.Equal(1,issueCustomFields[0].Id);
        Assert.Equal("1.0.1",issueCustomFields[0].Values[0].Info);
        Assert.False(issueCustomFields[0].Multiple);
        Assert.Equal("Affected version",issueCustomFields[0].Name);
    }
    
    [Fact]
    public void Should_Deserialize_Issue_With_Journals()
    {
        const string input = """ 
        <issue>
            <id>1</id>
            <project name="Redmine" id="1"/>
            <tracker name="Defect" id="1"/>
            <journals type="array">
                <journal id="1">
                    <user name="Jean-Philippe Lang" id="1"/>
                    <notes>Fixed in Revision 128</notes>
                    <created_on>2007-01-01T05:21:00+01:00</created_on>
                    <details type="array"/>
                </journal>
                <journal id="10531">
                    <user name="efgh efgh" id="7384"/>
                    <notes/>
                    <created_on>2009-08-13T11:33:17+02:00</created_on>
                    <details type="array">
                        <detail property="attr" name="status_id">
                            <old_value>5</old_value>
                            <new_value>8</new_value>
                        </detail>
                    </details>
                </journal>
            </journals>
        </issue>
        
        """;
        
        var output = fixture.Serializer.Deserialize<Redmine.Net.Api.Types.Issue>(input);
        
        Assert.NotNull(output);
        Assert.Equal(1, output.Id);
        Assert.Equal("Redmine", output.Project.Name);
        Assert.Equal(1, output.Project.Id);
        Assert.Equal("Defect", output.Tracker.Name);
        Assert.Equal(1, output.Tracker.Id);

        var journals = output.Journals.ToList();
        Assert.Equal(2, journals.Count);

        Assert.Equal(1, journals[0].Id);
        Assert.Equal("Jean-Philippe Lang", journals[0].User.Name);
        Assert.Equal(1, journals[0].User.Id);
        Assert.Equal("Fixed in Revision 128", journals[0].Notes);
        Assert.Equal(new DateTime(2007, 1, 1, 4, 21, 0, DateTimeKind.Utc).ToLocalTime(), journals[0].CreatedOn);
        Assert.Null(journals[0].Details);

        Assert.Equal(10531, journals[1].Id);
        Assert.Equal("efgh efgh", journals[1].User.Name);
        Assert.Equal(7384, journals[1].User.Id);
        Assert.Null(journals[1].Notes);
        Assert.Equal(new DateTime(2009, 8, 13, 9, 33, 17, DateTimeKind.Utc).ToLocalTime(), journals[1].CreatedOn);

        var details = journals[1].Details.ToList();
        Assert.Single(details);
        Assert.Equal("attr", details[0].Property);
        Assert.Equal("status_id", details[0].Name);
        Assert.Equal("5", details[0].OldValue);
        Assert.Equal("8", details[0].NewValue);
        
    }

    [Fact]
    public void Should_Deserialize_Issue_With_Watchers()
    {
        const string input = """
        <?xml version="1.0" encoding="UTF-8"?>
        <issue>
            <id>5</id>
            <project id="1" name="Project-Test"/>
            <tracker id="1" name="Bug"/>
            <status id="1" name="New" is_closed="true"/>
            <priority id="2" name="Normal"/>
            <author id="90" name="Admin User"/>
            <fixed_version id="2" name="version2"/>
            <subject>#380</subject>
            <description></description>
            <start_date>2025-04-28</start_date>
            <due_date/>
            <done_ratio>0</done_ratio>
            <is_private>false</is_private>
            <estimated_hours/>
            <total_estimated_hours/>
            <spent_hours>0.0</spent_hours>
            <total_spent_hours>0.0</total_spent_hours>
            <created_on>2025-04-28T17:58:42Z</created_on>
            <updated_on>2025-04-28T17:58:42Z</updated_on>
            <closed_on/>
            <watchers type="array">
                <user id="91" name="Normal User"/>
                <user id="90" name="Admin User"/>
            </watchers>
        </issue>
        """;
        
        var output = fixture.Serializer.Deserialize<Redmine.Net.Api.Types.Issue>(input);
        
        Assert.NotNull(output);
        Assert.Equal(5, output.Id);
        Assert.Equal("Project-Test", output.Project.Name);
        Assert.Equal(1, output.Project.Id);
        Assert.Equal("Bug", output.Tracker.Name);
        Assert.Equal(1, output.Tracker.Id);
        Assert.Equal("New", output.Status.Name);
        Assert.Equal(1, output.Status.Id);
        Assert.True(output.Status.IsClosed);
        Assert.False(output.Status.IsDefault);
        Assert.Equal(2, output.FixedVersion.Id);
        Assert.Equal("version2", output.FixedVersion.Name);
        Assert.Equal(new DateTime(2025, 4, 28), output.StartDate);
        Assert.Null(output.DueDate);
        Assert.Equal(0, output.DoneRatio);
        Assert.Null(output.EstimatedHours);
        Assert.Null(output.TotalEstimatedHours);

        var watchers = output.Watchers.ToList();
        Assert.Equal(2, watchers.Count);
        
        Assert.Equal(91, watchers[0].Id);
        Assert.Equal("Normal User", watchers[0].Name);
        Assert.Equal(90, watchers[1].Id);
        Assert.Equal("Admin User", watchers[1].Name);
    }
}

