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
public sealed class NewsComment: Identifiable<NewsComment>
{
    /// <summary>
    /// 
    /// </summary>
    public IdentifiableName Author { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Content { get; set; }
    
    /// <inheritdoc />
    public override bool Equals(NewsComment other)
    {
        if (other == null) return false;
        return Id == other.Id 
               && Author == other.Author 
               && string.Equals(Content, other.Content, StringComparison.Ordinal);
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
        return Equals(obj as NewsComment);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = 17;
        hashCode = HashCodeHelper.GetHashCode(Id, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Author, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Content, hashCode);

        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(NewsComment left, NewsComment right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(NewsComment left, NewsComment right)
    {
        return !Equals(left, right);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[NewsComment: Id={Id.ToInvariantString()}]";
}