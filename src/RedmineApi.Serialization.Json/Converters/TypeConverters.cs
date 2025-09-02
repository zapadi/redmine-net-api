using System;
using Newtonsoft.Json;
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Serialization.Json.Extensions;
using Padi.RedmineApi.Types;
using File = Padi.RedmineApi.Types.File;
using Types_File = Padi.RedmineApi.Types.File;
using Types_Version = Padi.RedmineApi.Types.Version;
using Version = Padi.RedmineApi.Types.Version;

namespace Padi.RedmineApi.Serialization.Json.Converters;

public sealed class AttachmentConverter : JsonRedmineConverter<Attachment>
{
    public override string RootName { get; } = RedmineKeys.ATTACHMENT;

    public override Attachment ReadJson(JsonReader reader)
    {
        var entity = new Attachment();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.AUTHOR: entity.Author = converter.ReadJson(reader); break;
                case RedmineKeys.CONTENT_TYPE: entity.ContentType = reader.ReadAsString(); break;
                case RedmineKeys.CONTENT_URL: entity.ContentUrl = reader.ReadAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.FILE_NAME: entity.FileName = reader.ReadAsString(); break;
                case RedmineKeys.FILE_SIZE: entity.FileSize = reader.ReadAsInt(); break;
                case RedmineKeys.THUMBNAIL_URL: entity.ThumbnailUrl = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, Attachment entity)
    {
        using (new JsonObject(writer, RootName))
        {
            writer.WriteProperty(RedmineKeys.FILE_NAME, entity.FileName);
            writer.WriteProperty(RedmineKeys.DESCRIPTION, entity.Description);
        }
    }
}

public sealed class ChangeSetConverter : JsonRedmineConverter<ChangeSet>
{
    public override string RootName { get; } = RedmineKeys.CHANGE_SET;

    public override ChangeSet ReadJson(JsonReader reader)
    {
        var entity = new ChangeSet();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.COMMENTS: entity.Comments = reader.ReadAsString(); break;
                case RedmineKeys.COMMITTED_ON: entity.CommittedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.REVISION: entity.Revision = reader.ReadAsString(); break;
                case RedmineKeys.USER: entity.User = converter.ReadJson(reader); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, ChangeSet value) { }
}

public sealed class CustomFieldConverter : JsonRedmineConverter<CustomField>
{
    public override string RootName { get; } = RedmineKeys.CUSTOM_FIELD;

    public override CustomField ReadJson(JsonReader reader)
    {
        var entity = new CustomField();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.CUSTOMIZED_TYPE: entity.CustomizedType = reader.ReadAsString(); break;
                case RedmineKeys.DEFAULT_VALUE: entity.DefaultValue = reader.ReadAsString(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.FIELD_FORMAT: entity.FieldFormat = reader.ReadAsString(); break;
                case RedmineKeys.IS_FILTER: entity.IsFilter = reader.ReadAsBool(); break;
                case RedmineKeys.IS_REQUIRED: entity.IsRequired = reader.ReadAsBool(); break;
                case RedmineKeys.MAX_LENGTH: entity.MaxLength = reader.ReadAsInt32(); break;
                case RedmineKeys.MIN_LENGTH: entity.MinLength = reader.ReadAsInt32(); break;
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

    public override void WriteJson(JsonWriter writer, CustomField value) { }
}

public sealed class CustomFieldPossibleValueConverter : JsonRedmineConverter<CustomFieldPossibleValue>
{
    public override string RootName { get; } = RedmineKeys.POSSIBLE_VALUE;

    public override CustomFieldPossibleValue ReadJson(JsonReader reader)
    {
        var entity = new CustomFieldPossibleValue();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.LABEL: entity.Label = reader.ReadAsString(); break;
                case RedmineKeys.VALUE: entity.Value = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, CustomFieldPossibleValue value) { }
}

public sealed class CustomFieldRoleConverter : JsonRedmineConverter<CustomFieldRole>
{
    public override string RootName { get; } = RedmineKeys.ROLE;

    public override CustomFieldRole ReadJson(JsonReader reader)
    {
        var entity = new CustomFieldRole();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType == JsonToken.PropertyName)
            {
                switch (reader.Value)
                {
                    case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                    case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, CustomFieldRole value) { }
}

public class CustomFieldValueConverter : JsonRedmineConverter<CustomFieldValue>
{
    public override string RootName { get; } = RedmineKeys.VALUE;

    public override CustomFieldValue? ReadJson(JsonReader reader)
    {
        string? info = null;

        if (reader.TokenType == JsonToken.PropertyName)
        {
            return null;
        }

        if (reader.TokenType == JsonToken.String)
        {
            info = reader.Value as string;
        }

        return new CustomFieldValue(){ Info = info};
    }

    public override void WriteJson(JsonWriter writer, CustomFieldValue value) { }
}

public sealed class DetailConverter : JsonRedmineConverter<Detail>
{
    public override string RootName { get; } = RedmineKeys.DETAIL;

    public override Detail ReadJson(JsonReader reader)
    {
        var entity = new Detail();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
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

    public override void WriteJson(JsonWriter writer, Detail value) { }
}

public sealed class DocumentCategoryConverter : JsonRedmineConverter<DocumentCategory>
{
    public override string RootName { get; } = RedmineKeys.DOCUMENT_CATEGORY;

    public override DocumentCategory ReadJson(JsonReader reader)
    {
        var entity = new DocumentCategory();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
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

    public override void WriteJson(JsonWriter writer, DocumentCategory value) { }
}

public class ErrorConverter : JsonRedmineConverter<Error>
{
    public override string RootName { get; } = RedmineKeys.ERROR;

    public override Error ReadJson(JsonReader reader)
    {
        string? info = null;

        if (reader.TokenType == JsonToken.PropertyName)
        {
            reader.Read();
        }

        if (reader.TokenType == JsonToken.String)
        {
            info = (string?)reader.Value;
        }

        return new Error { Info = info };
    }

    public override void WriteJson(JsonWriter writer, Error value) { }
}

public sealed class FileConverter : JsonRedmineConverter<Types_File>
{
    public override string RootName { get; } = RedmineKeys.FILE;

    public override File ReadJson(JsonReader reader)
    {
        var entity = new File();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt32().GetValueOrDefault(); break;
                case RedmineKeys.AUTHOR: entity.Author = converter.ReadJson(reader); break;
                case RedmineKeys.CONTENT_TYPE: entity.ContentType = reader.ReadAsString(); break;
                case RedmineKeys.CONTENT_URL: entity.ContentUrl = reader.ReadAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.DIGEST: entity.Digest = reader.ReadAsString(); break;
                case RedmineKeys.DOWNLOADS: entity.Downloads = reader.ReadAsInt32().GetValueOrDefault(); break;
                case RedmineKeys.FILE_NAME: entity.Filename = reader.ReadAsString(); break;
                case RedmineKeys.FILE_SIZE: entity.FileSize = reader.ReadAsInt32().GetValueOrDefault(); break;
                case RedmineKeys.TOKEN: entity.Token = reader.ReadAsString(); break;
                case RedmineKeys.VERSION: entity.Version = converter.ReadJson(reader); break;
                case RedmineKeys.VERSION_ID: entity.Version = IdentifiableName.Create<IdentifiableName>(reader.ReadAsInt32().GetValueOrDefault()); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, File entity)
    {
        using (new JsonObject(writer, RootName))
        {
            writer.WriteProperty(RedmineKeys.TOKEN, entity.Token);
            writer.WriteIdIfNotNull(RedmineKeys.VERSION_ID, entity.Version);
            writer.WriteProperty(RedmineKeys.FILE_NAME, entity.Filename);
            writer.WriteProperty(RedmineKeys.DESCRIPTION, entity.Description);
        }
    }
}

public sealed class GroupConverter : JsonRedmineConverter<Group>
{
    public override string RootName { get; } = RedmineKeys.GROUP;

    public override Group ReadJson(JsonReader reader)
    {
        var entity = new Group();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
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

    public override void WriteJson(JsonWriter writer, Group entity)
    {
        using (new JsonObject(writer, RootName))
        {
            writer.WriteProperty(RedmineKeys.NAME, entity.Name);
            writer.WriteRepeatableElement(RedmineKeys.USER_IDS, entity.Users);
        }
    }
}

public class GroupUserConverter : JsonRedmineConverter<GroupUser>
{
    public override string RootName { get; } = RedmineKeys.USER;

    public override GroupUser ReadJson(JsonReader reader)
    {
        var entity = new GroupUser();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType == JsonToken.PropertyName)
            {
                switch (reader.Value)
                {
                    case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                    case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, GroupUser value) { }
}

public class IdentifiableNameConverter : JsonRedmineConverter<IdentifiableName>
{
    public override string RootName { get; }

    public override IdentifiableName ReadJson(JsonReader reader)
    {
        var entity = new IdentifiableName();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType == JsonToken.PropertyName)
            {
                switch (reader.Value)
                {
                    case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                    case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, IdentifiableName entity)
    {
        // writer.WriteIdIfNotNull(RedmineKeys.ID, entity);
        // if (!entity.Name.IsNullOrWhiteSpace())
        // {
        //     writer.WriteProperty(RedmineKeys.NAME, entity.Name);
        // }
    }
}

public class IssueAllowedStatusConverter : JsonRedmineConverter<IssueAllowedStatus>
{
    public override string RootName { get; } = RedmineKeys.STATUS;

    public override IssueAllowedStatus ReadJson(JsonReader reader)
    {
        var entity = new IssueAllowedStatus();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType == JsonToken.PropertyName)
            {
                switch (reader.Value)
                {
                    case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                    case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                    case RedmineKeys.IS_CLOSED: entity.IsClosed = reader.ReadAsBoolean(); break;
                    default: reader.Read(); break;
                }
            }
        }
        return entity;
    }

    public override void WriteJson(JsonWriter writer, IssueAllowedStatus value) { }
}

public class IssueCategoryConverter : JsonRedmineConverter<IssueCategory>
{
    public override string RootName { get; } = RedmineKeys.ISSUE_CATEGORY;

    public override IssueCategory ReadJson(JsonReader reader)
    {
        var entity = new IssueCategory();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.ASSIGNED_TO: entity.AssignTo = converter.ReadJson(reader); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.PROJECT: entity.Project = converter.ReadJson(reader); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, IssueCategory entity)
    {
        using (new JsonObject(writer, RootName))
        {
            writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, entity.Project);
            writer.WriteProperty(RedmineKeys.NAME, entity.Name);
            writer.WriteIdIfNotNull(RedmineKeys.ASSIGNED_TO_ID, entity.AssignTo);
        }
    }
}

public sealed class IssueChildConverter : JsonRedmineConverter<IssueChild>
{
    public override string RootName { get; } = RedmineKeys.ISSUE;

    public override IssueChild ReadJson(JsonReader reader)
    {
        var entity = new IssueChild();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.SUBJECT: entity.Subject = reader.ReadAsString(); break;
                case RedmineKeys.TRACKER: entity.Tracker = converter.ReadJson(reader); break;
                default: reader.Read(); break;
            }
        }
        return entity;
    }

    public override void WriteJson(JsonWriter writer, IssueChild value) { }
}

public class IssueConverter : JsonRedmineConverter<Issue>
{
    public override string RootName { get; } = RedmineKeys.ISSUE;

    public override Issue ReadJson(JsonReader reader)
    {
        var entity = new Issue();

        var identifiableNameConverter = JsonConverterRegistry.GetConverter<IdentifiableName>();
        var issueStatusConverter = JsonConverterRegistry.GetConverter<IssueStatus>();

       while (reader.Read())
       {
           if (reader.TokenType == JsonToken.EndObject)
           {
               return entity;
           }

           if (reader.TokenType != JsonToken.PropertyName)
           {
               continue;
           }

           switch (reader.Value)
           {
               case RedmineKeys.ID: entity.Id = reader.ReadAsInt32().GetValueOrDefault(); break;
               case RedmineKeys.ALLOWED_STATUSES: entity.AllowedStatuses = reader.ReadAsCollection<IssueAllowedStatus>(); break;
               case RedmineKeys.ASSIGNED_TO: entity.AssignedTo = identifiableNameConverter.ReadJson(reader); break;
               case RedmineKeys.ATTACHMENTS: entity.Attachments = reader.ReadAsCollection<Attachment>(); break;
               case RedmineKeys.AUTHOR: entity.Author = identifiableNameConverter.ReadJson(reader); break;
               case RedmineKeys.CATEGORY: entity.Category = identifiableNameConverter.ReadJson(reader); break;
               case RedmineKeys.CHANGE_SETS: entity.ChangeSets = reader.ReadAsCollection<ChangeSet>(); break;
               case RedmineKeys.CHILDREN: entity.Children = reader.ReadAsCollection<IssueChild>(); break;
               case RedmineKeys.CLOSED_ON: entity.ClosedOn = reader.ReadAsDateTime(); break;
               case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsDateTime(); break;
               case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
               case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
               case RedmineKeys.DONE_RATIO: entity.DoneRatio = (float?)reader.ReadAsDouble(); break;
               case RedmineKeys.DUE_DATE: entity.DueDate = reader.ReadAsDateTime(); break;
               case RedmineKeys.ESTIMATED_HOURS: entity.EstimatedHours = (float?)reader.ReadAsDouble(); break;
               case RedmineKeys.FIXED_VERSION: entity.FixedVersion = identifiableNameConverter.ReadJson(reader); break;
               case RedmineKeys.IS_PRIVATE: entity.IsPrivate = reader.ReadAsBoolean().GetValueOrDefault(); break;
               case RedmineKeys.JOURNALS: entity.Journals = reader.ReadAsCollection<Journal>(); break;
               case RedmineKeys.NOTES: entity.Notes = reader.ReadAsString(); break;
               case RedmineKeys.PARENT: entity.ParentIssue = identifiableNameConverter.ReadJson(reader); break;
               case RedmineKeys.PRIORITY: entity.Priority = identifiableNameConverter.ReadJson(reader); break;
               case RedmineKeys.PRIVATE_NOTES: entity.PrivateNotes = reader.ReadAsBoolean().GetValueOrDefault(); break;
               case RedmineKeys.PROJECT: entity.Project = identifiableNameConverter.ReadJson(reader); break;
               case RedmineKeys.RELATIONS: entity.Relations = reader.ReadAsCollection<IssueRelation>(); break;
               case RedmineKeys.SPENT_HOURS: entity.SpentHours = (float?)reader.ReadAsDouble(); break;
               case RedmineKeys.START_DATE: entity.StartDate = reader.ReadAsDateTime(); break;
               case RedmineKeys.STATUS: entity.Status = issueStatusConverter.ReadJson(reader); break;
               case RedmineKeys.SUBJECT: entity.Subject = reader.ReadAsString(); break;
               case RedmineKeys.TOTAL_ESTIMATED_HOURS: entity.TotalEstimatedHours = (float?)reader.ReadAsDouble(); break;
               case RedmineKeys.TOTAL_SPENT_HOURS: entity.TotalSpentHours = (float?)reader.ReadAsDouble(); break;
               case RedmineKeys.TRACKER: entity.Tracker = identifiableNameConverter.ReadJson(reader); break;
               case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsDateTime(); break;
               case RedmineKeys.WATCHERS: entity.Watchers = reader.ReadAsCollection<Watcher>(); break;
               default: reader.Read(); break;
           }
       }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, Issue entity)
    {
         using (new JsonObject(writer, RootName))
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

             writer.WriteArray(RedmineKeys.UPLOADS, entity.Uploads);
             writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields);

             writer.WriteRepeatableElement(RedmineKeys.WATCHER_USER_IDS, entity.Watchers);
         }
    }
}

public class IssueCustomFieldConverter : JsonRedmineConverter<IssueCustomField>
{
    public override string RootName { get; } = RedmineKeys.CUSTOM_FIELD;

    public override IssueCustomField ReadJson(JsonReader reader)
    {
        var entity = new IssueCustomField();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.MULTIPLE: entity.Multiple = reader.ReadAsBool(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.VALUE:
                    reader.Read();
                    switch (reader.TokenType)
                    {
                        case JsonToken.Null: break;
                        case JsonToken.StartArray:
                            entity.Values = reader.ReadAsCollection<CustomFieldValue>();
                            break;
                        default:
                            entity.Values = [new CustomFieldValue { Info = reader.Value as string }];
                            break;
                    }
                    break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, IssueCustomField? entity)
    {
        if (entity == null || entity.Values == null)
        {
            return;
        }

        var itemsCount = entity.Values.Count;
        entity.Multiple = itemsCount > 1;

        using (new JsonObject(writer, RootName))
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
                    writer.WriteValue(cfv.Info);
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

public sealed class IssuePriorityConverter : JsonRedmineConverter<IssuePriority>
{
    public override string RootName { get; } = RedmineKeys.ISSUE_PRIORITY;

    public override IssuePriority ReadJson(JsonReader reader)
    {
        var entity = new IssuePriority();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
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

    public override void WriteJson(JsonWriter writer, IssuePriority value) { }
}

public class IssueRelationConverter : JsonRedmineConverter<IssueRelation>
{
    public override string RootName { get; } = RedmineKeys.RELATION;

    public override IssueRelation ReadJson(JsonReader reader)
    {
        var entity = new IssueRelation();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.DELAY: entity.Delay = reader.ReadAsInt32(); break;
                case RedmineKeys.ISSUE_ID: entity.IssueId = reader.ReadAsInt(); break;
                case RedmineKeys.ISSUE_TO_ID: entity.IssueToId = reader.ReadAsInt(); break;
                case RedmineKeys.RELATION_TYPE: entity.Type = ReadIssueRelationType(reader.ReadAsString()); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, IssueRelation entity)
    {
        if (entity.Type == IssueRelationType.Undefined)
        {
            throw new RedmineException(RedmineApiErrorCode.SerializationError, $"The value `{nameof(IssueRelationType)}.`{nameof(IssueRelationType.Undefined)}` is not allowed to create relations!");
        }

        using (new JsonObject(writer, RootName))
        {
            writer.WriteProperty(RedmineKeys.ISSUE_TO_ID, entity.IssueToId);
            writer.WriteProperty(RedmineKeys.RELATION_TYPE, entity.Type.ToLowerInvariant());

            if (entity.Type == IssueRelationType.Precedes || entity.Type == IssueRelationType.Follows)
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

public class IssueStatusConverter : JsonRedmineConverter<IssueStatus>
{
    public override string RootName { get; } = RedmineKeys.ISSUE_STATUS;

    public override IssueStatus ReadJson(JsonReader reader)
    {
        var entity = new IssueStatus();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
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

    public override void WriteJson(JsonWriter writer, IssueStatus value) { }
}

public class JournalConverter : JsonRedmineConverter<Journal>
{
    public override string RootName { get; } = RedmineKeys.JOURNAL;

    public override Journal ReadJson(JsonReader reader)
    {
        var entity = new Journal();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.DETAILS: entity.Details = reader.ReadAsCollection<Detail>(); break;
                case RedmineKeys.NOTES: entity.Notes = reader.ReadAsString(); break;
                case RedmineKeys.PRIVATE_NOTES: entity.PrivateNotes = reader.ReadAsBool(); break;
                case RedmineKeys.USER: entity.User = converter.ReadJson(reader); break;
                case RedmineKeys.UPDATED_BY: entity.UpdatedBy = converter.ReadJson(reader); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, Journal entity)
    {
        using (new JsonObject(writer, RootName))
        {
            writer.WriteProperty(RedmineKeys.NOTES, entity.Notes);
        }
    }
}

public class MembershipConverter : JsonRedmineConverter<Membership>
{
    public override string RootName { get; } = RedmineKeys.MEMBERSHIP;

    public override Membership ReadJson(JsonReader reader)
    {
        var entity = new Membership();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.GROUP: entity.Project = converter.ReadJson(reader); break;
                case RedmineKeys.PROJECT: entity.Project = converter.ReadJson(reader); break;
                case RedmineKeys.USER: entity.Project = converter.ReadJson(reader); break;
                case RedmineKeys.ROLES: entity.Roles = reader.ReadAsCollection<MembershipRole>(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, Membership value) { }
}

public sealed class MembershipRoleConverter : JsonRedmineConverter<MembershipRole>
{
    public override string RootName { get; } = RedmineKeys.POSSIBLE_VALUE;

    public override MembershipRole? ReadJson(JsonReader reader)
    {
        var entity = new MembershipRole();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.INHERITED: entity.Inherited = reader.ReadAsBool(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }
        return null;
    }

    public override void WriteJson(JsonWriter writer, MembershipRole entity)
    {
        using (new JsonObject(writer, RootName))
        {
            writer.WriteProperty(RedmineKeys.ID, entity.Id.ToInvariantString());
        }
    }
}

public class MyAccountConverter : JsonRedmineConverter<MyAccount>
{
    public override string RootName { get; } = RedmineKeys.USER;

    public override MyAccount ReadJson(JsonReader reader)
    {
        var entity = new MyAccount();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.ADMIN: entity.IsAdmin = reader.ReadAsBool(); break;
                case RedmineKeys.API_KEY: entity.ApiKey = reader.ReadAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadAsCollection<MyAccountCustomField>(); break;
                case RedmineKeys.FIRST_NAME: entity.FirstName = reader.ReadAsString(); break;
                case RedmineKeys.LAST_LOGIN_ON: entity.LastLoginOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.LAST_NAME: entity.LastName = reader.ReadAsString(); break;
                case RedmineKeys.LOGIN: entity.Login = reader.ReadAsString(); break;
                case RedmineKeys.MAIL: entity.Email = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, MyAccount entity)
    {
        using (new JsonObject(writer, RootName))
        {
            writer.WriteProperty(RedmineKeys.FIRST_NAME, entity.FirstName);
            writer.WriteProperty(RedmineKeys.LAST_NAME, entity.LastName);
            writer.WriteProperty(RedmineKeys.MAIL, entity.Email);
        }
    }
}

public class MyAccountCustomFieldConverter : JsonRedmineConverter<MyAccountCustomField>
{
    public override string RootName { get; } = RedmineKeys.CUSTOM_FIELD;

    public override MyAccountCustomField ReadJson(JsonReader reader)
    {
        var entity = new MyAccountCustomField();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType == JsonToken.PropertyName)
            {
                switch (reader.Value)
                {
                    case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                    case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                    case RedmineKeys.VALUE: entity.Value = reader.ReadAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, MyAccountCustomField entity) { }
}

public class NewsConverter : JsonRedmineConverter<News>
{
    public override string RootName { get; } = RedmineKeys.NEWS;

    public override News ReadJson(JsonReader reader)
    {
        var entity = new News();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.AUTHOR: entity.Author = converter.ReadJson(reader); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.PROJECT: entity.Project = converter.ReadJson(reader); break;
                case RedmineKeys.SUMMARY: entity.Summary = reader.ReadAsString(); break;
                case RedmineKeys.TITLE: entity.Title = reader.ReadAsString(); break;
                case RedmineKeys.ATTACHMENTS: entity.Attachments = reader.ReadAsCollection<Attachment>(); break;
                case RedmineKeys.COMMENTS: entity.Comments = reader.ReadAsCollection<NewsComment>(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, News entity)
    {
        using (new JsonObject(writer, RootName))
        {
            writer.WriteProperty(RedmineKeys.TITLE, entity.Title);
            writer.WriteProperty(RedmineKeys.SUMMARY, entity.Summary);
            writer.WriteProperty(RedmineKeys.DESCRIPTION, entity.Description);
            if (entity.Uploads != null)
            {
                writer.WriteArray(RedmineKeys.UPLOADS, entity.Uploads);
            }
        }
    }
}

public class NewsCommentConverter : JsonRedmineConverter<NewsComment>
{
    public override string RootName { get; } = RedmineKeys.COMMENT;

    public override NewsComment ReadJson(JsonReader reader)
    {
        var entity = new NewsComment();

        var identifiableNameConverter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt32().GetValueOrDefault(); break;
                case RedmineKeys.AUTHOR: entity.Author = identifiableNameConverter.ReadJson(reader); break;
                case RedmineKeys.CONTENT: entity.Content = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, NewsComment value) { }
}

public sealed class PermissionConverter : JsonRedmineConverter<Permission>
{
    public override string RootName { get; } = RedmineKeys.POSSIBLE_VALUE;

    public override Permission ReadJson(JsonReader reader)
    {
        var entity = new Permission();

        if (reader.TokenType == JsonToken.String)
        {
            entity.Info = reader.Value as string;
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, Permission value) { }
}

public sealed class ProjectConverter : JsonRedmineConverter<Project>
{
    public override string RootName { get; } = RedmineKeys.PROJECT;

    public override Project ReadJson(JsonReader reader)
    {
        var entity = new Project();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

       while (reader.Read())
       {
           if (reader.TokenType == JsonToken.EndObject)
           {
               return entity;
           }

           if (reader.TokenType != JsonToken.PropertyName)
           {
               continue;
           }

           switch (reader.Value)
           {
               case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
               case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsDateTime(); break;
               case RedmineKeys.CUSTOM_FIELDS: entity.IssueCustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
               case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
               case RedmineKeys.ENABLED_MODULES: entity.EnabledModules = reader.ReadAsCollection<ProjectEnabledModule>(); break;
               case RedmineKeys.HOMEPAGE: entity.HomePage = reader.ReadAsString(); break;
               case RedmineKeys.IDENTIFIER: entity.Identifier = reader.ReadAsString(); break;
               case RedmineKeys.INHERIT_MEMBERS: entity.InheritMembers = reader.ReadAsBool(); break;
               case RedmineKeys.IS_PUBLIC: entity.IsPublic = reader.ReadAsBool(); break;
               case RedmineKeys.ISSUE_CATEGORIES: entity.IssueCategories = reader.ReadAsCollection<ProjectIssueCategory>(); break;
               case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
               case RedmineKeys.PARENT: entity.Parent = converter.ReadJson(reader); break;
               case RedmineKeys.STATUS: entity.Status = (ProjectStatus)reader.ReadAsInt(); break;
               case RedmineKeys.TIME_ENTRY_ACTIVITIES: entity.TimeEntryActivities = reader.ReadAsCollection<ProjectTimeEntryActivity>(); break;
               case RedmineKeys.TRACKERS: entity.Trackers = reader.ReadAsCollection<ProjectTracker>(); break;
               case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsDateTime(); break;
               case RedmineKeys.DEFAULT_ASSIGNEE: entity.DefaultAssignee = converter.ReadJson(reader); break;
               case RedmineKeys.DEFAULT_VERSION: entity.DefaultVersion = converter.ReadJson(reader); break;
               default: reader.Read(); break;
           }
       }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, Project entity)
    {
        using (new JsonObject(writer, RootName))
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

            writer.WriteRepeatableElement(RedmineKeys.TRACKER_IDS, entity.Trackers);
            writer.WriteRepeatableElement(RedmineKeys.ENABLED_MODULE_NAMES, entity.EnabledModules);
            writer.WriteRepeatableElement(RedmineKeys.ISSUE_CUSTOM_FIELD_IDS, entity.IssueCustomFields);
            if (entity.Id == 0)
            {
                writer.WriteArray(RedmineKeys.CUSTOM_FIELD_VALUES, entity.CustomFieldValues);
            }
        }
    }
}

public sealed class ProjectEnabledModuleConverter : JsonRedmineConverter<ProjectEnabledModule>
{
    public override string RootName { get; } = RedmineKeys.ENABLED_MODULE;

    public override ProjectEnabledModule ReadJson(JsonReader reader)
    {
        var entity = new ProjectEnabledModule();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, ProjectEnabledModule value) { }
}

public class ProjectIssueCategoryConverter : JsonRedmineConverter<ProjectIssueCategory>
{
    public override string RootName { get; } = RedmineKeys.ISSUE_CATEGORY;

    public override ProjectIssueCategory ReadJson(JsonReader reader)
    {
        var entity = new ProjectIssueCategory();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, ProjectIssueCategory value) { }
}

public class ProjectMembershipConverter : JsonRedmineConverter<ProjectMembership>
{
    public override string RootName { get; } = RedmineKeys.MEMBERSHIP;

    public override ProjectMembership ReadJson(JsonReader reader)
    {
        var entity = new ProjectMembership();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.GROUP: entity.Group = converter.ReadJson(reader); break;
                case RedmineKeys.PROJECT: entity.Project = converter.ReadJson(reader); break;
                case RedmineKeys.ROLES: entity.Roles = reader.ReadAsCollection<MembershipRole>(); break;
                case RedmineKeys.USER: entity.User = converter.ReadJson(reader); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, ProjectMembership entity)
    {
        using (new JsonObject(writer, RootName))
        {
            if (entity.Id <= 0)
            {
                writer.WriteIdIfNotNull(RedmineKeys.USER_ID, entity.User);
            }

            writer.WriteArray(RedmineKeys.ROLE_IDS, entity.Roles);
        }
    }

    private static string GetId(MembershipRole membershipRole)
    {
        return membershipRole.Id.ToInvariantString();
    }
}

public class ProjectTimeEntryActivityConverter : JsonRedmineConverter<ProjectTimeEntryActivity>
{
    public override string RootName { get; } = RedmineKeys.TIME_ENTRY_ACTIVITY;

    public override ProjectTimeEntryActivity ReadJson(JsonReader reader)
    {
        var entity = new ProjectTimeEntryActivity();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, ProjectTimeEntryActivity value) { }
}

public class ProjectTrackerConverter : JsonRedmineConverter<ProjectTracker>
{
    public override string RootName { get; } = RedmineKeys.TRACKER;

    public override ProjectTracker ReadJson(JsonReader reader)
    {
        var entity = new ProjectTracker();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, ProjectTracker value) { }
}

public class QueryConverter : JsonRedmineConverter<Query>
{
    public override string RootName { get; } = RedmineKeys.QUERY;

    public override Query ReadJson(JsonReader reader)
    {
        var entity = new Query();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.IS_PUBLIC: entity.IsPublic = reader.ReadAsBool(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.PROJECT_ID: entity.ProjectId = reader.ReadAsInt32(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, Query value) { }
}

public class RoleConverter : JsonRedmineConverter<Role>
{
    public override string RootName { get; } = RedmineKeys.ROLE;

    public override Role ReadJson(JsonReader reader)
    {
        var entity = new Role();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.ASSIGNABLE: entity.IsAssignable = reader.ReadAsBoolean(); break;
                case RedmineKeys.ISSUES_VISIBILITY: entity.IssuesVisibility = reader.ReadAsString(); break;
                case RedmineKeys.TIME_ENTRIES_VISIBILITY: entity.TimeEntriesVisibility = reader.ReadAsString(); break;
                case RedmineKeys.USERS_VISIBILITY: entity.UsersVisibility = reader.ReadAsString(); break;
                case RedmineKeys.PERMISSIONS: entity.Permissions = reader.ReadAsCollection<Permission>(); break;
                default: reader.Read(); break;
            }
        }
        return entity;
    }

    public override void WriteJson(JsonWriter writer, Role value) { }
}

public class SearchConverter : JsonRedmineConverter<Search>
{
    public override string RootName { get; } = RedmineKeys.RESULT;

    public override Search ReadJson(JsonReader reader)
    {
        var entity = new Search();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.DATE_TIME: entity.DateTime = reader.ReadAsDateTime(); break;
                case RedmineKeys.URL: entity.Url = reader.ReadAsString(); break;
                case RedmineKeys.TYPE: entity.Type = reader.ReadAsString(); break;
                case RedmineKeys.TITLE: entity.Title = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, Search value) {}
}

public class TimeEntryConverter : JsonRedmineConverter<TimeEntry>
{
    public override string RootName { get; } = RedmineKeys.TIME_ENTRY;

    public override TimeEntry ReadJson(JsonReader reader)
    {
        var entity = new TimeEntry();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.ACTIVITY: entity.Activity = converter.ReadJson(reader); break;
                case RedmineKeys.ACTIVITY_ID: entity.Activity = converter.ReadJson(reader); break;
                case RedmineKeys.COMMENTS: entity.Comments = reader.ReadAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                case RedmineKeys.HOURS: entity.Hours = reader.ReadAsDecimal().GetValueOrDefault(); break;
                case RedmineKeys.ISSUE: entity.Issue = converter.ReadJson(reader); break;
                case RedmineKeys.ISSUE_ID: entity.Issue = converter.ReadJson(reader); break;
                case RedmineKeys.PROJECT: entity.Project = converter.ReadJson(reader); break;
                case RedmineKeys.PROJECT_ID: entity.Project = converter.ReadJson(reader); break;
                case RedmineKeys.SPENT_ON: entity.SpentOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.USER: entity.User = converter.ReadJson(reader); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, TimeEntry entity)
    {
        using (new JsonObject(writer, RootName))
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

public class TimeEntryActivityConverter : JsonRedmineConverter<TimeEntryActivity>
{
    public override string RootName { get; } = RedmineKeys.TIME_ENTRY_ACTIVITY;

    public override TimeEntryActivity ReadJson(JsonReader reader)
    {
        var entity = new TimeEntryActivity();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
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

    public override void WriteJson(JsonWriter writer, TimeEntryActivity entity) { }
}

public class TrackerConverter : JsonRedmineConverter<Tracker>
{
    public override string RootName { get; } = RedmineKeys.TRACKER;

    public override Tracker ReadJson(JsonReader reader)
    {
        var entity = new Tracker();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType == JsonToken.PropertyName)
            {
                switch (reader.Value)
                {
                    case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                    case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, Tracker value) { }
}

public class TrackerCoreFieldConverter : JsonRedmineConverter<TrackerCoreField>
{
    public override string RootName { get; } = RedmineKeys.FIELD;

    public override TrackerCoreField ReadJson(JsonReader reader)
    {
        var entity = new TrackerCoreField();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.PERMISSION: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, TrackerCoreField value) { }
}

public class TrackerCustomFieldConverter : JsonRedmineConverter<TrackerCustomField>
{
    public override string RootName { get; } = RedmineKeys.TRACKER;

    public override TrackerCustomField ReadJson(JsonReader reader)
    {
        var entity = new TrackerCustomField();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, TrackerCustomField value) { }
}

public class UploadConverter : JsonRedmineConverter<Upload>
{
    public override string RootName { get; } = RedmineKeys.UPLOAD;

    public override Upload ReadJson(JsonReader reader)
    {
        var entity = new Upload();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
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

    public override void WriteJson(JsonWriter writer, Upload entity)
    {
        using (new JsonObject(writer, RootName))
        {
            writer.WriteProperty(RedmineKeys.TOKEN, entity.Token);
            writer.WriteProperty(RedmineKeys.CONTENT_TYPE, entity.ContentType);
            writer.WriteProperty(RedmineKeys.FILE_NAME, entity.FileName);
            writer.WriteProperty(RedmineKeys.DESCRIPTION, entity.Description);
        }
    }
}

public class UserConverter : JsonRedmineConverter<User>
{
    public override string RootName { get; } = RedmineKeys.USER;

    public override User ReadJson(JsonReader reader)
    {
        var entity = new User();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.ADMIN: entity.IsAdmin = reader.ReadAsBool(); break;
                case RedmineKeys.API_KEY: entity.ApiKey = reader.ReadAsString(); break;
                case RedmineKeys.AUTH_SOURCE_ID: entity.AuthenticationModeId = reader.ReadAsInt32(); break;
                case RedmineKeys.AVATAR_URL: entity.AvatarUrl = reader.ReadAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                case RedmineKeys.LAST_LOGIN_ON: entity.LastLoginOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.LAST_NAME: entity.LastName = reader.ReadAsString(); break;
                case RedmineKeys.LOGIN: entity.Login = reader.ReadAsString(); break;
                case RedmineKeys.FIRST_NAME: entity.FirstName = reader.ReadAsString(); break;
                case RedmineKeys.GROUPS: entity.Groups = reader.ReadAsCollection<UserGroup>(); break;
                case RedmineKeys.MAIL: entity.Email = reader.ReadAsString(); break;
                case RedmineKeys.MAIL_NOTIFICATION: entity.MailNotification = reader.ReadAsString(); break;
                case RedmineKeys.MEMBERSHIPS: entity.Memberships = reader.ReadAsCollection<Membership>(); break;
                case RedmineKeys.MUST_CHANGE_PASSWORD: entity.MustChangePassword = reader.ReadAsBool(); break;
                case RedmineKeys.PASSWORD_CHANGED_ON: entity.PasswordChangedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.STATUS: entity.Status = (UserStatus)reader.ReadAsInt(); break;
                case RedmineKeys.TWO_FA_SCHEME: entity.TwoFactorAuthenticationScheme = reader.ReadAsString(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsDateTime(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, User entity)
    {
        using (new JsonObject(writer, RootName))
        {
            writer.WriteProperty(RedmineKeys.LOGIN, entity.Login);

            if (!string.IsNullOrEmpty(entity.Password))
            {
                writer.WriteProperty(RedmineKeys.PASSWORD, entity.Password);
            }

            writer.WriteProperty(RedmineKeys.FIRST_NAME, entity.FirstName);
            writer.WriteProperty(RedmineKeys.LAST_NAME, entity.LastName);
            writer.WriteProperty(RedmineKeys.MAIL, entity.Email);

            if(entity.AuthenticationModeId.HasValue)
            {
                writer.WriteValueOrEmpty(RedmineKeys.AUTH_SOURCE_ID, entity.AuthenticationModeId);
            }

            if(!entity.MailNotification.IsNullOrWhiteSpace())
            {
                writer.WriteProperty(RedmineKeys.MAIL_NOTIFICATION, entity.MailNotification);
            }

            writer.WriteBoolean(RedmineKeys.MUST_CHANGE_PASSWORD, entity.MustChangePassword);
            writer.WriteBoolean(RedmineKeys.GENERATE_PASSWORD, entity.GeneratePassword);
            writer.WriteBoolean(RedmineKeys.SEND_INFORMATION, entity.SendInformation);

            writer.WriteProperty(RedmineKeys.STATUS, ((int)entity.Status).ToInvariantString());

            writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields);
        }
    }
}

public class UserGroupConverter : JsonRedmineConverter<UserGroup>
{
    public override string RootName { get; } = RedmineKeys.GROUP;

    public override UserGroup ReadJson(JsonReader reader)
    {
        var entity = new UserGroup();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType == JsonToken.PropertyName)
            {
                switch (reader.Value)
                {
                    case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                    case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, UserGroup value) { }
}

public class VersionConverter : JsonRedmineConverter<Types_Version>
{
    public override string RootName { get; } = RedmineKeys.VERSION;

    public override Version ReadJson(JsonReader reader)
    {
        var entity = new Version();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadAsCollection<IssueCustomField>(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadAsString(); break;
                case RedmineKeys.DUE_DATE: entity.DueDate = reader.ReadAsDateTime(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                case RedmineKeys.PROJECT: entity.Project = converter.ReadJson(reader); break;
                case RedmineKeys.SHARING: entity.Sharing = EnumHelper.Parse<VersionSharing>(reader.ReadAsString(), true); break;
                case RedmineKeys.STATUS: entity.Status = EnumHelper.Parse<VersionStatus>(reader.ReadAsString(), true); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.WIKI_PAGE_TITLE: entity.WikiPageTitle = reader.ReadAsString(); break;
                case RedmineKeys.ESTIMATED_HOURS: entity.EstimatedHours = (float?)reader.ReadAsDouble(); break;
                case RedmineKeys.SPENT_HOURS: entity.SpentHours = (float?)reader.ReadAsDouble(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, Version entity)
    {
        using (new JsonObject(writer, RootName))
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

public class WatcherConverter : JsonRedmineConverter<Watcher>
{
    public override string RootName { get; } = RedmineKeys.USER;

    public override Watcher ReadJson(JsonReader reader)
    {
        var entity = new Watcher();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType == JsonToken.PropertyName)
            {
                switch (reader.Value)
                {
                    case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                    case RedmineKeys.NAME: entity.Name = reader.ReadAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, Watcher value) { }
}

public class WikiPageConverter : JsonRedmineConverter<WikiPage>
{
    public override string RootName { get; } = RedmineKeys.WIKI_PAGE;

    public override WikiPage ReadJson(JsonReader reader)
    {
        var entity = new WikiPage();

        var converter = JsonConverterRegistry.GetConverter<IdentifiableName>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
            {
                return entity;
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                continue;
            }

            switch (reader.Value)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadAsInt(); break;
                case RedmineKeys.ATTACHMENTS: entity.Attachments = reader.ReadAsCollection<Attachment>(); break;
                case RedmineKeys.AUTHOR: entity.Author = converter.ReadJson(reader); break;
                case RedmineKeys.COMMENTS: entity.Comments = reader.ReadAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.TEXT: entity.Text = reader.ReadAsString(); break;
                case RedmineKeys.TITLE: entity.Title = reader.ReadAsString(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadAsDateTime(); break;
                case RedmineKeys.VERSION: entity.Version = reader.ReadAsInt(); break;
                case RedmineKeys.PARENT: entity.ParentTitle = reader.ReadAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteJson(JsonWriter writer, WikiPage entity)
    {
        using (new JsonObject(writer, RootName))
        {
            writer.WriteProperty(RedmineKeys.TEXT, entity.Text);
            writer.WriteProperty(RedmineKeys.COMMENTS, entity.Comments);
            writer.WriteValueOrEmpty<int>(RedmineKeys.VERSION, entity.Version);
            writer.WriteArray(RedmineKeys.UPLOADS, entity.Uploads);
        }
    }
}
