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
/// Availability 1.1
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class News : Identifiable<News>
{
    #region Properties
    /// <summary>
    /// Gets or sets the project.
    /// </summary>
    /// <value>The project.</value>
    public IdentifiableName Project { get; set; }

    /// <summary>
    /// Gets or sets the author.
    /// </summary>
    /// <value>The author.</value>
    public IdentifiableName Author { get;  set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>The title.</value>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the summary.
    /// </summary>
    /// <value>The summary.</value>
    public string Summary { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the created on.
    /// </summary>
    /// <value>The created on.</value>
    public DateTime? CreatedOn { get; set; }
        
    /// <summary>
    /// 
    /// </summary>
    public List<Attachment> Attachments { get; set; }
        
    /// <summary>
    /// 
    /// </summary>
    public List<NewsComment> Comments { get; set; }
        
    /// <summary>
    /// 
    /// </summary>
    public List<Upload> Uploads { get; set; }
        
    #endregion

    #region Implementation of IEquatable<News>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(News other)
    {
        if(other == null) return false;

        var result = ((Object)this).Equals(other);
        result = result && Project == other.Project;
        result = result && Author == other.Author;
        result = result && string.Equals(Title, other.Title, StringComparison.Ordinal);
        result = result && string.Equals(Summary, other.Summary, StringComparison.Ordinal);
        result = result && string.Equals(Description, other.Description, StringComparison.Ordinal);
        result = result && CreatedOn == other.CreatedOn;
        result = result && (Attachments?.Equals<Attachment>(other.Attachments) ?? other.Attachments == null);
        result = result && (Comments?.Equals<NewsComment>(other.Comments) ?? other.Comments == null);
        result = result && (Uploads?.Equals<Upload>(other.Uploads) ?? other.Uploads == null);
        return result;
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
        return Equals(obj as News);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = 17;
        hashCode = HashCodeHelper.GetHashCode(Id, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Project, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Title, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Summary, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
        hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Comments, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Attachments, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Uploads, hashCode);
        return hashCode;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(News left, News right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(News left, News right)
    {
        return !Equals(left, right);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[News: Id={Id.ToInvariantString()}, Title={Title}]";

}