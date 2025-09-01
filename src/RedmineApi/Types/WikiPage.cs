/*
   Copyright 2011 - 2025 Adrian Popescu

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
using System.Diagnostics;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Internals;

namespace Padi.RedmineApi.Types;

/// <summary>
/// Availability 2.2
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class WikiPage : Identifiable<WikiPage>
{
    #region Properties
    /// <summary>
    /// Gets the title.
    /// </summary>
    public string Title { get; set; }
        
    /// <summary>
    /// 
    /// </summary>
    public string ParentTitle { get; set; }
        
    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the comments
    /// </summary>
    public string Comments { get; set; }

    /// <summary>
    /// Gets or sets the version
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets the author.
    /// </summary>
    public IdentifiableName Author { get; set; }

    /// <summary>
    /// Gets the created on.
    /// </summary>
    /// <value>The created on.</value>
    public DateTime? CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the updated on.
    /// </summary>
    /// <value>The updated on.</value>
    public DateTime? UpdatedOn { get; set; }

    /// <summary>
    /// Gets the attachments.
    /// </summary>
    /// <value>
    /// The attachments.
    /// </value>
    public List<Attachment> Attachments { get; set; }

    /// <summary>
    /// Sets the uploads.
    /// </summary>
    /// <value>
    /// The uploads.
    /// </value>
    /// <remarks>Availability starting with redmine version 3.3</remarks>
    public List<Upload> Uploads { get; set; }
    #endregion

    #region Implementation of IEquatable<WikiPage>

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(WikiPage other)
    {
        if (other == null) return false;

        return ((object)this).Equals(other)
               && string.Equals(Title, other.Title, StringComparison.Ordinal)
               && string.Equals(Text, other.Text, StringComparison.Ordinal)
               && string.Equals(Comments, other.Comments, StringComparison.Ordinal)
               && string.Equals(ParentTitle, other.ParentTitle, StringComparison.Ordinal)
               && Version == other.Version
               && Author == other.Author
               && CreatedOn == other.CreatedOn
               && UpdatedOn == other.UpdatedOn
               && (Attachments?.Equals<Attachment>(other.Attachments) ?? other.Attachments == null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals(obj as WikiPage);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(Title, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Text, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Comments, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Version, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
        hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
        hashCode = HashCodeHelper.GetHashCode(UpdatedOn, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Attachments, hashCode);
        return hashCode;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(WikiPage left, WikiPage right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(WikiPage left, WikiPage right)
    {
        return !Equals(left, right);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[WikiPage: Id={Id.ToInvariantString()}, Title={Title}]";
}