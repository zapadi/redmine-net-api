using System;
using System.IO;
using System.Collections.Specialized;
using System.Collections.Generic;
using Xunit;
using Redmine.Net.Api.Types;
using Redmine.Net.Api.Async;
using System.Threading.Tasks;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class AttachmentAsyncTests
	{
		private const string ATTACHMENT_ID = "10";

	    private readonly RedmineFixture fixture;
		public AttachmentAsyncTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public async Task Should_Get_Attachment_By_Id()
		{
			var attachment = await fixture.RedmineManager.GetObjectAsync<Attachment>(ATTACHMENT_ID, null);

			Assert.NotNull(attachment);
			Assert.IsType<Attachment>(attachment);
		}

		[Fact]
		public async Task Should_Upload_Attachment()
		{
			//read document from specified path
			string documentPath = AppDomain.CurrentDomain.BaseDirectory+ "/uploadAttachment.pages";
			byte[] documentData = File.ReadAllBytes(documentPath);

			//upload attachment to redmine
			Upload attachment = await fixture.RedmineManager.UploadFileAsync(documentData);

			//set attachment properties
			attachment.FileName = "uploadAttachment.pages";
			attachment.Description = "File uploaded using REST API";
			attachment.ContentType = "text/plain";

			//create list of attachments to be added to issue
			IList<Upload> attachments = new List<Upload>();
			attachments.Add(attachment);

			Issue issue = new Issue();
			issue.Project = new Project { Id = 9 };
			issue.Tracker = new IdentifiableName { Id = 3 };
			issue.Status = new IdentifiableName { Id = 6 };
			issue.Priority = new IdentifiableName { Id = 9 };
			issue.Subject = "Issue with attachments";
			issue.Description = "Issue description...";
			issue.Category = new IdentifiableName { Id = 18 };
			issue.FixedVersion = new IdentifiableName { Id = 9 };
			issue.AssignedTo = new IdentifiableName { Id = 8 };
			issue.ParentIssue = new IdentifiableName { Id = 96 };
			issue.CustomFields = new List<IssueCustomField>();
			issue.CustomFields.Add(new IssueCustomField { Id = 13, Values = new List<CustomFieldValue> { new CustomFieldValue { Info = "Issue custom field completed" } } });
			issue.IsPrivate = true;
			issue.EstimatedHours = 12;
			issue.StartDate = DateTime.Now;
			issue.DueDate = DateTime.Now.AddMonths(1);
			issue.Uploads = attachments;
			issue.Watchers = new List<Watcher>();
			issue.Watchers.Add(new Watcher { Id = 8 });
			issue.Watchers.Add(new Watcher { Id = 2 });

			//create issue and attach document
			Issue issueWithAttachment = await fixture.RedmineManager.CreateObjectAsync(issue);

			issue = await fixture.RedmineManager.GetObjectAsync<Issue>(issueWithAttachment.Id.ToString(), new NameValueCollection { { "include", "attachments" } });

			Assert.NotNull(issue);
			Assert.IsType<Issue>(issue);

			Assert.True(issue.Attachments.Count == 1, "Attachments count != 1");
			Assert.True(issue.Attachments[0].FileName == attachment.FileName);
		}

		[Fact]
		public async Task Sould_Download_Attachment()
		{
			var attachment = await fixture.RedmineManager.GetObjectAsync<Attachment>(ATTACHMENT_ID, null);

			var document = await fixture.RedmineManager.DownloadFileAsync(attachment.ContentUrl);

			Assert.NotNull(document);
		}
	}
}