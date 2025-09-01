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
public sealed class Watcher : IdentifiableName
    ,IEquatable<Watcher>
    ,ICloneable<Watcher>
    ,IValue
{
    #region Implementation of IValue 
    /// <summary>
    /// 
    /// </summary>
    public string Value => Id.ToInvariantString();

    #endregion

    #region Implementation of ICloneable<T> 
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public new Watcher Clone(bool resetId)
    {
        if (resetId)
        {
            return new Watcher()
            {
                Name = Name
            };
        }
        return new Watcher
        {
            Id = Id,
            Name = Name
        };
    }

    #endregion

    #region Implementation of IEquatable<IdentifiableName>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Watcher other)
    {
        if (other == null) return false;
        return Id == other.Id && string.Equals(Name, other.Name, StringComparison.Ordinal);
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
        return Equals(obj as Watcher);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Watcher left, Watcher right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Watcher left, Watcher right)
    {
        return !Equals(left, right);
    }
    #endregion
        
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[{nameof(Watcher)}: {ToString()}]";
}