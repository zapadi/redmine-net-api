/*
   Copyright 2011 - 2016 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using Xunit;

namespace xUnitTestredminenet45api
{
    [Collection("RedmineCollection")]
    public class AttachmentTests
    {
        public AttachmentTests(RedmineFixture fixture)
        {
            this.fixture = fixture;
        }

        private const string ATTACHMENT_LOCAL_PATH = "uploadAttachment.pages";
        private const string ATTACHMENT_NAME = "AttachmentUploaded.txt";
        private const string ATTACHMENT_DESCRIPTION = "File uploaded using REST API";
        private const string ATTACHMENT_CONTENT_TYPE = "text/plain";
        private const int PROJECT_ID = 9;
        private const string ISSUE_SUBJECT = "Issue with attachments";
        private const string ATTACHMENT_ID = "48";
        private const string ATTACHMENT_FILE_NAME = "uploadAttachment.pages";

        private readonly RedmineFixture fixture;

        [Fact]
        public void Should_Download_Attachment()
        {
            var url = Helper.Uri + "/attachments/download/" + ATTACHMENT_ID + "/" + ATTACHMENT_FILE_NAME;

            var document = fixture.RedmineManager.DownloadFile(url);

            Assert.NotNull(document);
        }

        [Fact]
        public void Should_Get_Attachment_By_Id()
        {
            var attachment = fixture.RedmineManager.GetObject<Attachment>(ATTACHMENT_ID, null);

            Assert.NotNull(attachment);
            Assert.IsType<Attachment>(attachment);
            Assert.True(attachment.FileName == ATTACHMENT_FILE_NAME, "Attachment file name is not the expected one.");
        }

        [Fact]
        public void Should_Upload_Attachment()
        {
            //read document from specified path
            var documentData = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + ATTACHMENT_LOCAL_PATH);

            //upload attachment to redmine
            var attachment = fixture.RedmineManager.UploadFile(documentData);

            //set attachment properties
            attachment.FileName = ATTACHMENT_NAME;
            attachment.Description = ATTACHMENT_DESCRIPTION;
            attachment.ContentType = ATTACHMENT_CONTENT_TYPE;

            //create list of attachments to be added to issue
            IList<Upload> attachments = new List<Upload>();
            attachments.Add(attachment);

            var issue = new Issue
            {
                Project = new Project {Id = PROJECT_ID},
                Subject = ISSUE_SUBJECT,
                Uploads = attachments
            };

            //create issue and attach document
            var issueWithAttachment = fixture.RedmineManager.CreateObject(issue);

            issue = fixture.RedmineManager.GetObject<Issue>(issueWithAttachment.Id.ToString(),
                new NameValueCollection {{RedmineKeys.INCLUDE, RedmineKeys.ATTACHMENTS}});

            Assert.NotNull(issue);
            Assert.NotNull(issue.Attachments);
            Assert.All(issue.Attachments, a => Assert.IsType<Attachment>(a));
            Assert.True(issue.Attachments.Count == 1, "Number of attachments != 1");
            Assert.True(issue.Attachments[0].FileName == ATTACHMENT_NAME, "Attachment name is not correct.");
            Assert.True(issue.Attachments[0].Description == ATTACHMENT_DESCRIPTION,
                "Attachment description is not correct.");
            Assert.True(issue.Attachments[0].ContentType == ATTACHMENT_CONTENT_TYPE,
                "Attachment content type is not correct.");
        }
    }
}