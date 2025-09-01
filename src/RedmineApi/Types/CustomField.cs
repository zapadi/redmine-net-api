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
/// 
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class CustomField : IdentifiableName, IEquatable<CustomField>
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public string CustomizedType { get; set; }

    /// <summary>
    /// Added in Redmine 5.1.0 version
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string FieldFormat { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Regexp { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? MinLength { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsFilter { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool Searchable { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool Multiple { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string DefaultValue { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public List<CustomFieldPossibleValue> PossibleValues { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public List<TrackerCustomField> Trackers { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public List<CustomFieldRole> Roles { get; set; }

    #endregion

    #region Implementation of IEquatable<CustomField>

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(CustomField other)
    {
        if (other == null) return false;

        var result = base.Equals(other)
                     && string.Equals(CustomizedType, other.CustomizedType, StringComparison.Ordinal)
                     && string.Equals(Description, other.Description, StringComparison.Ordinal)
                     && string.Equals(FieldFormat, other.FieldFormat, StringComparison.Ordinal)
                     && string.Equals(Regexp, other.Regexp, StringComparison.Ordinal)
                     && string.Equals(DefaultValue, other.DefaultValue, StringComparison.Ordinal)
                     && MinLength == other.MinLength
                     && MaxLength == other.MaxLength
                     && IsRequired == other.IsRequired
                     && IsFilter == other.IsFilter
                     && Searchable == other.Searchable
                     && Multiple == other.Multiple
                     && Visible == other.Visible
                     && (PossibleValues?.Equals<CustomFieldPossibleValue>(other.PossibleValues) ?? other.PossibleValues == null)
                     && (Trackers?.Equals<TrackerCustomField>(other.Trackers) ?? other.Trackers == null)
                     && (Roles?.Equals<CustomFieldRole>(other.Roles) ?? other.Roles == null);
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
        return Equals(obj as CustomField);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(CustomizedType, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Description, hashCode);
        hashCode = HashCodeHelper.GetHashCode(FieldFormat, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Regexp, hashCode);
        hashCode = HashCodeHelper.GetHashCode(MinLength, hashCode);
        hashCode = HashCodeHelper.GetHashCode(MaxLength, hashCode);
        hashCode = HashCodeHelper.GetHashCode(IsRequired, hashCode);
        hashCode = HashCodeHelper.GetHashCode(IsFilter, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Searchable, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Multiple, hashCode);
        hashCode = HashCodeHelper.GetHashCode(DefaultValue, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Visible, hashCode);
        hashCode = HashCodeHelper.GetHashCode(PossibleValues, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Trackers, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Roles, hashCode);
        return hashCode;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(CustomField left, CustomField right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(CustomField left, CustomField right)
    {
        return !Equals(left, right);
    }

    #endregion

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[CustomField: Id={Id.ToInvariantString()}, Name={Name}]";
}