#if !(NET20 || NET40)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Redmine.Net.Api.Async;
using Redmine.Net.Api.Types;
using Xunit;

namespace Padi.RedmineApi.Tests.Tests.Async
{
    [Collection("RedmineCollection")]
    public class AttachmentAsyncTests
    {
        private const string ATTACHMENT_ID = "10";

        private readonly RedmineFixture fixture;
        public AttachmentAsyncTests(RedmineFixture fixture)
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
            var documentPath = AppDomain.CurrentDomain.BaseDirectory + "/uploadAttachment.pages";
            var documentData = System.IO.File.ReadAllBytes(documentPath);

            //upload attachment to redmine
            var attachment = await fixture.RedmineManager.UploadFileAsync(documentData);

            //set attachment properties
            attachment.FileName = "uploadAttachment.pages";
            attachment.Description = "File uploaded using REST API";
            attachment.ContentType = "text/plain";

            //create list of attachments to be added to issue
            IList<Upload> attachments = new List<Upload>();
            attachments.Add(attachment);


            var icf = (IssueCustomField)IdentifiableName.Create(13);
            icf.Values = new List<CustomFieldValue> { new CustomFieldValue { Info = "Issue custom field completed" } };

            var issue = new Issue
            {
                Project = IdentifiableName.Create(9),
                Tracker = IdentifiableName.Create(3),
                Status = IdentifiableName.Create(6),
                Priority = IdentifiableName.Create(9),
                Subject = "Issue with attachments",
                Description = "Issue description...",
                Category = IdentifiableName.Create(18),
                FixedVersion = IdentifiableName.Create(9),
                AssignedTo = IdentifiableName.Create(8),
                ParentIssue = IdentifiableName.Create(96),
                CustomFields = new List<IssueCustomField> {icf},
                IsPrivate = true,
                EstimatedHours = 12,
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddMonths(1),
                Uploads = attachments,
                Watchers = new List<Watcher>
                {
                    (Watcher) IdentifiableName.Create(8),
                    (Watcher) IdentifiableName.Create(2)
                }
            };

            //create issue and attach document
            var issueWithAttachment = await fixture.RedmineManager.CreateObjectAsync(issue);

            issue = await fixture.RedmineManager.GetObjectAsync<Issue>(issueWithAttachment.Id.ToString(),
                new NameValueCollection { { "include", "attachments" } });

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
#endif