using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xunit;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace xUnitTestredminenet45api
{
	[Collection("RedmineCollection")]
	public class AttachmentTests
	{
		const string ATTACHMENT_LOCAL_PATH = "uploadAttachment.pages";
		const string ATTACHMENT_NAME = "AttachmentUploaded.txt";
		const string ATTACHMENT_DESCRIPTION = "File uploaded using REST API";
		const string ATTACHMENT_CONTENT_TYPE = "text/plain";
		const int PROJECT_ID = 9;
		const string ISSUE_SUBJECT = "Issue with attachments";

		const string ATTACHMENT_ID = "48";
		const string ATTACHMENT_FILE_NAME = "uploadAttachment.pages";

		RedmineFixture fixture;
		public AttachmentTests (RedmineFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void Should_Upload_Attachment()
		{
			//read document from specified path
			byte[] documentData = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory+ATTACHMENT_LOCAL_PATH);

			//upload attachment to redmine
			Upload attachment = fixture.RedmineManager.UploadFile(documentData);

			//set attachment properties
			attachment.FileName = ATTACHMENT_NAME;
			attachment.Description = ATTACHMENT_DESCRIPTION;
			attachment.ContentType = ATTACHMENT_CONTENT_TYPE;

			//create list of attachments to be added to issue
			IList<Upload> attachments = new List<Upload>();
			attachments.Add(attachment);

			Issue issue = new Issue();
			issue.Project = new Project { Id = PROJECT_ID };
			issue.Subject = ISSUE_SUBJECT;
			issue.Uploads = attachments;

			//create issue and attach document
			Issue issueWithAttachment = fixture.RedmineManager.CreateObject<Issue>(issue);

			issue = fixture.RedmineManager.GetObject<Issue>(issueWithAttachment.Id.ToString(), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.ATTACHMENTS } });

			Assert.NotNull(issue);
			Assert.NotNull(issue.Attachments);
			Assert.All (issue.Attachments, a => Assert.IsType<Attachment> (a));
			Assert.True(issue.Attachments.Count == 1, "Number of attachments != 1");
			Assert.True(issue.Attachments[0].FileName == ATTACHMENT_NAME, "Attachment name is not correct.");
			Assert.True(issue.Attachments[0].Description == ATTACHMENT_DESCRIPTION, "Attachment description is not correct.");
			Assert.True(issue.Attachments[0].ContentType == ATTACHMENT_CONTENT_TYPE, "Attachment content type is not correct.");
		}

		[Fact]
		public void Should_Get_Attachment_By_Id()
		{
			var attachment = fixture.RedmineManager.GetObject<Attachment>(ATTACHMENT_ID, null);

			Assert.NotNull(attachment);
			Assert.IsType<Attachment> (attachment);
			Assert.True(attachment.FileName == ATTACHMENT_FILE_NAME, "Attachment file name is not the expected one.");
		}

		[Fact]
		public void Sould_Download_Attachment()
		{
			var url = Helper.Uri + "/attachments/download/" + ATTACHMENT_ID + "/" + ATTACHMENT_FILE_NAME;

			var document = fixture.RedmineManager.DownloadFile(url);

			Assert.NotNull(document);
		}
	}
}

