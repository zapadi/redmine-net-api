using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;
using Redmine.Net.Api.Async;
using System.IO;
using System.Collections.Specialized;

namespace UnitTest_redmine_net45_api
{
    [TestClass]
    public class AttachmentTests
    {
        private RedmineManager redmineManager;

        [TestInitialize]
        public void Initialize()
        {
            SetMimeTypeXML();
            SetMimeTypeJSON();
            redmineManager.UseTraceLog();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey);
        }

        [TestMethod]
        public async Task Should_Get_Attachment_By_Id()
        {
            string attachmentId = "10";

            var attachment = await redmineManager.GetObjectAsync<Attachment>(attachmentId, null);

            Assert.IsNotNull(attachment, "Get attachment by id returned null.");
            Assert.IsInstanceOfType(attachment, typeof(Attachment));
        }

        [TestMethod]
        public async Task Should_Upload_Attachment()
        {
            //read document from specified path
            string documentPath = "E:\\uploadAttachment.txt";
            byte[] documentData = File.ReadAllBytes(documentPath);

            //upload attachment to redmine
            Upload attachment = await redmineManager.UploadFileAsync(documentData);

            //set attachment properties
            attachment.FileName = "AttachmentUploaded.txt";
            attachment.Description = "File uploaded using REST API";
            attachment.ContentType = "text/plain";

            //create list of attachments to be added to issue
            IList<Upload> attachments = new List<Upload>();
            attachments.Add(attachment);

            //read document from specified path
            documentPath = "E:\\uploadAttachment1.txt";
            documentData = File.ReadAllBytes(documentPath);

            //upload attachment to redmine
            Upload attachment1 = await redmineManager.UploadFileAsync(documentData);

            //set attachment properties
            attachment1.FileName = "AttachmentUploaded1.txt";
            attachment1.Description = "Second file uploaded";
            attachment1.ContentType = "text/plain";
            attachments.Add(attachment1);

            Issue issue = new Issue();
            issue.Project = new Project { Id = 10 };
            issue.Tracker = new IdentifiableName { Id = 4 };
            issue.Status = new IdentifiableName { Id = 5 };
            issue.Priority = new IdentifiableName { Id = 8 };
            issue.Subject = "Issue with attachments";
            issue.Description = "Issue description...";
            issue.Category = new IdentifiableName { Id = 11 };
            issue.FixedVersion = new IdentifiableName { Id = 9 };
            issue.AssignedTo = new IdentifiableName { Id = 8 };
            issue.ParentIssue = new IdentifiableName { Id = 19 };
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
            Issue issueWithAttachment = await redmineManager.CreateObjectAsync<Issue>(issue);

            issue = await redmineManager.GetObjectAsync<Issue>(issueWithAttachment.Id.ToString(), new NameValueCollection { { "include", "attachments" } });

            Assert.IsNotNull(issue, "Get issue returned null.");
            Assert.IsInstanceOfType(issue, typeof(Issue));

            Assert.IsTrue(issue.Attachments.Count == 2, "Attachments count != 2");
            CollectionAssert.AllItemsAreNotNull(issue.Attachments.ToList(), "Attachments contains null items.");
            CollectionAssert.AllItemsAreUnique(issue.Attachments.ToList(), "Attachments items are not unique.");
            Assert.IsTrue(issue.Attachments[0].FileName == attachment.FileName);
            Assert.IsTrue(issue.Attachments[1].FileName == attachment1.FileName);
        }
    }
}
