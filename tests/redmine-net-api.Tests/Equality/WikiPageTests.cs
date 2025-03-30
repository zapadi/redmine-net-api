using System;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class WikiPageTests : BaseEqualityTests<WikiPage>
{
    protected override WikiPage CreateSampleInstance()
    {
        return new WikiPage
        {
            Id = 1,
            Title = "Home Page",
            Text = "Welcome to the wiki",
            Version = 1,
            Author = new IdentifiableName { Id = 1, Name = "Author 1" },
            Comments = "Initial version",
            CreatedOn = new DateTime(2023, 1, 1),
            UpdatedOn = new DateTime(2023, 1, 1),
            Attachments = 
            [
                new Attachment
                {
                    Id = 1,
                    FileName = "doc.pdf",
                    FileSize = 1024,
                    Author = new IdentifiableName { Id = 1, Name = "Author 1" }
                }
            ]
        };
    }

    protected override WikiPage CreateDifferentInstance()
    {
        return new WikiPage
        {
            Id = 2,
            Title = "Different Page",
            Text = "Different content",
            Version = 2,
            Author = new IdentifiableName { Id = 2, Name = "Author 2" },
            Comments = "Updated version",
            CreatedOn = new DateTime(2023, 1, 2),
            UpdatedOn = new DateTime(2023, 1, 2)
        };
    }
}