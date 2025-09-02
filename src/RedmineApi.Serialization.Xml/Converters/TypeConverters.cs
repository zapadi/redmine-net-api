using System;
using System.Xml;
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Serialization.Xml.Extensions;
using Padi.RedmineApi.Types;
using Version = Padi.RedmineApi.Types.Version;

namespace Padi.RedmineApi.Serialization.Xml.Converters;

internal sealed class AttachmentConverter : XmlRedmineConverter<Attachment>
{
    public override string RootName { get; } = RedmineKeys.ATTACHMENT;

    public override Attachment ReadXml(XmlReader reader)
    {
        var result = new Attachment();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: result.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.AUTHOR: result.Author = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.CONTENT_TYPE: result.ContentType = reader.ReadElementContentAsString(); break;
                case RedmineKeys.CONTENT_URL: result.ContentUrl = reader.ReadElementContentAsString(); break;
                case RedmineKeys.CREATED_ON: result.CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.DESCRIPTION: result.Description = reader.ReadElementContentAsString(); break;
                case RedmineKeys.FILE_NAME: result.FileName = reader.ReadElementContentAsString(); break;
                case RedmineKeys.FILE_SIZE: result.FileSize = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.THUMBNAIL_URL: result.ThumbnailUrl = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return result;
    }

    public override void WriteXml(XmlWriter writer, Attachment value)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.FILE_NAME, value.FileName);
        writer.WriteElementString(RedmineKeys.DESCRIPTION, value.Description);
        writer.WriteEndElement();
    }
}

internal sealed class ChangeSetConverter : XmlRedmineConverter<ChangeSet>
{
    public override string RootName { get; } = RedmineKeys.CHANGE_SET;

    public override ChangeSet ReadXml(XmlReader reader)
    {
        var entity = new ChangeSet();

        while (!reader.EOF)
        {
            if (reader is { IsEmptyElement: true, HasAttributes: false })
            {
                reader.Read();
                continue;
            }

            entity.Revision = reader.GetAttribute(RedmineKeys.REVISION);

            switch (reader.Name)
            {
                case RedmineKeys.COMMENTS: entity.Comments = reader.ReadElementContentAsString(); break;
                case RedmineKeys.COMMITTED_ON: entity.CommittedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.USER: entity.User = reader.ReadAsIdentifiableName(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, ChangeSet value) { }
}

/// <summary>
/// XML converter for CustomFieldPossibleValue type
/// </summary>
internal sealed class CustomFieldConverter : XmlRedmineConverter<CustomField>
{
    public override string RootName { get; } = RedmineKeys.CUSTOM_FIELD;

    /// <summary>
    /// Reads the XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>An instance of CustomFieldPossibleValue</returns>
    public override CustomField ReadXml(XmlReader reader)
    {
        var entity = new CustomField();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.CUSTOMIZED_TYPE: entity.CustomizedType = reader.ReadElementContentAsString(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadElementContentAsString(); break;
                case RedmineKeys.DEFAULT_VALUE: entity.DefaultValue = reader.ReadElementContentAsString(); break;
                case RedmineKeys.FIELD_FORMAT: entity.FieldFormat = reader.ReadElementContentAsString(); break;
                case RedmineKeys.IS_FILTER: entity.IsFilter = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.IS_REQUIRED: entity.IsRequired = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.MAX_LENGTH: entity.MaxLength = reader.ReadElementContentAsNullableInt(); break;
                case RedmineKeys.MIN_LENGTH: entity.MinLength = reader.ReadElementContentAsNullableInt(); break;
                case RedmineKeys.MULTIPLE: entity.Multiple = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                case RedmineKeys.POSSIBLE_VALUES: entity.PossibleValues = reader.ReadElementContentAsCollection<CustomFieldPossibleValue>(); break;
                case RedmineKeys.REGEXP: entity.Regexp = reader.ReadElementContentAsString(); break;
                case RedmineKeys.ROLES: entity.Roles = reader.ReadElementContentAsCollection<CustomFieldRole>(); break;
                case RedmineKeys.SEARCHABLE: entity.Searchable = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.TRACKERS: entity.Trackers = reader.ReadElementContentAsCollection<TrackerCustomField>(); break;
                case RedmineKeys.VISIBLE: entity.Visible = reader.ReadElementContentAsBoolean(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    /// <summary>
    /// Writes the XML.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The CustomFieldPossibleValue object to write</param>
    public override void WriteXml(XmlWriter writer, CustomField value) { }
}

/// <summary>
///     XML converter for CustomFieldPossibleValue type
/// </summary>
internal sealed class CustomFieldPossibleValueConverter : XmlRedmineConverter<CustomFieldPossibleValue>
{
    public override string RootName { get; } = RedmineKeys.POSSIBLE_VALUE;

    /// <summary>
    ///     Reads the XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>An instance of CustomFieldPossibleValue</returns>
    public override CustomFieldPossibleValue ReadXml(XmlReader reader)
    {
        var entity = new CustomFieldPossibleValue();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.LABEL: entity.Label = reader.ReadElementContentAsString(); break;
                case RedmineKeys.VALUE: entity.Value = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    /// <summary>
    ///     Writes the XML.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The CustomFieldPossibleValue object to write</param>
    public override void WriteXml(XmlWriter writer, CustomFieldPossibleValue value) { }
}

/// <summary>
/// XML converter for CustomFieldRole type
/// </summary>
internal sealed class CustomFieldRoleConverter : XmlRedmineConverter<CustomFieldRole>
{
    public override string RootName { get; } = RedmineKeys.ROLE;

    /// <summary>
    /// Reads the XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>An instance of CustomFieldRole</returns>
    public override CustomFieldRole? ReadXml(XmlReader reader)
    {
        if (reader.Name != RootName || !reader.HasAttributes)
        {
            return null;
        }

        var result = new CustomFieldRole
        {
            Id = reader.ReadAttributeAsInt(RedmineKeys.ID),
            Name = reader.GetAttribute(RedmineKeys.NAME)
        };

        reader.Read();

        return result;
    }

    /// <summary>
    /// Writes the XML.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The CustomFieldRole object to write</param>
    public override void WriteXml(XmlWriter writer, CustomFieldRole value)
    {
        writer.WriteStartElement(RootName);
        writer.WriteAttributeString(RedmineKeys.ID, StringExtensions.ToInvariantString<int>(value.Id));
        writer.WriteAttributeString(RedmineKeys.NAME, value.Name);
        writer.WriteEndElement();
    }
}

internal sealed class CustomFieldValueConverter : XmlRedmineConverter<CustomFieldValue>
{
    public override string RootName { get; } = RedmineKeys.VALUE;

    public override CustomFieldValue ReadXml(XmlReader reader)
    {
        string? info = null;

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.VALUE: info = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return new CustomFieldValue() { Info = info };
    }

    public override void WriteXml(XmlWriter writer, CustomFieldValue value) { }
}

/// <summary>
/// XML converter for DetailConverter type
/// </summary>
internal sealed class DetailConverter : XmlRedmineConverter<Detail>
{
    public override string RootName { get; } = RedmineKeys.DETAIL;

    /// <summary>
    /// Reads the XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>An instance of Detail</returns>
    public override Detail ReadXml(XmlReader reader)
    {
        var entity = new Detail();

        if (reader.Name == RootName && reader.HasAttributes)
        {
            entity.Name = reader.GetAttribute(RedmineKeys.NAME);
            entity.Property = reader.GetAttribute(RedmineKeys.PROPERTY);
        }

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.NEW_VALUE: entity.NewValue = reader.ReadElementContentAsString(); break;
                case RedmineKeys.OLD_VALUE: entity.OldValue = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    /// <summary>
    /// Writes the XML.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The Detail object to write</param>
    public override void WriteXml(XmlWriter writer, Detail value) { }
}

/// <summary>
///     XML converter for DocumentCategory type
/// </summary>
internal sealed class DocumentCategoryConverter : XmlRedmineConverter<DocumentCategory>
{
    public override string RootName { get; } = RedmineKeys.DOCUMENT_CATEGORY;

    /// <summary>
    ///     Reads the XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>An instance of DocumentCategory</returns>
    public override DocumentCategory ReadXml(XmlReader reader)
    {
        var result = new DocumentCategory();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: result.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.NAME: result.Name = reader.ReadElementContentAsString(); break;
                case RedmineKeys.IS_DEFAULT: result.IsDefault = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.ACTIVE: result.IsActive = reader.ReadElementContentAsBoolean(); break;
                default: reader.Read(); break;
            }
        }

        return result;
    }

    /// <summary>
    ///     Writes the XML.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The DocumentCategory object to write</param>
    public override void WriteXml(XmlWriter writer, DocumentCategory value) { }
}

internal sealed class ErrorConverter : XmlRedmineConverter<Error>
{
    public override string RootName { get; } = RedmineKeys.ERROR;

    public override Error ReadXml(XmlReader reader)
    {
        string? info = null;

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ERROR: info = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return new Error() { Info = info };
    }

    public override void WriteXml(XmlWriter writer, Error value) { }
}

/// <summary>
///     XML converter for File type
/// </summary>
internal sealed class FileConverter : XmlRedmineConverter<File>
{
    public override string RootName { get; } = RedmineKeys.FILE;

    /// <summary>
    ///     Reads the XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>An instance of File</returns>
    public override File ReadXml(XmlReader reader)
    {
        var entity = new File();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.AUTHOR: entity.Author = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.CONTENT_TYPE: entity.ContentType = reader.ReadElementContentAsString(); break;
                case RedmineKeys.CONTENT_URL: entity.ContentUrl = reader.ReadElementContentAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadElementContentAsString(); break;
                case RedmineKeys.DIGEST: entity.Digest = reader.ReadElementContentAsString(); break;
                case RedmineKeys.DOWNLOADS: entity.Downloads = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.FILE_NAME: entity.Filename = reader.ReadElementContentAsString(); break;
                case RedmineKeys.FILE_SIZE: entity.FileSize = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.TOKEN: entity.Token = reader.ReadElementContentAsString(); break;
                case RedmineKeys.VERSION: entity.Version = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.VERSION_ID: entity.Version = IdentifiableName.Create<IdentifiableName>(reader.ReadElementContentAsInt()); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    /// <summary>
    ///     Writes the XML.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The File object to write</param>
    public override void WriteXml(XmlWriter writer, File value)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.TOKEN, value.Token);
        writer.WriteIdIfNotNull(RedmineKeys.VERSION_ID, value.Version);
        writer.WriteElementString(RedmineKeys.FILE_NAME, value.Filename);
        writer.WriteElementString(RedmineKeys.DESCRIPTION, value.Description);
        writer.WriteEndElement();
    }
}

internal sealed class GroupConverter : XmlRedmineConverter<Group>
{
    public override string RootName { get; } = RedmineKeys.GROUP;

    public override Group ReadXml(XmlReader reader)
    {
        var entity = new Group();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;
                case RedmineKeys.MEMBERSHIPS: entity.Memberships = reader.ReadElementContentAsCollection<Membership>(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                case RedmineKeys.USERS: entity.Users = reader.ReadElementContentAsCollection<GroupUser>(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, Group value)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.NAME, value.Name);
        writer.WriteArrayIds(RedmineKeys.USER_IDS, RedmineKeys.USER_ID, value.Users, GetId);
        writer.WriteEndElement();
        return;

        static string GetId(GroupUser gu)
        {
            return StringExtensions.ToInvariantString<int>(gu.Id);
        }
    }
}

internal sealed class GroupUserConverter : XmlRedmineConverter<GroupUser>
{
    public override string RootName { get; } = RedmineKeys.USER;

    public override GroupUser? ReadXml(XmlReader reader)
    {
        if (reader.Name != RootName || !reader.HasAttributes)
        {
            return null;
        }

        var entity = new GroupUser
        {
            Id = reader.ReadAttributeAsInt(RedmineKeys.ID),
            Name = reader.GetAttribute(RedmineKeys.NAME)
        };

        reader.Read();

        return entity;
    }

    public override void WriteXml(XmlWriter writer, GroupUser value)
    {
        writer.WriteElementString(RedmineKeys.USER_ID, StringExtensions.ToInvariantString<int>(value.Id));
    }
}

internal sealed class IdentifiableNameConverter : XmlRedmineConverter<IdentifiableName>
{
    public override string? RootName { get; }

    public override IdentifiableName? ReadXml(XmlReader reader)
    {
        if (!reader.HasAttributes)
        {
            return null;
        }

        var entity = new IdentifiableName
        {
            Id = reader.ReadAttributeAsInt(RedmineKeys.ID),
            Name = reader.GetAttribute(RedmineKeys.NAME)
        };
        reader.Read();
        return entity;
    }

    public override void WriteXml(XmlWriter writer, IdentifiableName value)
    {
        // writer.WriteAttributeString(RedmineKeys.ID, value.Id.ToInvariantString());
        // writer.WriteAttributeString(RedmineKeys.NAME, value.Name);
    }
}

internal sealed class IssueAllowedStatusConverter : XmlRedmineConverter<IssueAllowedStatus>
{
    public override string RootName { get; } = RedmineKeys.STATUS;

    public override IssueAllowedStatus? ReadXml(XmlReader reader)
    {
        if (reader.Name != RootName || !reader.HasAttributes)
        {
            return null;
        }

        var entity = new IssueAllowedStatus
        {
            Id = reader.ReadAttributeAsInt(RedmineKeys.ID),
            Name = reader.GetAttribute(RedmineKeys.NAME),
            IsClosed = reader.ReadAttributeAsBoolean(RedmineKeys.IS_CLOSED)
        };
        reader.Read();

        return entity;
    }

    public override void WriteXml(XmlWriter writer, IssueAllowedStatus value) { }
}

internal sealed class IssueCategoryConverter : XmlRedmineConverter<IssueCategory>
{
    public override string RootName { get; } = RedmineKeys.ISSUE_CATEGORY;

    public override IssueCategory ReadXml(XmlReader reader)
    {
        var entity = new IssueCategory();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.PROJECT: entity.Project = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.ASSIGNED_TO: entity.AssignTo = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, IssueCategory value)
    {
        writer.WriteStartElement(RootName);
        //   writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, value.Project);
        writer.WriteElementString(RedmineKeys.NAME, value.Name);
        writer.WriteIdIfNotNull(RedmineKeys.ASSIGNED_TO_ID, value.AssignTo);
        writer.WriteEndElement();
    }
}

internal sealed class IssueChildConverter : XmlRedmineConverter<IssueChild>
{
    public override string RootName { get; } = RedmineKeys.ISSUE;

    public override IssueChild ReadXml(XmlReader reader)
    {
        var entity = new IssueChild();
        if (reader.Name == RootName && reader.HasAttributes)
        {
            entity.Id = reader.ReadAttributeAsInt(RedmineKeys.ID);
            reader.Read();
        }

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.TRACKER: entity.Tracker = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.SUBJECT: entity.Subject = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, IssueChild value) { }
}

internal sealed class IssueConverter : XmlRedmineConverter<Issue>
{
    public override string RootName { get; } = RedmineKeys.ISSUE;

    public override Issue ReadXml(XmlReader reader)
    {
        var entity = new Issue();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.ALLOWED_STATUSES: entity.AllowedStatuses = reader.ReadElementContentAsCollection<IssueAllowedStatus>(); break;
                case RedmineKeys.ASSIGNED_TO: entity.AssignedTo = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.ATTACHMENTS: entity.Attachments = reader.ReadElementContentAsCollection<Attachment>(); break;
                case RedmineKeys.AUTHOR: entity.Author = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.CATEGORY: entity.Category = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.CHANGE_SETS: entity.ChangeSets = reader.ReadElementContentAsCollection<ChangeSet>(); break;
                case RedmineKeys.CHILDREN: entity.Children = reader.ReadElementContentAsCollection<IssueChild>(); break;
                case RedmineKeys.CLOSED_ON: entity.ClosedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadElementContentAsString(); break;
                case RedmineKeys.DONE_RATIO: entity.DoneRatio = reader.ReadElementContentAsNullableFloat(); break;
                case RedmineKeys.DUE_DATE: entity.DueDate = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.ESTIMATED_HOURS: entity.EstimatedHours = reader.ReadElementContentAsNullableFloat(); break;
                case RedmineKeys.FIXED_VERSION: entity.FixedVersion = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.IS_PRIVATE: entity.IsPrivate = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.JOURNALS: entity.Journals = reader.ReadElementContentAsCollection<Journal>(); break;
                case RedmineKeys.NOTES: entity.Notes = reader.ReadElementContentAsString(); break;
                case RedmineKeys.PARENT: entity.ParentIssue = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.PRIORITY: entity.Priority = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.PRIVATE_NOTES: entity.PrivateNotes = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.PROJECT: entity.Project = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.RELATIONS: entity.Relations = reader.ReadElementContentAsCollection<IssueRelation>(); break;
                case RedmineKeys.SPENT_HOURS: entity.SpentHours = reader.ReadElementContentAsNullableFloat(); break;
                case RedmineKeys.START_DATE: entity.StartDate = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.STATUS: entity.Status = new IssueStatusConverter().ReadXml(reader); break;
                case RedmineKeys.SUBJECT: entity.Subject = reader.ReadElementContentAsString(); break;
                case RedmineKeys.TOTAL_ESTIMATED_HOURS: entity.TotalEstimatedHours = reader.ReadElementContentAsNullableFloat(); break;
                case RedmineKeys.TOTAL_SPENT_HOURS: entity.TotalSpentHours = reader.ReadElementContentAsNullableFloat(); break;
                case RedmineKeys.TRACKER: entity.Tracker = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.WATCHERS: entity.Watchers = reader.ReadElementContentAsCollection<Watcher>(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, Issue entity)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.SUBJECT, entity.Subject);
        writer.WriteElementString(RedmineKeys.NOTES, entity.Notes);

        if (entity.Id != 0)
        {
            writer.WriteBoolean(RedmineKeys.PRIVATE_NOTES, entity.PrivateNotes);
        }

        writer.WriteElementString(RedmineKeys.DESCRIPTION, entity.Description);
        writer.WriteElementString(RedmineKeys.IS_PRIVATE, StringExtensions.ToInvariantString<bool>(entity.IsPrivate));

        writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, entity.Project);
        writer.WriteIdIfNotNull(RedmineKeys.PRIORITY_ID, entity.Priority);
        writer.WriteIdIfNotNull(RedmineKeys.STATUS_ID, entity.Status);
        writer.WriteIdIfNotNull(RedmineKeys.CATEGORY_ID, entity.Category);
        writer.WriteIdIfNotNull(RedmineKeys.TRACKER_ID, entity.Tracker);
        writer.WriteIdIfNotNull(RedmineKeys.ASSIGNED_TO_ID, entity.AssignedTo);
        writer.WriteIdIfNotNull(RedmineKeys.PARENT_ISSUE_ID, entity.ParentIssue);
        writer.WriteIdIfNotNull(RedmineKeys.FIXED_VERSION_ID, entity.FixedVersion);
        writer.WriteValueOrEmpty<float>(RedmineKeys.ESTIMATED_HOURS, entity.EstimatedHours);
        writer.WriteIfNotDefaultOrNull<float?>(RedmineKeys.DONE_RATIO, entity.DoneRatio);

        writer.WriteDateOrEmpty(RedmineKeys.START_DATE, entity.StartDate);
        writer.WriteDateOrEmpty(RedmineKeys.DUE_DATE, entity.DueDate);
        writer.WriteDateOrEmpty(RedmineKeys.UPDATED_ON, entity.UpdatedOn);

        writer.WriteArray(RedmineKeys.UPLOADS, entity.Uploads);
        writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields);
        // if (entity.CustomFields != null)
        // {
        //     writer.WriteStartElement(RedmineKeys.CUSTOM_FIELDS);
        //     writer.WriteAttributeString(Constants.TYPE, Constants.ARRAY);
        //
        //     foreach (var item in entity.CustomFields)
        //     {
        //         writer.WriteStartElement(RedmineKeys.CUSTOM_FIELD);
        //         writer.WriteAttributeString(RedmineKeys.ID, item.Id.ToInvariantString());
        //         if (item.Values != null && item.Values.Count > 0)
        //         {
        //             foreach (var customFieldValue in item.Values)
        //             {
        //                 writer.WriteElementString(RedmineKeys.VALUE, customFieldValue.Info);
        //             }
        //         }
        //
        //         writer.WriteEndElement();
        //     }
        //
        //     writer.WriteEndElement();
        // }
        //
        // writer.WriteListElements(RedmineKeys.WATCHER_USER_IDS, entity.Watchers);
        writer.WriteEndElement();
    }
}

internal sealed class IssueCustomFieldConverter : XmlRedmineConverter<IssueCustomField>
{
    public override string RootName { get; } = RedmineKeys.CUSTOM_FIELD;

    public override IssueCustomField ReadXml(XmlReader reader)
    {
        var entity = new IssueCustomField();

        while (!reader.EOF)
        {
            if (reader is { IsEmptyElement: true, HasAttributes: false })
            {
                reader.Read();
                continue;
            }

            if (reader.Name == RootName && reader.HasAttributes)
            {
                entity.Id = reader.ReadAttributeAsInt(RedmineKeys.ID);
                entity.Name = reader.GetAttribute(RedmineKeys.NAME);
                entity.Multiple = reader.ReadAttributeAsBoolean(RedmineKeys.MULTIPLE);
            }

            if (reader.NodeType == XmlNodeType.Text)
            {
                entity.Values = [new CustomFieldValue(reader.Value)];

                reader.Read();
                return entity;
            }

            var attributeExists = !reader.GetAttribute("type").IsNullOrWhiteSpace();

            if (!attributeExists)
            {
                if (reader.IsEmptyElement)
                {
                    reader.Read();
                    return entity;
                }

                reader.Read();
                if (reader.NodeType == XmlNodeType.Text)
                {
                    entity.Values = [new CustomFieldValue(reader.Value)];
                    reader.Read();
                }
                else if (reader.NodeType == XmlNodeType.Element)
                {
                    entity.Values = [new CustomFieldValue(reader.ReadElementContentAsString())];
                }
            }
            else
            {
                entity.Values = reader.ReadElementContentAsCollection<CustomFieldValue>();
            }

            if (reader.NodeType == XmlNodeType.EndElement)
            {
                return entity;
            }

            reader.Read();
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, IssueCustomField value)
    {
        writer.WriteStartElement(RootName);
        if (value.Values == null)
        {
            return;
        }

        var itemsCount = value.Values.Count;

        writer.WriteAttributeString(RedmineKeys.ID, StringExtensions.ToInvariantString<int>(value.Id));

        value.Multiple = itemsCount > 1;

        writer.WriteAttributeString(RedmineKeys.MULTIPLE, StringExtensions.ToInvariantString<bool>(value.Multiple));

        if (value.Multiple)
        {
            writer.WriteArrayStringElement(RedmineKeys.VALUE, value.Values, IssueCustomField.GetValue);
        }
        else
        {
            writer.WriteElementString(RedmineKeys.VALUE, itemsCount > 0 ? value.Values[0].Info : null);
        }

        writer.WriteEndElement();
    }
}

internal sealed class IssuePriorityConverter : XmlRedmineConverter<IssuePriority>
{
    public override string RootName { get; } = RedmineKeys.ISSUE_PRIORITY;

    public override IssuePriority ReadXml(XmlReader reader)
    {
        var entity = new IssuePriority();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.ACTIVE: entity.IsActive = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.IS_DEFAULT: entity.IsDefault = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, IssuePriority value) { }
}

internal sealed class IssueRelationConverter : XmlRedmineConverter<IssueRelation>
{
    public override string RootName { get; } = RedmineKeys.RELATION;

    public override IssueRelation ReadXml(XmlReader reader)
    {
        var entity = new IssueRelation();

        while (!reader.EOF)
        {
            if (reader is { IsEmptyElement: true, HasAttributes: false })
            {
                reader.Read();
                continue;
            }

            if (reader.IsEmptyElement && reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    var attributeName = reader.Name;
                    switch (attributeName)
                    {
                        case RedmineKeys.ID: entity.Id = reader.ReadAttributeAsInt(attributeName); break;
                        case RedmineKeys.DELAY: entity.Delay = reader.ReadAttributeAsNullableInt(attributeName); break;
                        case RedmineKeys.ISSUE_ID: entity.IssueId = reader.ReadAttributeAsInt(attributeName); break;
                        case RedmineKeys.ISSUE_TO_ID: entity.IssueToId = reader.ReadAttributeAsInt(attributeName); break;
                        case RedmineKeys.RELATION_TYPE: entity.Type = ReadIssueRelationType(reader.GetAttribute(attributeName)); break;
                    }
                }

                return entity;
            }

            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.DELAY: entity.Delay = reader.ReadElementContentAsNullableInt(); break;
                case RedmineKeys.ISSUE_ID: entity.IssueId = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.ISSUE_TO_ID: entity.IssueToId = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.RELATION_TYPE: entity.Type = ReadIssueRelationType(reader.ReadElementContentAsString()); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, IssueRelation value)
    {
        AssertValidIssueRelationType(value);

        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.ISSUE_TO_ID, StringExtensions.ToInvariantString<int>(value.IssueToId));
        writer.WriteElementString(RedmineKeys.RELATION_TYPE, value.Type.ToLowerInvariant());

        if (value.Type is IssueRelationType.Precedes or IssueRelationType.Follows)
        {
            writer.WriteValueOrEmpty(RedmineKeys.DELAY, value.Delay);
        }

        writer.WriteEndElement();

        return;

        static void AssertValidIssueRelationType(IssueRelation value)
        {
            if (value.Type == IssueRelationType.Undefined)
            {
                throw new RedmineException(RedmineApiErrorCode.SerializationError,$"The value `{nameof(IssueRelationType)}.`{nameof(IssueRelationType.Undefined)}` is not allowed to create relations!");
            }
        }
    }

    private static IssueRelationType ReadIssueRelationType(string? value)
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

internal sealed class IssueStatusConverter : XmlRedmineConverter<IssueStatus>
{
    public override string RootName { get; } = RedmineKeys.ISSUE_STATUS;

    public override IssueStatus ReadXml(XmlReader reader)
    {
        var entity = new IssueStatus();

        if (reader is { HasAttributes: true, Name: "status" })
        {
            entity.Id = reader.ReadAttributeAsInt(RedmineKeys.ID);
            entity.IsClosed = reader.ReadAttributeAsBoolean(RedmineKeys.IS_CLOSED);
            entity.IsDefault = reader.ReadAttributeAsBoolean(RedmineKeys.IS_DEFAULT);
            entity.Name = reader.GetAttribute(RedmineKeys.NAME);
            reader.Read();
            return entity;
        }

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.IS_CLOSED: entity.IsClosed = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.IS_DEFAULT: entity.IsDefault = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, IssueStatus value)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.ID, value.Id.ToInvariantString());
        writer.WriteElementString(RedmineKeys.NAME, value.Name);
        writer.WriteElementString(RedmineKeys.IS_CLOSED, value.IsClosed.ToInvariantString());
        writer.WriteElementString(RedmineKeys.IS_DEFAULT, value.IsDefault.ToInvariantString());
        writer.WriteEndElement();
    }
}

internal sealed class JournalConverter : XmlRedmineConverter<Journal>
{
    public override string RootName { get; } = RedmineKeys.JOURNAL;

    public override Journal ReadXml(XmlReader reader)
    {
        var entity = new Journal();

        if (reader.Name == RootName && reader.HasAttributes)
        {
            entity.Id = reader.ReadAttributeAsInt(RedmineKeys.ID);
            reader.Read();
        }

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.DETAILS: entity.Details = reader.ReadElementContentAsCollection<Detail>(); break;
                case RedmineKeys.NOTES: entity.Notes = reader.ReadElementContentAsString(); break;
                case RedmineKeys.PRIVATE_NOTES: entity.PrivateNotes = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.USER: entity.User = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.UPDATED_BY: entity.UpdatedBy = reader.ReadAsIdentifiableName(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, Journal entity)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.NOTES, entity.Notes);
        writer.WriteEndElement();
    }
}

internal sealed class MembershipConverter : XmlRedmineConverter<Membership>
{
    public override string RootName { get; } = RedmineKeys.MEMBERSHIP;

    public override Membership ReadXml(XmlReader reader)
    {
        var entity = new Membership();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.GROUP: entity.Group = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.PROJECT: entity.Project = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.USER: entity.User = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.ROLES: entity.Roles = reader.ReadElementContentAsCollection<MembershipRole>(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, Membership value) { }
}

internal sealed class MembershipRoleConverter : XmlRedmineConverter<MembershipRole>
{
    public override string RootName { get; } = RedmineKeys.POSSIBLE_VALUE;

    public override MembershipRole? ReadXml(XmlReader reader)
    {
        if (reader is not { Name: RedmineKeys.ROLE, HasAttributes: true })
        {
            return null;
        }

        var entity = new MembershipRole
        {
            Id = reader.ReadAttributeAsInt(RedmineKeys.ID),
            Inherited = reader.ReadAttributeAsBoolean(RedmineKeys.INHERITED),
            Name = reader.GetAttribute(RedmineKeys.NAME)
        };

        reader.Read();

        return entity;
    }

    public override void WriteXml(XmlWriter writer, MembershipRole value)
    {
        writer.WriteStartElement(RootName);
        writer.WriteValue(value.Id);
        writer.WriteEndElement();
    }
}

internal sealed class MyAccountConverter : XmlRedmineConverter<MyAccount>
{
    public override string RootName { get; } = RedmineKeys.USER;

    public override MyAccount ReadXml(XmlReader reader)
    {
        var entity = new MyAccount();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.ADMIN: entity.IsAdmin = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.API_KEY: entity.ApiKey = reader.ReadElementContentAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadElementContentAsCollection<MyAccountCustomField>(); break;
                case RedmineKeys.FIRST_NAME: entity.FirstName = reader.ReadElementContentAsString(); break;
                case RedmineKeys.LAST_LOGIN_ON: entity.LastLoginOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.LAST_NAME: entity.LastName = reader.ReadElementContentAsString(); break;
                case RedmineKeys.LOGIN: entity.Login = reader.ReadElementContentAsString(); break;
                case RedmineKeys.MAIL: entity.Email = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, MyAccount entity)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.FIRST_NAME, entity.FirstName);
        writer.WriteElementString(RedmineKeys.LAST_NAME, entity.LastName);
        writer.WriteElementString(RedmineKeys.MAIL, entity.Email);
        writer.WriteEndElement();
    }
}

internal sealed class MyAccountCustomFieldConverter : XmlRedmineConverter<MyAccountCustomField>
{
    public override string RootName { get; } = RedmineKeys.CUSTOM_FIELD;

    public override MyAccountCustomField ReadXml(XmlReader reader)
    {
        var entity = new MyAccountCustomField();

        if (reader.Name == RootName && reader.HasAttributes)
        {
            entity.Id = reader.ReadAttributeAsInt(RedmineKeys.ID);
            entity.Name = reader.GetAttribute(RedmineKeys.NAME);
            reader.Read();
        }

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.VALUE: entity.Value = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, MyAccountCustomField entity) { }
}

internal sealed class NewsCommentConverter : XmlRedmineConverter<NewsComment>
{
    public override string RootName { get; } = RedmineKeys.COMMENT;

    public override NewsComment ReadXml(XmlReader reader)
    {
        var entity = new NewsComment();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.AUTHOR: entity.Author = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.CONTENT: entity.Content = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, NewsComment value) { }
}

internal sealed class NewsConverter : XmlRedmineConverter<News>
{
    public override string RootName { get; } = RedmineKeys.NEWS;

    public override News ReadXml(XmlReader reader)
    {
        var entity = new News();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.AUTHOR: entity.Author = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadElementContentAsString(); break;
                case RedmineKeys.PROJECT: entity.Project = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.SUMMARY: entity.Summary = reader.ReadElementContentAsString(); break;
                case RedmineKeys.TITLE: entity.Title = reader.ReadElementContentAsString(); break;
                case RedmineKeys.ATTACHMENTS: entity.Attachments = reader.ReadElementContentAsCollection<Attachment>(); break;
                case RedmineKeys.COMMENTS: entity.Comments = reader.ReadElementContentAsCollection<NewsComment>(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, News entity)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.TITLE, entity.Title);
        writer.WriteElementString(RedmineKeys.SUMMARY, entity.Summary);
        writer.WriteElementString(RedmineKeys.DESCRIPTION, entity.Description);
        writer.WriteArray(RedmineKeys.UPLOADS, entity.Uploads);
        writer.WriteEndElement();
    }
}

internal sealed class PermissionConverter : XmlRedmineConverter<Permission>
{
    public override string RootName { get; } = RedmineKeys.POSSIBLE_VALUE;

    public override Permission ReadXml(XmlReader reader)
    {
        var entity = new Permission();

        while (!reader.EOF)
        {
            if (reader.NodeType == XmlNodeType.Text)
            {
                entity.Info = reader.Value;
            }

            reader.Read();
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, Permission value) { }
}

internal sealed class ProjectConverter : XmlRedmineConverter<Project>
{
    public override string RootName { get; } = RedmineKeys.PROJECT;

    public override Project ReadXml(XmlReader reader)
    {
        var entity = new Project();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.IssueCustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadElementContentAsString(); break;
                case RedmineKeys.ENABLED_MODULES: entity.EnabledModules = reader.ReadElementContentAsCollection<ProjectEnabledModule>(); break;
                case RedmineKeys.HOMEPAGE: entity.HomePage = reader.ReadElementContentAsString(); break;
                case RedmineKeys.IDENTIFIER: entity.Identifier = reader.ReadElementContentAsString(); break;
                case RedmineKeys.INHERIT_MEMBERS: entity.InheritMembers = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.IS_PUBLIC: entity.IsPublic = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.ISSUE_CATEGORIES: entity.IssueCategories = reader.ReadElementContentAsCollection<ProjectIssueCategory>(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                case RedmineKeys.PARENT: entity.Parent = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.STATUS: entity.Status = (ProjectStatus)reader.ReadElementContentAsInt(); break;
                case RedmineKeys.TIME_ENTRY_ACTIVITIES: entity.TimeEntryActivities = reader.ReadElementContentAsCollection<ProjectTimeEntryActivity>(); break;
                case RedmineKeys.TRACKERS: entity.Trackers = reader.ReadElementContentAsCollection<ProjectTracker>(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.DEFAULT_ASSIGNEE: entity.DefaultAssignee = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.DEFAULT_VERSION: entity.DefaultVersion = reader.ReadAsIdentifiableName(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, Project entity)
    {
        writer.WriteStartElement(RedmineKeys.PROJECT);
        writer.WriteElementString(RedmineKeys.NAME, entity.Name);
        writer.WriteElementString(RedmineKeys.IDENTIFIER, entity.Identifier);
        writer.WriteIfNotDefaultOrNull(RedmineKeys.DESCRIPTION, entity.Description);
        writer.WriteIfNotDefaultOrNull(RedmineKeys.HOMEPAGE, entity.HomePage);
        writer.WriteBoolean(RedmineKeys.IS_PUBLIC, entity.IsPublic);
        writer.WriteIdIfNotNull(RedmineKeys.PARENT_ID, entity.Parent);
        writer.WriteBoolean(RedmineKeys.INHERIT_MEMBERS, entity.InheritMembers);

        //It works only when the new project is a subproject, and it inherits the members.
        writer.WriteIdIfNotNull(RedmineKeys.DEFAULT_ASSIGNED_TO_ID, entity.DefaultAssignee);
        //It works only with existing shared versions.
        writer.WriteIdIfNotNull(RedmineKeys.DEFAULT_VERSION_ID, entity.DefaultVersion);

        writer.WriteRepeatableElement(RedmineKeys.TRACKER_IDS, entity.Trackers);
        writer.WriteRepeatableElement(RedmineKeys.ENABLED_MODULE_NAMES, entity.EnabledModules);
        writer.WriteRepeatableElement(RedmineKeys.ISSUE_CUSTOM_FIELD_IDS, entity.IssueCustomFields);
        if (entity.Id == 0)
        {
            writer.WriteArray(RedmineKeys.CUSTOM_FIELD_VALUES, entity.CustomFieldValues, GetCustomFieldValue);
        }

        writer.WriteEndElement();

        return;

        void GetCustomFieldValue(IdentifiableName identifiableName)
        {
            writer.WriteStartElement(RedmineKeys.CUSTOM_FIELD_VALUE);
            writer.WriteAttributeString(RedmineKeys.ID, identifiableName.Id.ToInvariantString());
            writer.WriteValue(identifiableName.Name);
            writer.WriteEndElement();
        }
    }
}

/// <summary>
/// XML converter for ProjectEnabledModule type
/// </summary>
internal sealed class ProjectEnabledModuleConverter : XmlRedmineConverter<ProjectEnabledModule>
{
    public override string RootName { get; } = RedmineKeys.ENABLED_MODULE;

    /// <summary>
    /// Reads the XML.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns>An instance of ProjectEnabledModule</returns>
    public override ProjectEnabledModule ReadXml(XmlReader reader)
    {
        var result = new ProjectEnabledModule();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: result.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.NAME: result.Name = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return result;
    }

    /// <summary>
    /// Writes the XML.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The ProjectEnabledModule object to write</param>
    public override void WriteXml(XmlWriter writer, ProjectEnabledModule value)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.ID, value.Id.ToInvariantString());
        writer.WriteElementString(RedmineKeys.NAME, value.Name);
        writer.WriteEndElement();
    }
}

internal sealed class ProjectIssueCategoryConverter : XmlRedmineConverter<ProjectIssueCategory>
{
    public override string RootName { get; } = RedmineKeys.ISSUE_CATEGORY;

    public override ProjectIssueCategory ReadXml(XmlReader reader)
    {
        var entity = new ProjectIssueCategory();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, ProjectIssueCategory value)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.ID, value.Id.ToInvariantString());
        writer.WriteElementString(RedmineKeys.NAME, value.Name);
        writer.WriteEndElement();
    }
}

internal sealed class ProjectMembershipConverter : XmlRedmineConverter<ProjectMembership>
{
    public override string RootName { get; } = RedmineKeys.MEMBERSHIP;

    public override ProjectMembership ReadXml(XmlReader reader)
    {
        var entity = new ProjectMembership();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.GROUP: entity.Group = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.PROJECT: entity.Project = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.ROLES: entity.Roles = reader.ReadElementContentAsCollection<MembershipRole>(); break;
                case RedmineKeys.USER: entity.User = reader.ReadAsIdentifiableName(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, ProjectMembership entity)
    {
        writer.WriteStartElement(RootName);
        if (entity.Id <= 0)
        {
            writer.WriteIdIfNotNull(RedmineKeys.USER_ID, entity.User);
        }

        writer.WriteArrayIds(RedmineKeys.ROLE_IDS, RedmineKeys.ROLE_ID, entity.Roles, GetId);
        writer.WriteEndElement();
    }

    private static string GetId(MembershipRole membershipRole)
    {
        return membershipRole.Id.ToInvariantString();
    }
}

internal sealed class ProjectTimeEntryActivityConverter : XmlRedmineConverter<ProjectTimeEntryActivity>
{
    public override string RootName { get; } = RedmineKeys.TIME_ENTRY_ACTIVITY;

    public override ProjectTimeEntryActivity ReadXml(XmlReader reader)
    {
        var entity = new ProjectTimeEntryActivity();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, ProjectTimeEntryActivity value)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.ID, value.Id.ToInvariantString());
        writer.WriteElementString(RedmineKeys.NAME, value.Name);
        writer.WriteEndElement();
    }
}

internal sealed class ProjectTrackerConverter : XmlRedmineConverter<ProjectTracker>
{
    public override string RootName { get; } = RedmineKeys.TRACKER;

    public override ProjectTracker ReadXml(XmlReader reader)
    {
        var entity = new ProjectTracker();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, ProjectTracker value)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.ID, value.Id.ToInvariantString());
        writer.WriteElementString(RedmineKeys.NAME, value.Name);
        writer.WriteEndElement();
    }
}

internal sealed class QueryConverter : XmlRedmineConverter<Query>
{
    public override string RootName { get; } = RedmineKeys.QUERY;

    public override Query ReadXml(XmlReader reader)
    {
        var entity = new Query();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.IS_PUBLIC: entity.IsPublic = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                case RedmineKeys.PROJECT_ID: entity.ProjectId = reader.ReadElementContentAsNullableInt(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, Query value) { }
}

internal sealed class RoleConverter : XmlRedmineConverter<Role>
{
    public override string RootName { get; } = RedmineKeys.ROLE;

    public override Role ReadXml(XmlReader reader)
    {
        var entity = new Role();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                case RedmineKeys.ASSIGNABLE: entity.IsAssignable = reader.ReadElementContentAsNullableBoolean(); break;
                case RedmineKeys.ISSUES_VISIBILITY: entity.IssuesVisibility = reader.ReadElementContentAsString(); break;
                case RedmineKeys.TIME_ENTRIES_VISIBILITY: entity.TimeEntriesVisibility = reader.ReadElementContentAsString(); break;
                case RedmineKeys.USERS_VISIBILITY: entity.UsersVisibility = reader.ReadElementContentAsString(); break;
                case RedmineKeys.PERMISSIONS: entity.Permissions = reader.ReadElementContentAsCollection<Permission>(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, Role value) { }
}

internal sealed class SearchConverter : XmlRedmineConverter<Search>
{
    public override string RootName { get; } = RedmineKeys.RESULT;

    public override Search ReadXml(XmlReader reader)
    {
        var entity = new Search();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadElementContentAsString(); break;
                case RedmineKeys.DATE_TIME: entity.DateTime = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.URL: entity.Url = reader.ReadElementContentAsString(); break;
                case RedmineKeys.TYPE: entity.Type = reader.ReadElementContentAsString(); break;
                case RedmineKeys.TITLE: entity.Title = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, Search value) { }
}

internal sealed class TimeEntryActivityConverter : XmlRedmineConverter<TimeEntryActivity>
{
    public override string RootName { get; } = RedmineKeys.TIME_ENTRY_ACTIVITY;

    public override TimeEntryActivity ReadXml(XmlReader reader)
    {
        var entity = new TimeEntryActivity();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.IS_DEFAULT: entity.IsDefault = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                case RedmineKeys.ACTIVE: entity.IsActive = reader.ReadElementContentAsBoolean(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, TimeEntryActivity entity) { }
}

internal sealed class TimeEntryConverter : XmlRedmineConverter<TimeEntry>
{
    public override string RootName { get; } = RedmineKeys.TIME_ENTRY;

    public override TimeEntry ReadXml(XmlReader reader)
    {
        var entity = new TimeEntry();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.ACTIVITY: entity.Activity = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.ACTIVITY_ID: entity.Activity = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.COMMENTS: entity.Comments = reader.ReadElementContentAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;
                case RedmineKeys.HOURS: entity.Hours = reader.ReadElementContentAsDecimal(); break;
                case RedmineKeys.ISSUE_ID: entity.Issue = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.ISSUE: entity.Issue = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.PROJECT: entity.Project = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.PROJECT_ID: entity.Project = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.SPENT_ON: entity.SpentOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.USER: entity.User = reader.ReadAsIdentifiableName(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, TimeEntry entity)
    {
        writer.WriteStartElement(RootName);
        writer.WriteIdIfNotNull(RedmineKeys.ISSUE_ID, entity.Issue);
        writer.WriteIdIfNotNull(RedmineKeys.PROJECT_ID, entity.Project);
        writer.WriteDateOrEmpty(RedmineKeys.SPENT_ON, entity.SpentOn.GetValueOrDefault(DateTime.Now));
        writer.WriteValueOrEmpty<decimal>(RedmineKeys.HOURS, entity.Hours);
        writer.WriteIdIfNotNull(RedmineKeys.ACTIVITY_ID, entity.Activity);
        writer.WriteElementString(RedmineKeys.COMMENTS, entity.Comments);
        writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields);
        writer.WriteEndElement();
    }
}

internal sealed class TrackerConverter : XmlRedmineConverter<Tracker>
{
    public override string RootName { get; } = RedmineKeys.TRACKER;

    public override Tracker ReadXml(XmlReader reader)
    {
        var entity = new Tracker();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                case RedmineKeys.DEFAULT_STATUS: entity.DefaultStatus = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadElementContentAsString(); break;
                case RedmineKeys.ENABLED_STANDARD_FIELDS: entity.EnabledStandardFields = reader.ReadElementContentAsCollection<TrackerCoreField>(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, Tracker value) { }
}

internal sealed class TrackerCoreFieldConverter : XmlRedmineConverter<TrackerCoreField>
{
    public override string RootName { get; } = RedmineKeys.FIELD;

    public override TrackerCoreField ReadXml(XmlReader reader)
    {
        var entity = new TrackerCoreField();
        reader.Read();
        if (reader.NodeType == XmlNodeType.Text)
        {
            entity.Name = reader.Value;
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, TrackerCoreField value) { }
}

internal sealed class TrackerCustomFieldConverter : XmlRedmineConverter<TrackerCustomField>
{
    public override string RootName { get; } = RedmineKeys.TRACKER;

    public override TrackerCustomField? ReadXml(XmlReader reader)
    {
        if (reader.Name != RootName || !reader.HasAttributes)
        {
            return null;
        }

        var entity = new TrackerCustomField
        {
            Id = reader.ReadAttributeAsInt(RedmineKeys.ID),
            Name = reader.GetAttribute(RedmineKeys.NAME)
        };
        reader.Read();
        return entity;
    }

    public override void WriteXml(XmlWriter writer, TrackerCustomField value)
    {
        writer.WriteStartElement(RootName);
        writer.WriteAttributeString(RedmineKeys.ID, value.Id.ToInvariantString());
        writer.WriteAttributeString(RedmineKeys.NAME, value.Name);
        writer.WriteEndElement();
    }
}

internal sealed class UploadConverter : XmlRedmineConverter<Upload>
{
    public override string RootName { get; } = RedmineKeys.UPLOAD;

    public override Upload ReadXml(XmlReader reader)
    {
        var entity = new Upload();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsString(); break;
                case RedmineKeys.CONTENT_TYPE: entity.ContentType = reader.ReadElementContentAsString(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadElementContentAsString(); break;
                case RedmineKeys.FILE_NAME: entity.FileName = reader.ReadElementContentAsString(); break;
                case RedmineKeys.TOKEN: entity.Token = reader.ReadElementContentAsString(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, Upload entity)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.TOKEN, entity.Token);
        writer.WriteElementString(RedmineKeys.CONTENT_TYPE, entity.ContentType);
        writer.WriteElementString(RedmineKeys.FILE_NAME, entity.FileName);
        writer.WriteElementString(RedmineKeys.DESCRIPTION, entity.Description);
        writer.WriteEndElement();
    }
}

internal sealed class UserConverter : XmlRedmineConverter<User>
{
    public override string RootName { get; } = RedmineKeys.USER;

    public override User ReadXml(XmlReader reader)
    {
        var entity = new User();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.ADMIN: entity.IsAdmin = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.API_KEY: entity.ApiKey = reader.ReadElementContentAsString(); break;
                case RedmineKeys.AUTH_SOURCE_ID: entity.AuthenticationModeId = reader.ReadElementContentAsNullableInt(); break;
                case RedmineKeys.AVATAR_URL: entity.AvatarUrl = reader.ReadElementContentAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;
                case RedmineKeys.FIRST_NAME: entity.FirstName = reader.ReadElementContentAsString(); break;
                case RedmineKeys.GROUPS: entity.Groups = reader.ReadElementContentAsCollection<UserGroup>(); break;
                case RedmineKeys.LAST_LOGIN_ON: entity.LastLoginOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.LAST_NAME: entity.LastName = reader.ReadElementContentAsString(); break;
                case RedmineKeys.LOGIN: entity.Login = reader.ReadElementContentAsString(); break;
                case RedmineKeys.MAIL: entity.Email = reader.ReadElementContentAsString(); break;
                case RedmineKeys.MAIL_NOTIFICATION: entity.MailNotification = reader.ReadElementContentAsString(); break;
                case RedmineKeys.MEMBERSHIPS: entity.Memberships = reader.ReadElementContentAsCollection<Membership>(); break;
                case RedmineKeys.MUST_CHANGE_PASSWORD: entity.MustChangePassword = reader.ReadElementContentAsBoolean(); break;
                case RedmineKeys.PASSWORD_CHANGED_ON: entity.PasswordChangedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.STATUS: entity.Status = (UserStatus)reader.ReadElementContentAsInt(); break;
                case RedmineKeys.TWO_FA_SCHEME: entity.TwoFactorAuthenticationScheme = reader.ReadElementContentAsString(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, User entity)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.LOGIN, entity.Login);

        if (!entity.Password.IsNullOrWhiteSpace())
        {
            writer.WriteElementString(RedmineKeys.PASSWORD, entity.Password);
        }

        writer.WriteElementString(RedmineKeys.FIRST_NAME, entity.FirstName);
        writer.WriteElementString(RedmineKeys.LAST_NAME, entity.LastName);
        writer.WriteElementString(RedmineKeys.MAIL, entity.Email);

        if (entity.AuthenticationModeId.HasValue)
        {
            writer.WriteValueOrEmpty(RedmineKeys.AUTH_SOURCE_ID, entity.AuthenticationModeId);
        }

        if (!entity.MailNotification.IsNullOrWhiteSpace())
        {
            writer.WriteElementString(RedmineKeys.MAIL_NOTIFICATION, entity.MailNotification);
        }

        writer.WriteBoolean(RedmineKeys.MUST_CHANGE_PASSWORD, entity.MustChangePassword);
        writer.WriteBoolean(RedmineKeys.GENERATE_PASSWORD, entity.GeneratePassword);
        writer.WriteBoolean(RedmineKeys.SEND_INFORMATION, entity.SendInformation);

        writer.WriteElementString(RedmineKeys.STATUS, ((int)entity.Status).ToInvariantString());

        writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields);
        writer.WriteEndElement();
    }
}

internal sealed class UserGroupConverter : XmlRedmineConverter<UserGroup>
{
    public override string RootName { get; } = RedmineKeys.GROUP;

    public override UserGroup? ReadXml(XmlReader reader)
    {
        if (reader.Name != RootName || !reader.HasAttributes)
        {
            return null;
        }

        var entity = new UserGroup
        {
            Id = reader.ReadAttributeAsInt(RedmineKeys.ID),
            Name = reader.GetAttribute(RedmineKeys.NAME)
        };

        reader.Read();

        return entity;

    }

    public override void WriteXml(XmlWriter writer, UserGroup value)
    {
        writer.WriteElementString(RedmineKeys.USER_ID, value.Id.ToInvariantString());
    }
}

internal sealed class VersionConverter : XmlRedmineConverter<Version>
{
    public override string RootName { get; } = RedmineKeys.VERSION;

    public override Version ReadXml(XmlReader reader)
    {
        var entity = new Version();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.CUSTOM_FIELDS: entity.CustomFields = reader.ReadElementContentAsCollection<IssueCustomField>(); break;
                case RedmineKeys.DESCRIPTION: entity.Description = reader.ReadElementContentAsString(); break;
                case RedmineKeys.DUE_DATE: entity.DueDate = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.NAME: entity.Name = reader.ReadElementContentAsString(); break;
                case RedmineKeys.PROJECT: entity.Project = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.SHARING: entity.Sharing = EnumHelper.Parse<VersionSharing>(reader.ReadElementContentAsString(), true); break;
                case RedmineKeys.STATUS: entity.Status = EnumHelper.Parse<VersionStatus>(reader.ReadElementContentAsString(), true); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.WIKI_PAGE_TITLE: entity.WikiPageTitle = reader.ReadElementContentAsString(); break;
                case RedmineKeys.ESTIMATED_HOURS: entity.EstimatedHours = reader.ReadElementContentAsNullableFloat(); break;
                case RedmineKeys.SPENT_HOURS: entity.SpentHours = reader.ReadElementContentAsNullableFloat(); break;
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, Version entity)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.NAME, entity.Name);
        writer.WriteElementString(RedmineKeys.STATUS, StringExtensions.ToInvariantString(entity.Status));

        if (entity.Sharing != VersionSharing.Unknown)
        {
            writer.WriteElementString(RedmineKeys.SHARING, StringExtensions.ToInvariantString(entity.Sharing));
        }

        writer.WriteDateOrEmpty(RedmineKeys.DUE_DATE, entity.DueDate);
        writer.WriteElementString(RedmineKeys.DESCRIPTION, entity.Description);
        writer.WriteElementString(RedmineKeys.WIKI_PAGE_TITLE, entity.WikiPageTitle);
        writer.WriteArray(RedmineKeys.CUSTOM_FIELDS, entity.CustomFields);
        writer.WriteEndElement();
    }
}

internal sealed class WatcherConverter : XmlRedmineConverter<Watcher>
{
    public override string RootName { get; } = RedmineKeys.USER;

    public override Watcher? ReadXml(XmlReader reader)
    {
        if (reader.Name != RootName || !reader.HasAttributes)
        {
            return null;
        }
        var entity = new Watcher
        {
            Id = reader.ReadAttributeAsInt(RedmineKeys.ID),
            Name = reader.GetAttribute(RedmineKeys.NAME)
        };

        reader.Read();

        return entity;
    }

    public override void WriteXml(XmlWriter writer, Watcher value)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.ID, value.Id.ToInvariantString());
        writer.WriteElementString(RedmineKeys.NAME, value.Name);
        writer.WriteEndElement();
    }
}

internal sealed class WikiPageConverter : XmlRedmineConverter<WikiPage>
{
    public override string RootName { get; } = RedmineKeys.WIKI_PAGE;

    public override WikiPage ReadXml(XmlReader reader)
    {
        var entity = new WikiPage();

        while (!reader.EOF)
        {
            switch (reader.Name)
            {
                case RedmineKeys.ID: entity.Id = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.ATTACHMENTS: entity.Attachments = reader.ReadElementContentAsCollection<Attachment>(); break;
                case RedmineKeys.AUTHOR: entity.Author = reader.ReadAsIdentifiableName(); break;
                case RedmineKeys.COMMENTS: entity.Comments = reader.ReadElementContentAsString(); break;
                case RedmineKeys.CREATED_ON: entity.CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.TEXT: entity.Text = reader.ReadElementContentAsString(); break;
                case RedmineKeys.TITLE: entity.Title = reader.ReadElementContentAsString(); break;
                case RedmineKeys.UPDATED_ON: entity.UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;
                case RedmineKeys.VERSION: entity.Version = reader.ReadElementContentAsInt(); break;
                case RedmineKeys.PARENT:
                {
                    if (reader.HasAttributes)
                    {
                        entity.ParentTitle = reader.GetAttribute(RedmineKeys.TITLE);
                        reader.Read();
                    }

                    break;
                }
                default: reader.Read(); break;
            }
        }

        return entity;
    }

    public override void WriteXml(XmlWriter writer, WikiPage entity)
    {
        writer.WriteStartElement(RootName);
        writer.WriteElementString(RedmineKeys.TEXT, entity.Text);
        writer.WriteElementString(RedmineKeys.COMMENTS, entity.Comments);
        writer.WriteValueOrEmpty<int>(RedmineKeys.VERSION, entity.Version);
        writer.WriteArray(RedmineKeys.UPLOADS, entity.Uploads);
        writer.WriteEndElement();
    }
}
