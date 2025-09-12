using System.Collections.Generic;
using System.Linq;
using Padi.DotNet.RedmineAPI.Tests.Infrastructure;
using Xunit;

namespace Padi.DotNet.RedmineAPI.Tests.Serialization;

[Collection(Constants.DeserializeCollection)]
public class IssueTests()
{
    [Theory]
    [MemberData(nameof(IssuesDeserializeTheoryData))]
    public void Should_Deserialize_Issues(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
        
        var output = serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.Issue>(input);
        
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
        Assert.Equal("Aggregate Multiple Issue Changes for Email Notifications", issues[0].Subject);
        Assert.Contains("This is not to be confused with another useful proposed feature", issues[0].Description);
        AssertDateTime.Equal("2009-12-3", issues[0].StartDate, dateOnly: true);
        Assert.Null(issues[0].DueDate);
        Assert.Equal(0, issues[0].DoneRatio);
        Assert.Null(issues[0].EstimatedHours);
        // AssertDateTime.Equal("2009-12-3T14:02:12", issues[0].CreatedOn);
        // AssertDateTime.Equal("2010-1-3T11:08:41", issues[0].UpdatedOn);

        Assert.Equal(4325, issues[1].Id);
        Assert.Null(issues[1].Journals);
        Assert.Null(issues[1].ChangeSets);
        Assert.Null(issues[1].CustomFields);
    }
    
    [Theory]
    [MemberData(nameof(IssueWithCustomFieldsDeserializeTheoryData))]
    public void Should_Deserialize_Issues_With_CustomFields(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
       
        var output = serializer.DeserializeToPagedResults<Redmine.Net.Api.Types.Issue>(input);
        
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
        AssertDateTime.Equal("2009-12-3", issues[0].StartDate, dateOnly: true);
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
    
    [Theory]
    [MemberData(nameof(IssueWithJournalsDeserializeTheoryData))]
    public void Should_Deserialize_Issue_With_Journals(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
       
        var output = serializer.Deserialize<Redmine.Net.Api.Types.Issue>(input);
        
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
        AssertDateTime.Equal("2007-1-1T04:21:00", journals[0].CreatedOn);
        Assert.Null(journals[0].Details);

        Assert.Equal(10531, journals[1].Id);
        Assert.Equal("efgh efgh", journals[1].User.Name);
        Assert.Equal(7384, journals[1].User.Id);
        Assert.Null(journals[1].Notes);
        AssertDateTime.Equal("2009-8-13T09:33:17", journals[1].CreatedOn);

        var details = journals[1].Details.ToList();
        Assert.Single(details);
        Assert.Equal("attr", details[0].Property);
        Assert.Equal("status_id", details[0].Name);
        Assert.Equal("5", details[0].OldValue);
        Assert.Equal("8", details[0].NewValue);
        
    }

    [Theory]
    [MemberData(nameof(IssueWithWatchersDeserializeTheoryData))]
    public void Should_Deserialize_Issue_With_Watchers(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
      
        var output = serializer.Deserialize<Redmine.Net.Api.Types.Issue>(input);
        
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
        AssertDateTime.Equal("2025-4-28", output.StartDate);
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
    
    [Theory]
    [MemberData(nameof(IssueWithAttachmentsAndCustomFieldsDeserializeTheoryData))]
    public void Should_Deserialize_Issue_With_Attachments_And_CustomFields(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
      
        var output = serializer.Deserialize<Redmine.Net.Api.Types.Issue>(input);
        
        Assert.NotNull(output);
        Assert.Equal(370, output.Id);
        Assert.Equal("Redmine-NET-API", output.Project.Name);
        Assert.Equal(1, output.Project.Id);
        Assert.Equal("Bug", output.Tracker.Name);
        Assert.Equal(1, output.Tracker.Id);
        Assert.Equal("New", output.Status.Name);
        Assert.Equal(1, output.Status.Id);
        Assert.Equal("Normal", output.Priority.Name);
        Assert.Equal(2, output.Priority.Id);
        // Assert.True(output.Status.IsClosed);
        // Assert.False(output.Status.IsDefault);
         Assert.Equal(21, output.FixedVersion.Id);
        // Assert.Equal("version2", output.FixedVersion.Name);
        // AssertDateTime.Equal("2025-4-28", output.StartDate);
        // Assert.Null(output.DueDate);
        // Assert.Equal(0, output.DoneRatio);
        // Assert.Null(output.EstimatedHours);
        // Assert.Null(output.TotalEstimatedHours);

         var customFields = output.CustomFields.ToList();
         Assert.Single(customFields);
         
         var attachments = output.Attachments.ToList();
         Assert.Single(attachments);
         Assert.Equal(289, attachments[0].Id);
    }
    
    [Theory]
    [MemberData(nameof(IssueWithHierarchicalChildrenDeserializeTheoryData))]
    public void Should_Deserialize_Issue_With_Hierarchical_Children(string input, SerializerKind kind)
    {
        var serializer = SerializerFactory.Create(kind);
      
        var output = serializer.Deserialize<Redmine.Net.Api.Types.Issue>(input);
        
        Assert.NotNull(output);
        Assert.Equal(21512, output.Id);
         
        var list = output.Children.ToList();
        Assert.Single(list);

        var children = list[0].Children;
        Assert.Equal(8, children.Count);
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> IssuesDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "issues": {
                                    "total_count": 1640,
                                    "items": [
                                      {
                                        "id": 4326,
                                        "project": { "id": 1, "name": "Redmine" },
                                        "tracker": { "id": 2, "name": "Feature" },
                                        "status": { "id": 1, "name": "New" },
                                        "priority": { "id": 4, "name": "Normal" },
                                        "author": { "id": 10106, "name": "John Smith" },
                                        "category": { "id": 9, "name": "Email notifications" },
                                        "subject": "Aggregate Multiple Issue Changes for Email Notifications",
                                        "description": "This is not to be confused with another useful proposed feature that\n                                       would do digest emails for notifications.",
                                        "start_date": "2009-12-03",
                                        "due_date": null,
                                        "done_ratio": 0,
                                        "estimated_hours": null,
                                        "created_on": "2009-12-03T15:02:12+01:00",
                                        "updated_on": "2010-01-03T12:08:41+01:00"
                                      },
                                      {
                                        "id": 4325,
                                        "journals": [],
                                        "changesets": [],
                                        "custom_fields": []
                                      }
                                    ]
                                  }
                                }
                                """;

            const string xml = """
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

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText)
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    } 
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> IssueWithCustomFieldsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "issues": {
                                    "count": 1640,
                                    "items": [
                                      {
                                        "id": 4326,
                                        "project": { "id": 1, "name": "Redmine" },
                                        "tracker": { "id": 2, "name": "Feature" },
                                        "status": { "id": 1, "name": "New" },
                                        "priority": { "id": 4, "name": "Normal" },
                                        "author": { "id": 10106, "name": "John Smith" },
                                        "category": { "id": 9, "name": "Email notifications" },
                                        "subject": "Aggregate Multiple Issue Changes for Email Notifications",
                                        "description": "This is not to be confused with another useful proposed feature that\n                                       would do digest emails for notifications.",
                                        "start_date": "2009-12-03",
                                        "due_date": null,
                                        "done_ratio": 0,
                                        "estimated_hours": null,
                                        "custom_fields": [
                                          { "id": 2, "name": "Resolution", "value": "Duplicate" },
                                          { "id": 5, "name": "Texte", "value": "Test" },
                                          { "id": 6, "name": "Boolean", "value": "1" },
                                          { "id": 7, "name": "Date", "value": "2010-01-12" }
                                        ],
                                        "created_on": "2009-12-03T15:02:12+01:00",
                                        "updated_on": "2010-01-03T12:08:41+01:00"
                                      },
                                      {
                                        "id": 4325,
                                        "journals": [],
                                        "changesets": [],
                                        "custom_fields": [
                                          { "id": 1, "name": "Affected version", "value": "1.0.1" },
                                          { "id": 2, "name": "Resolution", "value": "Fixed" }
                                        ]
                                      }
                                    ]
                                  }
                                }
                                """;

            const string xml = """
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
                                       <created_on>2009-12-03T15:02:12+01:00</created_on>
                                       <updated_on>2010-01-03T12:08:41+01:00</updated_on>
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

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText)
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    } 
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> IssueWithJournalsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "issue": {
                                    "id": 1,
                                    "project": { "id": 1, "name": "Redmine" },
                                    "tracker": { "id": 1, "name": "Defect" },
                                    "journals": [
                                      {
                                        "id": 1,
                                        "user": { "id": 1, "name": "Jean-Philippe Lang" },
                                        "notes": "Fixed in Revision 128",
                                        "created_on": "2007-01-01T05:21:00+01:00",
                                        "details": []
                                      },
                                      {
                                        "id": 10531,
                                        "user": { "id": 7384, "name": "efgh efgh" },
                                        "notes": null,
                                        "created_on": "2009-08-13T11:33:17+02:00",
                                        "details": [
                                          {
                                            "property": "attr",
                                            "name": "status_id",
                                            "old_value": "5",
                                            "new_value": "8"
                                          }
                                        ]
                                      }
                                    ]
                                  }
                                }
                                """;

            const string xml = """
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

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText)
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    } 
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> IssueWithWatchersDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "issue": {
                                    "id": 5,
                                    "project": { "id": 1, "name": "Project-Test" },
                                    "tracker": { "id": 1, "name": "Bug" },
                                    "status": { "id": 1, "name": "New", "is_closed": true },
                                    "priority": { "id": 2, "name": "Normal" },
                                    "author": { "id": 90, "name": "Admin User" },
                                    "fixed_version": { "id": 2, "name": "version2" },
                                    "subject": "#380",
                                    "description": "",
                                    "start_date": "2025-04-28",
                                    "due_date": null,
                                    "done_ratio": 0,
                                    "is_private": false,
                                    "estimated_hours": null,
                                    "total_estimated_hours": null,
                                    "spent_hours": 0.0,
                                    "total_spent_hours": 0.0,
                                    "created_on": "2025-04-28T17:58:42Z",
                                    "updated_on": "2025-04-28T17:58:42Z",
                                    "closed_on": null,
                                    "watchers": [
                                      { "id": 91, "name": "Normal User" },
                                      { "id": 90, "name": "Admin User" }
                                    ]
                                  }
                                }
                                """;

            const string xml = """
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

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText)
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> IssueWithAttachmentsAndCustomFieldsDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "issue": {
                                    "id": 370,
                                    "project": { "id": 1, "name": "Redmine-NET-API" },
                                    "tracker": { "id": 1, "name": "Bug" },
                                    "status": { "id": 1, "is_closed": false, "name": "New" },
                                    "priority": { "id": 2, "name": "Normal" },
                                    "author": { "id": 1, "name": "Redmine Admin" },
                                    "fixed_version": { "id": 21, "name": "Test Version Create KwQThN" },
                                    "subject": "FtwGTSKCl",
                                    "description": "ZegAYgPGVyPeJdQbot",
                                    "start_date": null,
                                    "due_date": null,
                                    "done_ratio": 0,
                                    "is_private": false,
                                    "estimated_hours": null,
                                    "total_estimated_hours": null,
                                    "spent_hours": 0.0,
                                    "total_spent_hours": 0.0,
                                    "custom_fields": [
                                      {
                                        "id": 1,
                                        "name": "IssueCustonField1",
                                        "value": "3"
                                      }
                                    ],
                                    "created_on": "2025-09-12T09:29:52Z",
                                    "updated_on": "2025-09-12T09:29:52Z",
                                    "closed_on": null,
                                    "attachments": [
                                      {
                                        "id": 289,
                                        "filename": "test-file_qzYORpS",
                                        "filesize": 512000,
                                        "content_type": null,
                                        "description": null,
                                        "content_url": "http://localhost:8089/attachments/download/289/test-file_qzYORpS",
                                        "author": { "id": 1, "name": "Redmine Admin" },
                                        "created_on": "2025-09-12T09:29:46Z"
                                      }
                                    ]
                                  }
                                }
                                """;

            const string xml = """
                               <?xml version="1.0" encoding="UTF-8" standalone="no"?>
                               <issue>
                                   <id>370</id>
                                   <project id="1" name="Redmine-NET-API"/>
                                   <tracker id="1" name="Bug"/>
                                   <status id="1" is_closed="false" name="New"/>
                                   <priority id="2" name="Normal"/>
                                   <author id="1" name="Redmine Admin"/>
                                   <fixed_version id="21" name="Test Version Create KwQThN"/>
                                   <subject>FtwGTSKCl</subject>
                                   <description>ZegAYgPGVyPeJdQbot</description>
                                   <start_date/>
                                   <due_date/>
                                   <done_ratio>0</done_ratio>
                                   <is_private>false</is_private>
                                   <estimated_hours/>
                                   <total_estimated_hours/>
                                   <spent_hours>0.0</spent_hours>
                                   <total_spent_hours>0.0</total_spent_hours>
                                   <custom_fields type="array">
                                       <custom_field id="1" name="IssueCustonField1">
                                           <value>3</value>
                                       </custom_field>
                                   </custom_fields>
                                   <created_on>2025-09-12T09:29:52Z</created_on>
                                   <updated_on>2025-09-12T09:29:52Z</updated_on>
                                   <closed_on/>
                                   <attachments type="array">
                                       <attachment>
                                           <id>289</id>
                                           <filename>test-file_qzYORpS</filename>
                                           <filesize>512000</filesize>
                                           <content_type/>
                                           <description/>
                                           <content_url>http://localhost:8089/attachments/download/289/test-file_qzYORpS</content_url>
                                           <author id="1" name="Redmine Admin"/>
                                           <created_on>2025-09-12T09:29:46Z</created_on>
                                       </attachment>
                                   </attachments>
                               </issue>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText)
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
    
    public static IEnumerable<TheoryDataRow<string, SerializerKind>> IssueWithHierarchicalChildrenDeserializeTheoryData
    {
        get
        {
            const string json = """
                                {
                                  "issue": {
                                    "id": 21512,
                                    "project": { "id": 1498, "name": "project name" },
                                    "tracker": { "id": 9, "name": "group" },
                                    "status": { "id": 2, "name": "start", "is_closed": false },
                                    "priority": { "id": 2, "name": "normal" },
                                    "author": { "id": 25, "name": "foo" },
                                    "subject": "parent issue",
                                    "description": null,
                                    "start_date": "2025-04-17",
                                    "due_date": "2025-09-30",
                                    "done_ratio": 64,
                                    "is_private": false,
                                    "estimated_hours": null,
                                    "total_estimated_hours": 0.0,
                                    "spent_hours": 0.0,
                                    "total_spent_hours": 125.2,
                                    "created_on": "2025-02-03T08:35:55Z",
                                    "updated_on": "2025-08-18T23:48:17Z",
                                    "closed_on": null,
                                      "children": [
                                      {
                                        "id": 21558,
                                        "tracker": { "id": 5, "name": "func" },
                                        "subject": "issue-10 dev",
                                        "children": [
                                          {
                                            "id": 21641,
                                            "tracker": { "id": 9, "name": "group" },
                                            "subject": "issue-10 #plan survey",
                                            "children": [
                                              { "id": 21642, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #plan 1" },
                                              { "id": 23139, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #plan 2" }
                                            ]
                                          },
                                          {
                                            "id": 21644,
                                            "tracker": { "id": 9, "name": "group" },
                                            "subject": "issue-10 #UID",
                                            "children": [
                                              { "id": 23055, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #UI task-1" },
                                              { "id": 23319, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #UI task-2" }
                                            ]
                                          },
                                          {
                                            "id": 21645,
                                            "tracker": { "id": 9, "name": "group" },
                                            "subject": "issue-10 #SSD",
                                            "children": [
                                              { "id": 21646, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #SS task-1" },
                                              { "id": 23062, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #SS task-2-1" },
                                              { "id": 23063, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #SS task-2-2" }
                                            ]
                                          },
                                          {
                                            "id": 21648,
                                            "tracker": { "id": 9, "name": "group" },
                                            "subject": "issue-10 #PG",
                                            "children": [
                                              { "id": 21649, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #PG task-1-1" },
                                              { "id": 23442, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #PG task-1-2" },
                                              { "id": 23443, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #PG task-1-3" },
                                              { "id": 23571, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #PG task-2-1" },
                                              { "id": 23572, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #PG task-2-2" }
                                            ]
                                          },
                                          {
                                            "id": 21650,
                                            "tracker": { "id": 9, "name": "group" },
                                            "subject": "issue-10 #PT",
                                            "children": [
                                              { "id": 21651, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #PT task-1-1" },
                                              { "id": 23577, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #PT task-1-2" },
                                              { "id": 23578, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #PT task-1-3" },
                                              { "id": 23579, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #PT task-2-1" },
                                              { "id": 23580, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #PT task-2-2" }
                                            ]
                                          },
                                          {
                                            "id": 21652,
                                            "tracker": { "id": 9, "name": "group" },
                                            "subject": "issue-10 #IT",
                                            "children": [
                                              { "id": 21653, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #IT task-1" },
                                              { "id": 23585, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #IT task-2" }
                                            ]
                                          },
                                          {
                                            "id": 23056,
                                            "tracker": { "id": 9, "name": "group" },
                                            "subject": "issue-10 #ST",
                                            "children": [
                                              { "id": 23057, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #ST task" }
                                            ]
                                          },
                                          {
                                            "id": 23591,
                                            "tracker": { "id": 9, "name": "group" },
                                            "subject": "issue-10 #MAN",
                                            "children": [
                                              { "id": 23592, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #MAN A" },
                                              { "id": 23593, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #MAN B" },
                                              { "id": 23797, "tracker": { "id": 6, "name": "task" }, "subject": "issue-10 #MAN C" }
                                            ]
                                          }
                                        ]
                                      }
                                    ]
                                  }
                                }
                                """;

            const string xml = """
                               <issue>
                                   <id>21512</id>
                                   <project id="1498" name="project name" />
                                   <tracker id="9" name="group" />
                                   <status id="2" name="start" is_closed="false" />
                                   <priority id="2" name="normal" />
                                   <author id="25" name="foo" />
                                   <subject>parent issue</subject>
                                   <description />
                                   <start_date>2025-04-17</start_date>
                                   <due_date>2025-09-30</due_date>
                                   <done_ratio>64</done_ratio>
                                   <is_private>false</is_private>
                                   <estimated_hours />
                                   <total_estimated_hours>0.0</total_estimated_hours>
                                   <spent_hours>0.0</spent_hours>
                                   <total_spent_hours>125.2</total_spent_hours>
                                   <created_on>2025-02-03T08:35:55Z</created_on>
                                   <updated_on>2025-08-18T23:48:17Z</updated_on>
                                   <closed_on />
                                   <children type="array">
                                       <issue id="21558">
                                           <tracker id="5" name="func" />
                                           <subject>issue-10 dev</subject>
                                           <children type="array">
                                               <issue id="21641">
                                                   <tracker id="9" name="group" />
                                                   <subject>issue-10 #plan survey</subject>
                                                   <children type="array">
                                                       <issue id="21642">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #plan 1</subject>
                                                       </issue>
                                                       <issue id="23139">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #plan 2</subject>
                                                       </issue>
                                                   </children>
                                               </issue>
                                               <issue id="21644">
                                                   <tracker id="9" name="group" />
                                                   <subject>issue-10 #UID</subject>
                                                   <children type="array">
                                                       <issue id="23055">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #UI task-1</subject>
                                                       </issue>
                                                       <issue id="23319">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #UI task-2</subject>
                                                       </issue>
                                                   </children>
                                               </issue>
                                               <issue id="21645">
                                                   <tracker id="9" name="group" />
                                                   <subject>issue-10 #SSD</subject>
                                                   <children type="array">
                                                       <issue id="21646">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #SS task-1</subject>
                                                       </issue>
                                                       <issue id="23062">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #SS task-2-1</subject>
                                                       </issue>
                                                       <issue id="23063">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #SS task-2-2</subject>
                                                       </issue>
                                                   </children>
                                               </issue>
                                               <issue id="21648">
                                                   <tracker id="9" name="group" />
                                                   <subject>issue-10 #PG</subject>
                                                   <children type="array">
                                                       <issue id="21649">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #PG task-1-1</subject>
                                                       </issue>
                                                       <issue id="23442">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #PG task-1-2</subject>
                                                       </issue>
                                                       <issue id="23443">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #PG task-1-3</subject>
                                                       </issue>
                                                       <issue id="23571">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #PG task-2-1</subject>
                                                       </issue>
                                                       <issue id="23572">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #PG task-2-2</subject>
                                                       </issue>
                                                   </children>
                                               </issue>
                                               <issue id="21650">
                                                   <tracker id="9" name="group" />
                                                   <subject>issue-10 #PT</subject>
                                                   <children type="array">
                                                       <issue id="21651">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #PT task-1-1</subject>
                                                       </issue>
                                                       <issue id="23577">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #PT task-1-2</subject>
                                                       </issue>
                                                       <issue id="23578">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #PT task-1-3</subject>
                                                       </issue>
                                                       <issue id="23579">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #PT task-2-1</subject>
                                                       </issue>
                                                       <issue id="23580">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #PT task-2-2</subject>
                                                       </issue>
                                                    </children>
                                               </issue>
                                               <issue id="21652">
                                                   <tracker id="9" name="group" />
                                                   <subject>issue-10 #IT</subject>
                                                   <children type="array">
                                                       <issue id="21653">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #IT task-1</subject>
                                                       </issue>
                                                       <issue id="23585">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #IT task-2</subject>
                                                       </issue>
                                                   </children>
                                               </issue>
                                               <issue id="23056">
                                                   <tracker id="9" name="group" />
                                                   <subject>issue-10 #ST</subject>
                                                   <children type="array">
                                                       <issue id="23057">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #ST task</subject>
                                                       </issue>
                                                   </children>
                                               </issue>
                                               <issue id="23591">
                                                   <tracker id="9" name="group" />
                                                   <subject>issue-10 #MAN</subject>
                                                   <children type="array">
                                                       <issue id="23592">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #MAN A</subject>
                                                       </issue>
                                                       <issue id="23593">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #MAN B</subject>
                                                       </issue>
                                                       <issue id="23797">
                                                           <tracker id="6" name="task" />
                                                           <subject>issue-10 #MAN C</subject>
                                                       </issue>
                                                   </children>
                                               </issue>
                                           </children>
                                       </issue>
                                   </children>
                               </issue>
                               """;

            yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.NewtonsoftJson).WithTestDisplayName(Constants.JsonNewtonsoft);
            // yield return new TheoryDataRow<string, SerializerKind>(json, SerializerKind.SystemTextJson).WithTestDisplayName(Constants.JsonSystemText)
            yield return new TheoryDataRow<string, SerializerKind>(xml, SerializerKind.Xml).WithTestDisplayName(Constants.Xml);
        }
    }
}

