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

namespace UnitTest_redmine_net40_api
{
    [TestClass]
    public class AttachmentTests
    {
        private RedmineManager redmineManager;

        const string ATTACHMENT_LOCAL_PATH = "E:\\uploadAttachment.txt";
        const string ATTACHMENT_NAME = "AttachmentUploaded.txt";
        const string ATTACHMENT_DESCRIPTION = "File uploaded using REST API";
        const string ATTACHMENT_CONTENT_TYPE = "text/plain";
        const int PROJECT_ID = 1;
        const string ISSUE_SUBJECT = "Issue with attachments";

        const string ATTACHMENT_ID = "1";
        const string ATTACHMENT_FILE_NAME = "AttachmentUploaded.txt";
        
        [TestInitialize]
        public void Initialize()
        {
            SetMimeTypeJSON();
            SetMimeTypeXML();
        }

        [Conditional("JSON")]
        private void SetMimeTypeJSON()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.json);
        }

        [Conditional("XML")]
        private void SetMimeTypeXML()
        {
            redmineManager = new RedmineManager(Helper.Uri, Helper.ApiKey, MimeFormat.xml);
        }

        [TestMethod]
        public void Should_Upload_Attachment()
        {
            //read document from specified path
            byte[] documentData = File.ReadAllBytes(ATTACHMENT_LOCAL_PATH);

            //upload attachment to redmine
            Upload attachment = redmineManager.UploadFile(documentData);

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
            Issue issueWithAttachment = redmineManager.CreateObject<Issue>(issue);

            issue = redmineManager.GetObject<Issue>(issueWithAttachment.Id.ToString(), new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.ATTACHMENTS } });

            Assert.IsNotNull(issue, "Get created issue returned null.");
            Assert.IsNotNull(issue.Attachments, "Attachments list is null.");
            CollectionAssert.AllItemsAreNotNull(issue.Attachments.ToList(), "Attachments list contains null items.");
            CollectionAssert.AllItemsAreInstancesOfType(issue.Attachments.ToList(), typeof(Attachment), "Attachments contains items of unexpected type.");
            Assert.IsTrue(issue.Attachments.Count == 1, "Number of attachments != 1");
            Assert.IsTrue(issue.Attachments[0].FileName == ATTACHMENT_NAME, "Attachment name is not correct.");
            Assert.IsTrue(issue.Attachments[0].Description == ATTACHMENT_DESCRIPTION, "Attachment description is not correct.");
            Assert.IsTrue(issue.Attachments[0].ContentType == ATTACHMENT_CONTENT_TYPE, "Attachment content type is not correct.");
        }

        [TestMethod]
        public void Should_Get_Attachment_By_Id()
        {
            var attachment = redmineManager.GetObject<Attachment>(ATTACHMENT_ID, null);

            Assert.IsNotNull(attachment, "Get attachment returned null.");
            Assert.IsInstanceOfType(attachment, typeof(Attachment), "Downloaded object is not of type attachment.");
            Assert.IsTrue(attachment.FileName == ATTACHMENT_FILE_NAME, "Attachment file name is not the expected one.");
        }

        [TestMethod]
        public void Sould_Download_Attachment()
        {
            var url = Helper.Uri + "/attachments/download/" + ATTACHMENT_ID + "/" + ATTACHMENT_FILE_NAME;
            
            var document = redmineManager.DownloadFile(url);

            Assert.IsNotNull(document, "Downloaded file is null.");
        }
    }
}
