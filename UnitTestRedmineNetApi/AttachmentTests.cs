using Microsoft.VisualStudio.TestTools.UnitTesting;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestRedmineNetApi
{
    [TestClass]
    public class AttachmentTests
    {
        private RedmineManager redmineManager;
        private string uri;
        private string apiKey;

        [TestInitialize]
        public void Initialize()
        {
            uri = ConfigurationManager.AppSettings["uri"];
            apiKey = ConfigurationManager.AppSettings["apiKey"];

            SetMimeTypeJSON();
            SetMimeTypeXML();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(uri, apiKey, MimeFormat.xml);
        }

        [TestMethod]
        public void RedmineAttachments_ShouldGetById()
        {
            var attachment = redmineManager.GetObject<Attachment>("10", null);

            Assert.IsNotNull(attachment);
        }

        [TestMethod]
        public void RedmineAttachments_ShouldUploadAttachment()
        {
            //read document from specified path
            string documentPath = "E:\\uploadAttachment.txt";
            byte[] documentData = File.ReadAllBytes(documentPath);

            //upload attachment to redmine
            Upload attachment = redmineManager.UploadFile(documentData);

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
            Upload attachment1 = redmineManager.UploadFile(documentData);

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
            Issue issueWithAttachment = redmineManager.CreateObject<Issue>(issue);

            issue = redmineManager.GetObject<Issue>(issueWithAttachment.Id.ToString(), new NameValueCollection { { "include", "attachments" } });

            Assert.IsTrue(issue.Attachments.Count == 2 && issue.Attachments[0].FileName == attachment.FileName);
        }
    }
}
