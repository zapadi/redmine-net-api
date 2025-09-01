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
using Padi.RedmineApi.Internals;

namespace Padi.RedmineApi.Types;

/// <summary>
/// Support for adding attachments through the REST API is added in Redmine 1.4.0.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Upload : IEquatable<Upload>
    , ICloneable<Upload>
{
    #region Properties
    /// <summary>
    ///  Gets the uploaded id.
    /// </summary>
    public string Id { get; set; }
        
    /// <summary>
    /// Gets or sets the uploaded token.
    /// </summary>
    /// <value>The name of the file.</value>
    public string Token { get; set; }

    /// <summary>
    /// Gets or sets the name of the file.
    /// Maximum allowed file size (1024000).
    /// </summary>
    /// <value>The name of the file.</value>
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    /// <value>The name of the file.</value>
    public string ContentType { get; set; }

    /// <summary>
    /// Gets or sets the file description. (Undocumented feature)
    /// </summary>
    /// <value>The file descroütopm.</value>
    public string Description { get; set; }
    #endregion

    #region Implementation of IEquatable<Upload>
    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    public bool Equals(Upload other)
    {
        return other != null
               && string.Equals(Token, other.Token, StringComparison.Ordinal)
               && string.Equals(FileName, other.FileName, StringComparison.Ordinal)
               && string.Equals(Description, other.Description, StringComparison.Ordinal)
               && string.Equals(ContentType, other.ContentType, StringComparison.Ordinal);
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
        return Equals(obj as Upload);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = 17;
        hashCode = HashCodeHelper.GetHashCode(Token, hashCode);
        hashCode = HashCodeHelper.GetHashCode(FileName, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
        hashCode = HashCodeHelper.GetHashCode(ContentType, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Upload left, Upload right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Upload left, Upload right)
    {
        return !Equals(left, right);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[Upload: Token={Token}, FileName={FileName}]";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Upload Clone(bool resetId)
    {
        return new Upload
        {
            Token = Token,
            FileName = FileName,
            ContentType = ContentType,
            Description = Description
        };
    }
}