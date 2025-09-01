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
/// 
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class Detail : IEquatable<Detail>, ICloneable<Detail>
{
    /// <summary>
    /// 
    /// </summary>
    public Detail() { }

    internal Detail(string name = null, string property = null, string oldValue = null, string newValue = null)
    {
        Name = name;
        Property = property;
        OldValue = oldValue;
        NewValue = newValue;
    }

    #region Properties
    /// <summary>
    /// Gets the property.
    /// </summary>
    /// <value>
    /// The property.
    /// </value>
    public string Property { get; set; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets the old value.
    /// </summary>
    /// <value>
    /// The old value.
    /// </value>
    public string OldValue { get; set; }

    /// <summary>
    /// Gets the new value.
    /// </summary>
    /// <value>
    /// The new value.
    /// </value>
    public string NewValue { get; set; }
    #endregion

    #region Implementation of IEquatable<Detail>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Detail other)
    {
        if (other == null) return false;
        return string.Equals(Property, other.Property, StringComparison.Ordinal)
               && string.Equals(Name, other.Name, StringComparison.Ordinal)
               && string.Equals(OldValue, other.OldValue, StringComparison.Ordinal)
               && string.Equals(NewValue, other.NewValue, StringComparison.Ordinal);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Detail Clone(bool resetId)
    {
        return new Detail(Name, Property, OldValue, NewValue);
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
        return Equals(obj as Detail);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = 17;
        hashCode = HashCodeHelper.GetHashCode(Property, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
        hashCode = HashCodeHelper.GetHashCode(OldValue, hashCode);
        hashCode = HashCodeHelper.GetHashCode(NewValue, hashCode);

        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Detail left, Detail right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Detail left, Detail right)
    {
        return !Equals(left, right);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[Detail: Property={Property}, Name={Name}, OldValue={OldValue}, NewValue={NewValue}]";

}