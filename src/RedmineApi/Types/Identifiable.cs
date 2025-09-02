﻿/*
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
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public abstract class Identifiable<T> : IEquatable<T>, ICloneable<Identifiable<T>> 
    where T : Identifiable<T>
{
    #region Properties
    /// <summary>
    /// Gets the id.
    /// </summary>
    /// <value>The id.</value>
    public int Id { get; set; }
    #endregion

    #region Implementation of IEquatable<Identifiable<T>>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Identifiable<T> other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Id == other.Id;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(T other)
    {
        if (other == null) return false;
        return Id == other.Id;
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
        return Equals(obj as Identifiable<T>);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = 17;
        hashCode = HashCodeHelper.GetHashCode(Id, hashCode);
        return hashCode;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Identifiable<T> left, Identifiable<T> right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Identifiable<T> left, Identifiable<T> right)
    {
        return !Equals(left, right);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"Id={Id.ToInvariantString()}";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual Identifiable<T> Clone(bool resetId)
    {
        throw new NotImplementedException();
    }
}