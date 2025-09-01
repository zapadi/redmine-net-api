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
using System.Diagnostics;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Internals;

namespace Padi.RedmineApi.Types;

/// <summary>
/// Availability 1.3
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Attachment : Identifiable<Attachment>, ICloneable<Attachment>
{
    #region Properties
    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    /// <value>The name of the file.</value>
    public string FileName { get; set; }

    /// <summary>
    /// Gets the size of the file.
    /// </summary>
    /// <value>The size of the file.</value>
    public int FileSize { get; set; }

    /// <summary>
    /// Gets the type of the content.
    /// </summary>
    /// <value>The type of the content.</value>
    public string ContentType { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets the content URL.
    /// </summary>
    /// <value>The content URL.</value>
    public string ContentUrl { get; set; }

    /// <summary>
    /// Gets the author.
    /// </summary>
    /// <value>The author.</value>
    public IdentifiableName Author { get; set; }

    /// <summary>
    /// Gets the created on.
    /// </summary>
    /// <value>The created on.</value>
    public DateTime? CreatedOn { get; set; }

    /// <summary>
    /// Gets the thumbnail url.
    /// </summary>
    public string ThumbnailUrl { get; set; }
    #endregion

    #region Implementation of IEquatable<CustomFieldValue>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(Attachment other)
    {
        if (other == null) return false;
        return ((object)this).Equals(other)
               && string.Equals(FileName, other.FileName, StringComparison.Ordinal)
               && string.Equals(ContentType, other.ContentType, StringComparison.Ordinal)
               && string.Equals(Description, other.Description, StringComparison.Ordinal)
               && string.Equals(ContentUrl, other.ContentUrl, StringComparison.Ordinal)
               && string.Equals(ThumbnailUrl, other.ThumbnailUrl, StringComparison.Ordinal)
               && Author == other.Author
               && FileSize == other.FileSize
               && CreatedOn == other.CreatedOn;
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
        return Equals(obj as Attachment);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(FileName, hashCode);
        hashCode = HashCodeHelper.GetHashCode(FileSize, hashCode);
        hashCode = HashCodeHelper.GetHashCode(ContentType, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
        hashCode = HashCodeHelper.GetHashCode(ContentUrl, hashCode);
        hashCode = HashCodeHelper.GetHashCode(ThumbnailUrl, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
        hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Attachment left, Attachment right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Attachment left, Attachment right)
    {
        return !Equals(left, right);
    }
    #endregion

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay =>$"[Attachment: Id={Id.ToInvariantString()}, FileName={FileName}, FileSize={FileSize.ToInvariantString()}]";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public new Attachment Clone(bool resetId)
    {
        if (resetId)
        {
            return new Attachment
            {
                FileName = FileName,
                FileSize = FileSize,
                ContentType = ContentType,
                Description = Description,
                ContentUrl = ContentUrl,
                ThumbnailUrl = ThumbnailUrl,
                Author = Author?.Clone(false),
                CreatedOn = CreatedOn
            };
        }
            
        return new Attachment
        {
            Id = Id,
            FileName = FileName,
            FileSize = FileSize,
            ContentType = ContentType,
            Description = Description,
            ContentUrl = ContentUrl,
            ThumbnailUrl = ThumbnailUrl,
            Author = Author?.Clone(true),
            CreatedOn = CreatedOn
        };
    }
}