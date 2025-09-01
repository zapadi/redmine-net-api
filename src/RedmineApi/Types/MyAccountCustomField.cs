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
public sealed class MyAccountCustomField : IdentifiableName, IEquatable<MyAccountCustomField>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyAccountCustomField"/> class.
    /// </summary>
    /// <remarks>Serialization</remarks>
    public MyAccountCustomField() { }
        
    /// <summary>
    /// 
    /// </summary>
    public string Value { get; set; }
        
    internal MyAccountCustomField(int id, string name)
    {
        Id = id;
        Name = name;
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
        return obj is MyAccountCustomField other && Equals(other);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(MyAccountCustomField other)
    {
        if (other == null) return false;
        return base.Equals(other)
               && string.Equals(Value, other.Value, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(Value, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(MyAccountCustomField left, MyAccountCustomField right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(MyAccountCustomField left, MyAccountCustomField right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[MyAccountCustomField: Id={Id.ToInvariantString()}, Name={Name}, Value: {Value}]";

}