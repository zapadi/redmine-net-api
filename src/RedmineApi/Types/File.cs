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
/// 
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class File : Identifiable<File>
{
    #region Properties
    /// <summary>
    /// 
    /// </summary>
    public string Filename { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int FileSize { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ContentUrl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public IdentifiableName Author { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? CreatedOn { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public IdentifiableName Version { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Digest { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Downloads { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Token { get; set; }
    #endregion

    #region Implementation of IEquatable<File>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(File other)
    {
        if (other == null) return false;
        return ((object)this).Equals(other)
               && string.Equals(Filename, other.Filename, StringComparison.Ordinal)
               && string.Equals(ContentType, other.ContentType, StringComparison.Ordinal)
               && string.Equals(Description, other.Description, StringComparison.Ordinal)
               && string.Equals(ContentUrl, other.ContentUrl, StringComparison.Ordinal)
               && string.Equals(Digest, other.Digest, StringComparison.Ordinal)
               && Author == other.Author
               && FileSize == other.FileSize
               && CreatedOn == other.CreatedOn
               && Version == other.Version
               && Downloads == other.Downloads;
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
        return Equals(obj as File);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(Filename, hashCode);
        hashCode = HashCodeHelper.GetHashCode(FileSize, hashCode);
        hashCode = HashCodeHelper.GetHashCode(ContentType, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
        hashCode = HashCodeHelper.GetHashCode(ContentUrl, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
        hashCode = HashCodeHelper.GetHashCode(CreatedOn, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Version, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Digest, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Downloads, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(File left, File right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(File left, File right)
    {
        return !Equals(left, right);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[File: {Id.ToInvariantString()}, Name={Filename}]";

}