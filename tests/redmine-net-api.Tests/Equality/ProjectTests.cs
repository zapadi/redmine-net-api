using System;
using Redmine.Net.Api.Types;

namespace Padi.DotNet.RedmineAPI.Tests.Equality;

public sealed class ProjectTests : BaseEqualityTests<Project>
{
    protected override Project CreateSampleInstance()
    {
        return new Project
        {
            Id = 1,
            Name = "Test Project",
            Identifier = "test-project",
            Description = "Test Description",
            HomePage = "https://test.com",
            Status = ProjectStatus.Active,
            IsPublic = true,
            InheritMembers = true,
            DefaultAssignee = new IdentifiableName(5, "DefaultAssignee"),
            DefaultVersion = new IdentifiableName(5, "DefaultVersion"),
            Parent = new IdentifiableName { Id = 1, Name = "Parent Project" },
            CreatedOn = new DateTime(2023, 1, 1).Date,
            UpdatedOn = new DateTime(2023, 1, 1).Date,
            Trackers = 
            [
                new() { Id = 1, Name = "Bug" },
                new() { Id = 2, Name = "Feature" }
            ],
            
            CustomFields = 
            [
                new() { Id = 1, Name = "Field1"},
                new() { Id = 2, Name = "Field2"}
            ],
            
            IssueCategories =
            [
                new() { Id = 1, Name = "Category1" },
                new() { Id = 2, Name = "Category2" }
            ],
            EnabledModules = 
            [
                new() { Id = 1, Name = "Module1" },
                new() { Id = 2, Name = "Module2" }
            ],
            TimeEntryActivities = 
            [
                new() { Id = 1, Name = "Activity1" },
                new() { Id = 2, Name = "Activity2" }
            ]
        };
    }

    protected override Project CreateDifferentInstance()
    {
        return new Project
        {
            Id = 2,
            Name = "Different Project",
            Identifier = "different-project",
            Description = "Different Description",
            HomePage = "https://different.com",
            Status = ProjectStatus.Archived,
            IsPublic = false,
            Parent = new IdentifiableName { Id = 2, Name = "Different Parent" },
            CreatedOn = new DateTime(2023, 1, 2).Date,
            UpdatedOn = new DateTime(2023, 1, 2).Date,
            Trackers = 
            [
                new() { Id = 3, Name = "Different Bug" }
            ],
            CustomFields =
            [
                new() { Id = 3, Name = "DifferentField"}
            ],
            IssueCategories = 
            [
                new() { Id = 3, Name = "DifferentCategory" }
            ],
            EnabledModules = 
            [
                new() { Id = 3, Name = "DifferentModule" }
            ],
            TimeEntryActivities = 
            [
                new() { Id = 3, Name = "DifferentActivity" }
            ]
        };
    }
}