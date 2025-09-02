using System;
using System.Text.Json;
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Serialization.JsonText.Extensions;
using Padi.RedmineApi.Types;
using Types_Version = Padi.RedmineApi.Types.Version;
using Version = Padi.RedmineApi.Types.Version;

namespace Padi.RedmineApi.Serialization.JsonText.Converters;

internal sealed class AttachmentConverter : JsonTextRedmineConverter<Attachment>
{
    public override string RootName { get; } = RedmineKeys.ATTACHMENT;

    public override Attachment ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Attachment();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.AUTHOR: entity.Author = converter.ReadJson(ref reader); break;
                case RedmineKeys.CONTENT_TYPE: entity.ContentType = reader.ReadAsString(); break;
                case RedmineKeys.CONTENT_URL: entity.ContentUrl = reader.ReadAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.FILE_NAME: entity.FileName = reader.ReadAsString(); break;
                case RedmineKeys.FILE_SIZE: entity.FileSize = reader.ReadAsInt(); break;
                case RedmineKeys.THUMBNAIL_URL: entity.ThumbnailUrl = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Attachment entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.FILE_NAME, entity.FileName);
            writer.WriteProperty(RedmineKeys.DESCRIPTION, entity.Description);
        }
    }
}

internal sealed class ChangeSetConverter : JsonTextRedmineConverter<ChangeSet>
{
    public override string RootName { get; } = RedmineKeys.CHANGE_SET;

    public override ChangeSet ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new ChangeSet();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.COMMENTS: entity.Comments = reader.ReadAsString(); break;
                case RedmineKeys.COMMITTED_ON: entity.CommittedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.REVISION: entity.Revision = reader.ReadAsString(); break;
                case RedmineKeys.USER: entity.User = converter.ReadJson(ref reader); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, ChangeSet value, bool writeRootName) { }
}

internal sealed class CustomFieldConverter : JsonTextRedmineConverter<CustomField>
{
    public override string RootName { get; } = RedmineKeys.CUSTOM_FIELD;

    public override CustomField ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new CustomField();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var value = reader.GetString();

            switch (value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.CUSTOMIZED_TYPE: entity.CustomizedType = reader.ReadAsString(); break;
                case RedmineKeys.DEFAULT_VALUE: entity.DefaultValue = reader.ReadAsString(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.FIELD_FORMAT: entity.FieldFormat = reader.ReadAsString(); break;
                case RedmineKeys.IS_FILTER: entity.IsFilter = reader.ReadAsBool(); break;
                case RedmineKeys.IS_REQUIRED: entity.IsRequired = reader.ReadAsBool(); break;
                case RedmineKeys.MAX_LENGTH: entity.MaxLength = reader.ReadAsNullableInt(); break;
                case RedmineKeys.MIN_LENGTH: entity.MinLength = reader.ReadAsNullableInt(); break;
                case RedmineKeys.MULTIPLE: entity.Multiple = reader.ReadAsBool(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.POSSIBLE_VALUES: entity.PossibleValues = reader.ReadAsCollection<CustomFieldPossibleValue>(); break;
                case RedmineKeys.REGEXP: entity.Regexp = reader.ReadAsString(); break;
                case RedmineKeys.ROLES: entity.Roles = reader.ReadAsCollection<CustomFieldRole>(); break;
                case RedmineKeys.SEARCHABLE: entity.Searchable = reader.ReadAsBool(); break;
                case RedmineKeys.TRACKERS: entity.Trackers = reader.ReadAsCollection<TrackerCustomField>(); break;
                case RedmineKeys.VISIBLE: entity.Visible = reader.ReadAsBool(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, CustomField value, bool writeRootName) { }
}

internal sealed class CustomFieldPossibleValueConverter : JsonTextRedmineConverter<CustomFieldPossibleValue>
{
    public override string RootName { get; } = RedmineKeys.POSSIBLE_VALUE;

    public override CustomFieldPossibleValue ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new CustomFieldPossibleValue();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch (propertyName)
            {
                case RedmineKeys.LABEL: entity.Label = reader.ReadAsString(); break;
                case RedmineKeys.VALUE: entity.Value = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, CustomFieldPossibleValue value, bool writeRootName) { }
}

internal sealed class CustomFieldRoleConverter : JsonTextRedmineConverter<CustomFieldRole>
{
    public override string RootName { get; } = RedmineKeys.ROLE;

    public override CustomFieldRole ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new CustomFieldRole();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var value = reader.GetString();

            switch (value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, CustomFieldRole value, bool writeRootName) { }
}

internal sealed class CustomFieldValueConverter : JsonTextRedmineConverter<CustomFieldValue>
{
    public override string RootName { get; } = RedmineKeys.VALUE;

    public override CustomFieldValue ReadJson(ref Utf8JsonReader reader)
    {
        string? info = null;

        if (reader.TokenType == JsonTokenType.String)
        {
            info = reader.GetString();
        }

        return new CustomFieldValue { Info = info};
    }

    public override void WriteJson(Utf8JsonWriter writer, CustomFieldValue value, bool writeRootName) { }
}

internal sealed class DetailConverter : JsonTextRedmineConverter<Detail>
{
    public override string RootName { get; } = RedmineKeys.DETAIL;

    public override Detail ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Detail();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var value = reader.GetString();

            switch (value)
            {
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.PROPERTY: entity.Property = reader.ReadAsString(); break;
                case RedmineKeys.NEW_VALUE: entity.NewValue = reader.ReadAsString(); break;
                case RedmineKeys.OLD_VALUE: entity.OldValue = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }
        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Detail value, bool writeRootName) { }
}

internal sealed class DocumentCategoryConverter : JsonTextRedmineConverter<DocumentCategory>
{
    public override string RootName { get; } = RedmineKeys.DOCUMENT_CATEGORY;

    public override DocumentCategory ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new DocumentCategory();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.IS_DEFAULT: entity.IsDefault = reader.ReadAsBool(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.ACTIVE: entity.IsActive = reader.ReadAsBool(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, DocumentCategory value, bool writeRootName) { }
}

internal sealed class ErrorConverter : JsonTextRedmineConverter<Error>
{
    public override string RootName { get; } = RedmineKeys.ERROR;

    public override Error ReadJson(ref Utf8JsonReader reader)
    {
        string? info = null;
        if (reader.TokenType == JsonTokenType.String)
        {
            info = reader.GetString();
        }

        return new Error { Info = info};
    }

    public override void WriteJson(Utf8JsonWriter writer, Error value, bool writeRootName) { }
}

internal sealed class FileConverter : JsonTextRedmineConverter<File>
{
    public override string RootName { get; } = RedmineKeys.FILE;

    public override File ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new File();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.AUTHOR: entity.Author = converter.ReadJson(ref reader); break;
                case RedmineKeys.CONTENT_TYPE: entity.ContentType = reader.ReadAsString(); break;
                case RedmineKeys.CONTENT_URL: entity.ContentUrl = reader.ReadAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.DIGEST: entity.Digest = reader.ReadAsString(); break;
                case RedmineKeys.DOWNLOADS: entity.Downloads = reader.ReadAsInt(); break;
                case RedmineKeys.FILE_NAME: entity.Filename = reader.ReadAsString(); break;
                case RedmineKeys.FILE_SIZE: entity.FileSize = reader.ReadAsInt(); break;
                case RedmineKeys.TOKEN: entity.Token = reader.ReadAsString(); break;
                case RedmineKeys.VERSION: entity.Version = converter.ReadJson(ref reader); break;
                case RedmineKeys.VERSION_ID: entity.Version = IdentifiableName.Create<IdentifiableName>(reader.ReadAsInt()); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, File entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
           // using (new JsonObject(writer))
           // {
                writer.WriteProperty(RedmineKeys.TOKEN, entity.Token);
                writer.WriteIdIfNotNull(RedmineKeys.VERSION_ID, entity.Version);
                writer.WriteProperty(RedmineKeys.FILE_NAME, entity.Filename);
                writer.WriteProperty(RedmineKeys.DESCRIPTION, entity.Description);
           // }
        }
    }
}

internal sealed class GroupConverter : JsonTextRedmineConverter<Group>
{
    public override string RootName { get; } = RedmineKeys.GROUP;

    public override Group ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Group();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                case RedmineKeys.MEMBERSHIPS: entity.Memberships = reader.ReadAsCollection<Membership>(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.USERS: entity.Users = reader.ReadAsCollection<GroupUser>(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Group entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.NAME, entity.Name);
            writer.WriteRepeatableElement(RedmineKeys.USER_IDS, entity.Users);
        }
    }
}

internal sealed class GroupUserConverter : JsonTextRedmineConverter<GroupUser>
{
    public override string RootName { get; } = RedmineKeys.USER;

    public override GroupUser ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new GroupUser();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, GroupUser value, bool writeRootName) { }
}

internal sealed class IdentifiableNameConverter : JsonTextRedmineConverter<IdentifiableName>
{
    public override string RootName { get; }

    public override IdentifiableName ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new IdentifiableName();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch (propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, IdentifiableName entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteIdIfNotNull(RedmineKeys.ID, entity);
            if (!entity.Name.IsNullOrWhiteSpace())
            {
                writer.WriteProperty(RedmineKeys.NAME, entity.Name);
            }
        }
    }
}


internal sealed class IssueAllowedStatusConverter : JsonTextRedmineConverter<IssueAllowedStatus>
{
    public override string RootName { get; } = RedmineKeys.STATUS;

    public override IssueAllowedStatus ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new IssueAllowedStatus();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.IS_CLOSED: entity.IsClosed = reader.ReadAsBool(); break;
                default: reader.Read(); break;
            }
        }
        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, IssueAllowedStatus value, bool writeRootName) { }
}

internal sealed class IssueCategoryConverter : JsonTextRedmineConverter<IssueCategory>
{
    public override string RootName { get; } = RedmineKeys.ISSUE_CATEGORY;

    public override IssueCategory ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new IssueCategory();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.ASSIGNED_TO: entity.AssignTo = converter.ReadJson(ref reader); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.PROJECT: entity.Project = converter.ReadJson(ref reader); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, IssueCategory entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, entity.Project);
            writer.WriteProperty(RedmineKeys.NAME, entity.Name);
            writer.WriteIdIfNotNull(RedmineKeys.ASSIGNED_TO_ID, entity.AssignTo);
        }
    }
}

internal sealed class IssueChildConverter : JsonTextRedmineConverter<IssueChild>
{
    public override string RootName { get; } = RedmineKeys.ISSUE;

    public override IssueChild ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new IssueChild();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.SUBJECT: entity.Subject = reader.ReadAsString(); break;
                case RedmineKeys.TRACKER: entity.Tracker = converter.ReadJson(ref reader); break;
                default: reader.Read(); break;
            }
        }
        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, IssueChild value, bool writeRootName) { }
}

internal sealed class IssueConverter : JsonTextRedmineConverter<Issue>
{
    public override string RootName { get; } = RedmineKeys.ISSUE;

    public override Issue ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Issue();

        var identifiableNameConverter = JsonConverterRegistry.GetConverter<IdentifiableName>();
        var issueStatusConverter = JsonConverterRegistry.GetConverter<IssueStatus>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch (propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.ALLOWED_STATUSES: entity.AllowedStatuses = reader.ReadAsCollection<IssueAllowedStatus>(); break;
                case RedmineKeys.ASSIGNED_TO: entity.AssignedTo = identifiableNameConverter.ReadJson(ref reader); break;
                case RedmineKeys.ATTACHMENTS: entity.Attachments = reader.ReadAsCollection<Attachment>(); break;
                case RedmineKeys.AUTHOR: entity.Author = identifiableNameConverter.ReadJson(ref reader); break;
                case RedmineKeys.CATEGORY: entity.Category = identifiableNameConverter.ReadJson(ref reader); break;
                case RedmineKeys.CHANGE_SETS: entity.ChangeSets = reader.ReadAsCollection<ChangeSet>(); break;
                case RedmineKeys.CHILDREN: entity.Children = reader.ReadAsCollection<IssueChild>(); break;
                case RedmineKeys.CLOSED_ON: entity.ClosedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.DONE_RATIO: entity.DoneRatio = reader.ReadAsNullableFloat(); break;
                case RedmineKeys.DUE_DATE: entity.DueDate = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.ESTIMATED_HOURS: entity.EstimatedHours = reader.ReadAsNullableFloat(); break;
                case RedmineKeys.FIXED_VERSION: entity.FixedVersion = identifiableNameConverter.ReadJson(ref reader); break;
                case RedmineKeys.IS_PRIVATE: entity.IsPrivate = reader.ReadAsBool(); break;
                case RedmineKeys.JOURNALS: entity.Journals = reader.ReadAsCollection<Journal>(); break;
                case RedmineKeys.NOTES: entity.Notes = reader.ReadAsString(); break;
                case RedmineKeys.PARENT: entity.ParentIssue = identifiableNameConverter.ReadJson(ref reader); break;
                case RedmineKeys.PRIORITY: entity.Priority = identifiableNameConverter.ReadJson(ref reader); break;
                case RedmineKeys.PRIVATE_NOTES: entity.PrivateNotes = reader.ReadAsBool(); break;
                case RedmineKeys.PROJECT: entity.Project = identifiableNameConverter.ReadJson(ref reader); break;
                case RedmineKeys.RELATIONS: entity.Relations = reader.ReadAsCollection<IssueRelation>(); break;
                case RedmineKeys.SPENT_HOURS: entity.SpentHours = reader.ReadAsNullableFloat(); break;
                case RedmineKeys.START_DATE: entity.StartDate = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.STATUS: entity.Status = issueStatusConverter.ReadJson(ref reader); break;
                case RedmineKeys.SUBJECT: entity.Subject = reader.ReadAsString(); break;
                case RedmineKeys.TOTAL_ESTIMATED_HOURS: entity.TotalEstimatedHours = reader.ReadAsNullableFloat(); break;
                case RedmineKeys.TOTAL_SPENT_HOURS: entity.TotalSpentHours = reader.ReadAsNullableFloat(); break;
                case RedmineKeys.TRACKER: entity.Tracker = identifiableNameConverter.ReadJson(ref reader); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.WATCHERS: entity.Watchers = reader.ReadAsCollection<Watcher>(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Issue entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.SUBJECT, entity.Subject);
            writer.WriteProperty(RedmineKeys.DESCRIPTION, entity.Description);
            writer.WriteProperty(RedmineKeys.NOTES, entity.Notes);

            if (entity.Id != 0)
            {
                writer.WriteBoolean(RedmineKeys.PRIVATE_NOTES, entity.PrivateNotes);
            }

            writer.WriteBoolean(RedmineKeys.IS_PRIVATE, entity.IsPrivate);
            writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, entity.Project);
            writer.WriteIdIfNotNull(RedmineKeys.PRIORITY_ID, entity.Priority);
            writer.WriteIdIfNotNull(RedmineKeys.STATUS_ID, entity.Status);
            writer.WriteIdIfNotNull(RedmineKeys.CATEGORY_ID, entity.Category);
            writer.WriteIdIfNotNull(RedmineKeys.TRACKER_ID, entity.Tracker);
            writer.WriteIdIfNotNull(RedmineKeys.ASSIGNED_TO_ID, entity.AssignedTo);
            writer.WriteIdIfNotNull(RedmineKeys.FIXED_VERSION_ID, entity.FixedVersion);
            writer.WriteValueOrEmpty(RedmineKeys.ESTIMATED_HOURS, entity.EstimatedHours);

            writer.WriteIdOrEmpty(RedmineKeys.PARENT_ISSUE_ID, entity.ParentIssue);
            writer.WriteDateOrEmpty(RedmineKeys.START_DATE, entity.StartDate);
            writer.WriteDateOrEmpty(RedmineKeys.DUE_DATE, entity.DueDate);
            writer.WriteDateOrEmpty(RedmineKeys.UPDATED_ON, entity.UpdatedOn);

            if (entity.DoneRatio != null)
            {
                writer.WriteProperty(RedmineKeys.DONE_RATIO, entity.DoneRatio.Value.ToInvariantString());
            }

            if (entity.SpentHours != null)
            {
                writer.WriteProperty(RedmineKeys.SPENT_HOURS, entity.SpentHours.Value.ToInvariantString());
            }

            writer.WriteArray(RedmineKeys.UPLOADS, entity.Uploads, false);
            writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields);

            writer.WriteRepeatableElement(RedmineKeys.WATCHER_USER_IDS, entity.Watchers);
        }
    }

}

internal sealed class IssueCustomFieldConverter : JsonTextRedmineConverter<IssueCustomField>
{
    public override string RootName { get; } = RedmineKeys.CUSTOM_FIELD;

    public override IssueCustomField ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new IssueCustomField();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();

                switch (propertyName)
                {
                    case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                    case RedmineKeys.MULTIPLE: entity.Multiple = reader.ReadAsBool(); break;
                    case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                    case RedmineKeys.VALUE:
                        reader.Read();
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.Null: break;
                            case JsonTokenType.StartArray:
                                entity.Values = reader.ReadAsCollection<CustomFieldValue>();
                                break;
                            default:
                                entity.Values = [new CustomFieldValue { Info = reader.GetString() }];
                                break;
                        }

                        break;
                }
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, IssueCustomField entity, bool writeRootName)
    {
        if (entity.Values == null)
        {
            return;
        }

        var itemsCount = entity.Values.Count;
        entity.Multiple = itemsCount > 1;

        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.ID, entity.Id);
            writer.WriteProperty(RedmineKeys.NAME, entity.Name);
            writer.WriteBoolean(RedmineKeys.MULTIPLE, entity.Multiple);

            if (entity.Multiple)
            {
                writer.WritePropertyName(RedmineKeys.VALUE);
                writer.WriteStartArray();
                foreach (var cfv in entity.Values)
                {
                    writer.WriteStringValue(cfv.Info);
                }

                writer.WriteEndArray();
            }
            else
            {
                writer.WriteProperty(RedmineKeys.VALUE, itemsCount > 0 ? entity.Values[0].Info : null);
            }
        }
    }
}

internal sealed class IssuePriorityConverter : JsonTextRedmineConverter<IssuePriority>
{
    public override string RootName { get; } = RedmineKeys.ISSUE_PRIORITY;

    public override IssuePriority ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new IssuePriority();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.ACTIVE: entity.IsActive = reader.ReadAsBool(); break;
                case RedmineKeys.IS_DEFAULT: entity.IsDefault = reader.ReadAsBool(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }
        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, IssuePriority value, bool writeRootName) { }
}

internal sealed class IssueRelationConverter : JsonTextRedmineConverter<IssueRelation>
{
    public override string RootName { get; } = RedmineKeys.RELATION;

    public override IssueRelation ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new IssueRelation();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.DELAY: entity.Delay = reader.ReadAsNullableInt(); break;
                case RedmineKeys.ISSUE_ID: entity.IssueId = reader.ReadAsInt(); break;
                case RedmineKeys.ISSUE_TO_ID: entity.IssueToId = reader.ReadAsInt(); break;
                case RedmineKeys.RELATION_TYPE: entity.Type = ReadIssueRelationType(reader.ReadAsString()); break;
            }
        }


        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, IssueRelation entity, bool writeRootName)
    {
        if (entity.Type == IssueRelationType.Undefined)
        {
            throw new RedmineException(RedmineApiErrorCode.SerializationError, $"The value `{nameof(IssueRelationType)}.`{nameof(IssueRelationType.Undefined)}` is not allowed to create relations!");
        }

        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.ISSUE_TO_ID, entity.IssueToId);
            writer.WriteProperty(RedmineKeys.RELATION_TYPE, entity.Type.ToLowerInvariant());

            if (entity.Type is IssueRelationType.Precedes or IssueRelationType.Follows)
            {
                writer.WriteValueOrEmpty(RedmineKeys.DELAY, entity.Delay);
            }
        }
    }

    private static IssueRelationType ReadIssueRelationType(string value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return IssueRelationType.Undefined;
        }

        if (short.TryParse(value, out var enumId))
        {
            return (IssueRelationType)enumId;
        }

        if (RedmineKeys.COPIED_TO.Equals(value, StringComparison.OrdinalIgnoreCase))
        {
            return IssueRelationType.CopiedTo;
        }

        if (RedmineKeys.COPIED_FROM.Equals(value, StringComparison.OrdinalIgnoreCase))
        {
            return IssueRelationType.CopiedFrom;
        }

        return EnumHelper.Parse<IssueRelationType>(value, true);
    }
}

internal sealed class IssueStatusConverter : JsonTextRedmineConverter<IssueStatus>
{
    public override string RootName { get; } = RedmineKeys.ISSUE_STATUS;

    public override IssueStatus ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new IssueStatus();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.IS_CLOSED: entity.IsClosed = reader.ReadAsBool(); break;
                case RedmineKeys.IS_DEFAULT: entity.IsDefault = reader.ReadAsBool(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, IssueStatus value, bool writeRootName) { }
}

internal sealed class JournalConverter : JsonTextRedmineConverter<Journal>
{
    public override string RootName { get; } = RedmineKeys.JOURNAL;

    public override Journal ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Journal();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.DETAILS: entity.Details = reader.ReadAsCollection<Detail>(); break;
                case RedmineKeys.NOTES: entity.Notes = reader.ReadAsString(); break;
                case RedmineKeys.PRIVATE_NOTES: entity.PrivateNotes = reader.ReadAsBool(); break;
                case RedmineKeys.USER: entity.User = converter.ReadJson(ref reader); break;
                case RedmineKeys.UPDATED_BY: entity.UpdatedBy = converter.ReadJson(ref reader); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Journal entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.NOTES, entity.Notes);
        }
    }
}

internal sealed class MembershipConverter : JsonTextRedmineConverter<Membership>
{
    public override string RootName { get; } = RedmineKeys.MEMBERSHIP;

    public override Membership ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Membership();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch (propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.GROUP: entity.Group = converter.ReadJson(ref reader); break;
                case RedmineKeys.PROJECT: entity.Project = converter.ReadJson(ref reader); break;
                case RedmineKeys.USER: entity.User = converter.ReadJson(ref reader); break;
                case RedmineKeys.ROLES: entity.Roles = reader.ReadAsCollection<MembershipRole>(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Membership value, bool writeRootName) { }
}

internal sealed class MembershipRoleConverter : JsonTextRedmineConverter<MembershipRole>
{
    public override string RootName { get; } = RedmineKeys.POSSIBLE_VALUE;

    public override MembershipRole? ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new MembershipRole();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.INHERITED: entity.Inherited = reader.ReadAsBool(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }
        return null;
    }

    public override void WriteJson(Utf8JsonWriter writer, MembershipRole entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.ID, entity.Id.ToInvariantString());
        }
    }
}

internal sealed class MyAccountConverter : JsonTextRedmineConverter<MyAccount>
{
    public override string RootName { get; } = RedmineKeys.USER;

    public override MyAccount ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new MyAccount();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.ADMIN: entity.IsAdmin = reader.ReadAsBool(); break;
                case RedmineKeys.API_KEY: entity.ApiKey = reader.ReadAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadAsCollection<MyAccountCustomField>(); break;
                case RedmineKeys.FIRST_NAME: entity.FirstName = reader.ReadAsString(); break;
                case RedmineKeys.LAST_LOGIN_ON: entity.LastLoginOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.LAST_NAME: entity.LastName = reader.ReadAsString(); break;
                case RedmineKeys.LOGIN: entity.Login = reader.ReadAsString(); break;
                case RedmineKeys.MAIL: entity.Email = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, MyAccount entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.FIRST_NAME, entity.FirstName);
            writer.WriteProperty(RedmineKeys.LAST_NAME, entity.LastName);
            writer.WriteProperty(RedmineKeys.MAIL, entity.Email);
        }
    }
}

internal sealed class MyAccountCustomFieldConverter : JsonTextRedmineConverter<MyAccountCustomField>
{
    public override string RootName { get; } = RedmineKeys.CUSTOM_FIELD;

    public override MyAccountCustomField ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new MyAccountCustomField();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.VALUE: entity.Value = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, MyAccountCustomField entity, bool  writeRootName) { }
}

internal sealed class NewsCommentConverter : JsonTextRedmineConverter<NewsComment>
{
    public override string RootName { get; } = RedmineKeys.COMMENT;

    public override NewsComment ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new NewsComment();

        var identifiableNameConverter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch (propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.AUTHOR: entity.Author = identifiableNameConverter.ReadJson(ref reader); break;
                case RedmineKeys.CONTENT: entity.Content = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, NewsComment value, bool  writeRootName) { }
}

internal sealed class NewsConverter : JsonTextRedmineConverter<News>
{
    public override string RootName { get; } = RedmineKeys.NEWS;

    public override News ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new News();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.AUTHOR: entity.Author = converter.ReadJson(ref reader); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.PROJECT: entity.Project = converter.ReadJson(ref reader); break;
                case RedmineKeys.SUMMARY: entity.Summary = reader.ReadAsString(); break;
                case RedmineKeys.TITLE: entity.Title = reader.ReadAsString(); break;
                case RedmineKeys.ATTACHMENTS: entity.Attachments = reader.ReadAsCollection<Attachment>(); break;
                case RedmineKeys.COMMENTS: entity.Comments = reader.ReadAsCollection<NewsComment>(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, News entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.TITLE, entity.Title);
            writer.WriteProperty(RedmineKeys.SUMMARY, entity.Summary);
            writer.WriteProperty(RedmineKeys.DESCRIPTION, entity.Description);
            writer.WriteArray(RedmineKeys.UPLOADS, entity.Uploads);
        }
    }
}

internal sealed class PermissionConverter : JsonTextRedmineConverter<Permission>
{
    public override string RootName { get; } = RedmineKeys.POSSIBLE_VALUE;

    public override Permission ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Permission();

        if (reader.TokenType == JsonTokenType.String)
        {
            entity.Info = reader.GetString();
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Permission value, bool  writeRootName) { }
}

internal sealed class ProjectConverter : JsonTextRedmineConverter<Project>
{
    public override string RootName { get; } = RedmineKeys.PROJECT;

    public override Project ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Project();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

       while (reader.Read())
       {
           if (reader.TokenType == JsonTokenType.EndObject)
           {
               return entity;
           }

           if (reader.TokenType != JsonTokenType.PropertyName)
           {
               continue;
           }

           var propertyName = reader.GetString();

           switch (propertyName)
           {
               case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
               case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsNullableDateTime(); break;
               case RedmineKeys.ISSUE_CUSTOM_FIELDS: entity.IssueCustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
               case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
               case RedmineKeys.ENABLED_MODULES: entity.EnabledModules = reader.ReadAsCollection<ProjectEnabledModule>(); break;
               case RedmineKeys.HOMEPAGE: entity.HomePage = reader.ReadAsString(); break;
               case RedmineKeys.IDENTIFIER: entity.Identifier = reader.ReadAsString(); break;
               case RedmineKeys.INHERIT_MEMBERS: entity.InheritMembers = reader.ReadAsBool(); break;
               case RedmineKeys.IS_PUBLIC: entity.IsPublic = reader.ReadAsBool(); break;
               case RedmineKeys.ISSUE_CATEGORIES: entity.IssueCategories = reader.ReadAsCollection<ProjectIssueCategory>(); break;
               case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
               case RedmineKeys.PARENT: entity.Parent = converter.ReadJson(ref reader); break;
               case RedmineKeys.STATUS: entity.Status = (ProjectStatus)reader.ReadAsInt(); break;
               case RedmineKeys.TIME_ENTRY_ACTIVITIES: entity.TimeEntryActivities = reader.ReadAsCollection<ProjectTimeEntryActivity>(); break;
               case RedmineKeys.TRACKERS: entity.Trackers = reader.ReadAsCollection<ProjectTracker>(); break;
               case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsNullableDateTime(); break;
               case RedmineKeys.DEFAULT_ASSIGNEE: entity.DefaultAssignee = converter.ReadJson(ref reader); break;
               case RedmineKeys.DEFAULT_VERSION: entity.DefaultVersion = converter.ReadJson(ref reader); break;
               default: reader.Read(); break;
           }
       }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Project entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.NAME, entity.Name);
            writer.WriteProperty(RedmineKeys.IDENTIFIER, entity.Identifier);
            writer.WriteIfNotDefaultOrNull(RedmineKeys.DESCRIPTION, entity.Description);
            writer.WriteIfNotDefaultOrNull(RedmineKeys.HOMEPAGE, entity.HomePage);
            writer.WriteBoolean(RedmineKeys.INHERIT_MEMBERS, entity.InheritMembers);
            writer.WriteBoolean(RedmineKeys.IS_PUBLIC, entity.IsPublic);
            writer.WriteIdIfNotNull(RedmineKeys.PARENT_ID, entity.Parent);

            //It works only when the new project is a subproject, and it inherits the members.
            writer.WriteIdIfNotNull(RedmineKeys.DEFAULT_ASSIGNED_TO_ID, entity.DefaultAssignee);
            //It works only with existing shared versions.
            writer.WriteIdIfNotNull(RedmineKeys.DEFAULT_VERSION_ID, entity.DefaultVersion);

            writer.WriteArrayIds(RedmineKeys.TRACKER_IDS, entity.Trackers);
            writer.WriteArrayNames(RedmineKeys.ENABLED_MODULE_NAMES, entity.EnabledModules);
            writer.WriteArrayIds(RedmineKeys.ISSUE_CUSTOM_FIELD_IDS, entity.IssueCustomFields);

            writer.WritePropertyName(RedmineKeys.CUSTOM_FIELD_VALUES);
            writer.WriteStartObject();

            if(entity.CustomFieldValues != null)
            {
                foreach (var idn in entity.CustomFieldValues)
                {
                    writer.WriteProperty(idn.Id.ToInvariantString(), idn.Name);
                }
            }

            writer.WriteEndObject();
        }
    }
}

internal sealed class ProjectEnabledModuleConverter : JsonTextRedmineConverter<ProjectEnabledModule>
{
    public override string RootName { get; } = RedmineKeys.ENABLED_MODULE;

    public override ProjectEnabledModule ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new ProjectEnabledModule();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, ProjectEnabledModule value, bool writeRootName) { }
}

internal sealed class ProjectIssueCategoryConverter : JsonTextRedmineConverter<ProjectIssueCategory>
{
    public override string RootName { get; } = RedmineKeys.ISSUE_CATEGORY;

    public override ProjectIssueCategory ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new ProjectIssueCategory();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, ProjectIssueCategory value, bool  writeRootName) { }
}

internal sealed class ProjectMembershipConverter : JsonTextRedmineConverter<ProjectMembership>
{
    public override string RootName { get; } = RedmineKeys.MEMBERSHIP;

    public override ProjectMembership ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new ProjectMembership();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString(); switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.GROUP: entity.Group = converter.ReadJson(ref reader); break;
                case RedmineKeys.PROJECT: entity.Project = converter.ReadJson(ref reader); break;
                case RedmineKeys.ROLES: entity.Roles = reader.ReadAsCollection<MembershipRole>(); break;
                case RedmineKeys.USER: entity.User = converter.ReadJson(ref reader); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, ProjectMembership entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            if (entity.Id <= 0)
            {
                writer.WriteIdIfNotNull(RedmineKeys.USER_ID, entity.User);
            }

            writer.WriteArrayIds(RedmineKeys.ROLE_IDS, entity.Roles);
        }
    }
}

internal sealed class ProjectTrackerConverter : JsonTextRedmineConverter<ProjectTracker>
{
    public override string RootName { get; } = RedmineKeys.TRACKER;

    public override ProjectTracker ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new ProjectTracker();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, ProjectTracker value, bool  writeRootName) { }
}

internal sealed class ProjectTimeEntryActivityConverter : JsonTextRedmineConverter<ProjectTimeEntryActivity>
{
    public override string RootName { get; } = RedmineKeys.TIME_ENTRY_ACTIVITY;

    public override ProjectTimeEntryActivity ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new ProjectTimeEntryActivity();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, ProjectTimeEntryActivity value, bool writeRootName) { }
}

internal sealed class QueryConverter : JsonTextRedmineConverter<Query>
{
    public override string RootName { get; } = RedmineKeys.QUERY;

    public override Query ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Query();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.IS_PUBLIC: entity.IsPublic = reader.ReadAsBool(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.PROJECT_ID: entity.ProjectId = reader.ReadAsNullableInt(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Query value, bool  writeRootName) { }
}

internal sealed class RoleConverter : JsonTextRedmineConverter<Role>
{
    public override string RootName { get; } = RedmineKeys.ROLE;

    public override Role ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Role();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.ASSIGNABLE: entity.IsAssignable = reader.ReadAsNullableBoolean(); break;
                case RedmineKeys.ISSUES_VISIBILITY: entity.IssuesVisibility = reader.ReadAsString(); break;
                case RedmineKeys.TIME_ENTRIES_VISIBILITY: entity.TimeEntriesVisibility = reader.ReadAsString(); break;
                case RedmineKeys.USERS_VISIBILITY: entity.UsersVisibility = reader.ReadAsString(); break;
                case RedmineKeys.PERMISSIONS: entity.Permissions = reader.ReadAsCollection<Permission>(); break;
                default: reader.Read(); break;
            }
        }
        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Role value, bool writeRootName) { }
}

internal sealed class SearchConverter : JsonTextRedmineConverter<Search>
{
    public override string RootName { get; } = RedmineKeys.RESULT;

    public override Search ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Search();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.DATE_TIME: entity.DateTime = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.URL: entity.Url = reader.ReadAsString(); break;
                case RedmineKeys.TYPE: entity.Type = reader.ReadAsString(); break;
                case RedmineKeys.TITLE: entity.Title = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Search value, bool writeRootName) { }
}

internal sealed class TimeEntryActivityConverter : JsonTextRedmineConverter<TimeEntryActivity>
{
    public override string RootName { get; } = RedmineKeys.TIME_ENTRY_ACTIVITY;

    public override TimeEntryActivity ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new TimeEntryActivity();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString(); switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.IS_DEFAULT: entity.IsDefault = reader.ReadAsBool(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.ACTIVE: entity.IsActive = reader.ReadAsBool(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, TimeEntryActivity entity, bool writeRootName) { }
}

internal sealed class TimeEntryConverter : JsonTextRedmineConverter<TimeEntry>
{
    public override string RootName { get; } = RedmineKeys.TIME_ENTRY;

    public override TimeEntry ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new TimeEntry();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.ACTIVITY: entity.Activity = converter.ReadJson(ref reader); break;
                case RedmineKeys.ACTIVITY_ID: entity.Activity = converter.ReadJson(ref reader); break;
                case RedmineKeys.COMMENTS: entity.Comments = reader.ReadAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                case RedmineKeys.HOURS: entity.Hours = reader.ReadAsDecimal(); break;
                case RedmineKeys.ISSUE: entity.Issue = converter.ReadJson(ref reader); break;
                case RedmineKeys.ISSUE_ID: entity.Issue = converter.ReadJson(ref reader); break;
                case RedmineKeys.PROJECT: entity.Project = converter.ReadJson(ref reader); break;
                case RedmineKeys.PROJECT_ID: entity.Project = converter.ReadJson(ref reader); break;
                case RedmineKeys.SPENT_ON: entity.SpentOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.USER: entity.User = converter.ReadJson(ref reader); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, TimeEntry entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteIdIfNotNull(RedmineKeys.ISSUE_ID, entity.Issue);
            writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, entity.Project);
            writer.WriteIdIfNotNull(RedmineKeys.ACTIVITY_ID, entity.Activity);
            writer.WriteDateOrEmpty(RedmineKeys.SPENT_ON, entity.SpentOn.GetValueOrDefault(DateTime.Now));
            writer.WriteProperty(RedmineKeys.HOURS, entity.Hours);
            writer.WriteProperty(RedmineKeys.COMMENTS, entity.Comments);
            writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields);
        }
    }
}

internal sealed class TrackerConverter : JsonTextRedmineConverter<Tracker>
{
    public override string RootName { get; } = RedmineKeys.TRACKER;

    public override Tracker ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Tracker();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Tracker value, bool writeRootName) { }
}

internal sealed class TrackerCoreFieldConverter : JsonTextRedmineConverter<TrackerCoreField>
{
    public override string RootName { get; } = RedmineKeys.FIELD;

    public override TrackerCoreField ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new TrackerCoreField();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.PERMISSION: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, TrackerCoreField value, bool writeRoot) { }
}

internal sealed class TrackerCustomFieldConverter : JsonTextRedmineConverter<TrackerCustomField>
{
    public override string RootName { get; } = RedmineKeys.TRACKER;

    public override TrackerCustomField ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new TrackerCustomField();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, TrackerCustomField value, bool writeRoot) { }
}

internal sealed class UploadConverter : JsonTextRedmineConverter<Upload>
{
    public override string RootName { get; } = RedmineKeys.UPLOAD;

    public override Upload ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Upload();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsString(); break;
                case RedmineKeys.CONTENT_TYPE: entity.ContentType = reader.ReadAsString(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.FILE_NAME: entity.FileName = reader.ReadAsString(); break;
                case RedmineKeys.TOKEN: entity.Token = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Upload entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.TOKEN, entity.Token);
            writer.WriteProperty(RedmineKeys.CONTENT_TYPE, entity.ContentType);
            writer.WriteProperty(RedmineKeys.FILE_NAME, entity.FileName);
            writer.WriteIfNotDefaultOrNull(RedmineKeys.DESCRIPTION, entity.Description);
        }
    }
}

internal sealed class UserConverter : JsonTextRedmineConverter<User>
{
    public override string RootName { get; } = RedmineKeys.USER;

    public override User ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new User();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.ADMIN: entity.IsAdmin = reader.ReadAsBool(); break;
                case RedmineKeys.API_KEY: entity.ApiKey = reader.ReadAsString(); break;
                case RedmineKeys.AUTH_SOURCE_ID: entity.AuthenticationModeId = reader.ReadAsNullableInt(); break;
                case RedmineKeys.AVATAR_URL: entity.AvatarUrl = reader.ReadAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                case RedmineKeys.LAST_LOGIN_ON: entity.LastLoginOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.LAST_NAME: entity.LastName = reader.ReadAsString(); break;
                case RedmineKeys.LOGIN: entity.Login = reader.ReadAsString(); break;
                case RedmineKeys.FIRST_NAME: entity.FirstName = reader.ReadAsString(); break;
                case RedmineKeys.GROUPS: entity.Groups = reader.ReadAsCollection<UserGroup>(); break;
                case RedmineKeys.MAIL: entity.Email = reader.ReadAsString(); break;
                case RedmineKeys.MAIL_NOTIFICATION: entity.MailNotification = reader.ReadAsString(); break;
                case RedmineKeys.MEMBERSHIPS: entity.Memberships = reader.ReadAsCollection<Membership>(); break;
                case RedmineKeys.MUST_CHANGE_PASSWORD: entity.MustChangePassword = reader.ReadAsBool(); break;
                case RedmineKeys.PASSWORD_CHANGED_ON: entity.PasswordChangedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.STATUS: entity.Status = (UserStatus)reader.ReadAsInt(); break;
                case RedmineKeys.TWO_FA_SCHEME: entity.TwoFactorAuthenticationScheme = reader.ReadAsString(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsNullableDateTime(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, User entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.LOGIN, entity.Login);
            writer.WriteIfNotDefaultOrNull(RedmineKeys.PASSWORD, entity.Password);
            writer.WriteProperty(RedmineKeys.FIRST_NAME, entity.FirstName);
            writer.WriteProperty(RedmineKeys.LAST_NAME, entity.LastName);
            writer.WriteProperty(RedmineKeys.MAIL, entity.Email);
            writer.WriteValueOrEmpty(RedmineKeys.AUTH_SOURCE_ID, entity.AuthenticationModeId);
            writer.WriteIfNotDefaultOrNull(RedmineKeys.MAIL_NOTIFICATION, entity.MailNotification);
            writer.WriteBoolean(RedmineKeys.MUST_CHANGE_PASSWORD, entity.MustChangePassword);
            writer.WriteBoolean(RedmineKeys.GENERATE_PASSWORD, entity.GeneratePassword);
            writer.WriteBoolean(RedmineKeys.SEND_INFORMATION, entity.SendInformation);

            var userStatus = (int)entity.Status;
            if (userStatus != 0)
            {
                writer.WriteProperty(RedmineKeys.STATUS, userStatus.ToInvariantString());
            }

            if (entity.Id > 0)
            {
                writer.WriteBoolean(RedmineKeys.ADMIN, entity.IsAdmin);
            }

            writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields);
        }
    }
}

internal sealed class UserGroupConverter : JsonTextRedmineConverter<UserGroup>
{
    public override string RootName { get; } = RedmineKeys.GROUP;

    public override UserGroup ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new UserGroup();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, UserGroup value, bool writeRootName) { }
}

internal sealed class VersionConverter : JsonTextRedmineConverter<Types_Version>
{
    public override string RootName { get; } = RedmineKeys.VERSION;

    public override Version ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Version();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString(); switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.DUE_DATE: entity.DueDate = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.PROJECT: entity.Project = converter.ReadJson(ref reader); break;
                case RedmineKeys.SHARING: entity.Sharing = EnumHelper.Parse<VersionSharing>(reader.ReadAsString() ?? string.Empty, true); break;
                case RedmineKeys.STATUS: entity.Status = EnumHelper.Parse<VersionStatus>(reader.ReadAsString() ?? string.Empty, true); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.WIKI_PAGE_TITLE: entity.WikiPageTitle = reader.ReadAsString(); break;
                case RedmineKeys.ESTIMATED_HOURS: entity.EstimatedHours = reader.ReadAsNullableFloat(); break;
                case RedmineKeys.SPENT_HOURS: entity.SpentHours = reader.ReadAsNullableFloat(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Version entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.NAME, entity.Name);
            writer.WriteProperty(RedmineKeys.STATUS, entity.Status.ToLowerInvariant());
            writer.WriteProperty(RedmineKeys.SHARING, entity.Sharing.ToLowerInvariant());
            writer.WriteProperty(RedmineKeys.DESCRIPTION, entity.Description);
            writer.WriteDateOrEmpty(RedmineKeys.DUE_DATE, entity.DueDate);
            writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields);
            writer.WriteProperty(RedmineKeys.WIKI_PAGE_TITLE, entity.WikiPageTitle);
        }
    }
}

internal sealed class WatcherConverter : JsonTextRedmineConverter<Watcher>
{
    public override string RootName { get; } = RedmineKeys.USER;

    public override Watcher ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new Watcher();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            switch (propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, Watcher value, bool writeRootName) { }
}

internal sealed class WikiPageConverter : JsonTextRedmineConverter<WikiPage>
{
    public override string RootName { get; } = RedmineKeys.WIKI_PAGE;

    public override WikiPage ReadJson(ref Utf8JsonReader reader)
    {
        var entity = new WikiPage();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString(); switch(propertyName)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.ATTACHMENTS: entity.Attachments = reader.ReadAsCollection<Attachment>(); break;
                case RedmineKeys.AUTHOR: entity.Author = converter.ReadJson(ref reader); break;
                case RedmineKeys.COMMENTS: entity.Comments = reader.ReadAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.TEXT: entity.Text = reader.ReadAsString(); break;
                case RedmineKeys.TITLE: entity.Title = reader.ReadAsString(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsNullableDateTime(); break;
                case RedmineKeys.VERSION: entity.Version = reader.ReadAsInt(); break;
                case RedmineKeys.PARENT: entity.ParentTitle = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(Utf8JsonWriter writer, WikiPage entity, bool writeRootName)
    {
        using (new JsonObject(writer, writeRootName ? RootName : null))
        {
            writer.WriteProperty(RedmineKeys.TEXT, entity.Text);
            writer.WriteIfNotDefaultOrNull(RedmineKeys.COMMENTS, entity.Comments);
            writer.WriteIfNotDefaultOrNull(RedmineKeys.VERSION, entity.Version);
            writer.WriteArray(RedmineKeys.UPLOADS, entity.Uploads, false);
        }
    }
}


