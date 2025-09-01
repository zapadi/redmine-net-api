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
public class IdentifiableName : Identifiable<IdentifiableName>
    , ICloneable<IdentifiableName>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static T Create<T>(int id) where T: IdentifiableName, new()
    { 
        return new T 
        {
            Id = id
        };
    }
      
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Create<T>(int id, string name) where T: IdentifiableName, new()
    { 
        return new T 
        {
            Id = id,
            Name = name
        };
    }
        
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentifiableName"/> class.
    /// </summary>
    public IdentifiableName() { }

    /// <summary>
    /// Initializes the class by using the given Id and Name.
    /// </summary>
    /// <param name="id">The Id.</param>
    /// <param name="name">The Name.</param>
    public IdentifiableName(int id, string name)
    {
        Id = id;
        Name = name;
    }

    #region Properties
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }
    #endregion

    #region Implementation of IEquatable<IdentifiableName>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(IdentifiableName other)
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
        return Equals(obj as IdentifiableName);
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
    public static bool operator ==(IdentifiableName left, IdentifiableName right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(IdentifiableName left, IdentifiableName right)
    {
        return !Equals(left, right);
    }
    #endregion
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifiableName"></param>
    /// <returns></returns>
    public static implicit operator string(IdentifiableName identifiableName) => FromIdentifiableName(identifiableName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="identifiableName"></param>
    /// <returns></returns>
    public static string FromIdentifiableName(IdentifiableName identifiableName)
    {
        return identifiableName?.Id.ToInvariantString();
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[IdentifiableName: Id={Id.ToInvariantString()}, Name={Name}]";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public new IdentifiableName Clone(bool resetId)
    {
        return new IdentifiableName
        {
            Id = Id,
            Name = Name
        };
    }
}