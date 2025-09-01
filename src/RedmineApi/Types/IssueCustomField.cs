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
public sealed class IssueCustomField : IdentifiableName, IEquatable<IssueCustomField>, ICloneable<IssueCustomField>, IValue
{
    #region Properties
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    public List<CustomFieldValue> Values { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool Multiple { get; set; }
    #endregion

    #region Implementation of IEquatable<IssueCustomField>
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IssueCustomField other)
    {
        if (other == null) return false;
        return base.Equals(other)
               && Multiple == other.Multiple
               && (Values?.Equals<CustomFieldValue>(other.Values) ?? other.Values == null);
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
        return Equals(obj as IssueCustomField);
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        hashCode = HashCodeHelper.GetHashCode(Values, hashCode);
        hashCode = HashCodeHelper.GetHashCode(Multiple, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(IssueCustomField left, IssueCustomField right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(IssueCustomField left, IssueCustomField right)
    {
        return !Equals(left, right);
    }
    #endregion

    #region Implementation of IClonable<IssueCustomField>
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public new IssueCustomField Clone(bool resetId)
    {
        IssueCustomField clone;
        if (resetId)
        {
            clone = new IssueCustomField();
        }
        else
        {
            clone = new IssueCustomField
            {
                Id = Id,
            };
        }

        clone.Name = Name;
        clone.Multiple = Multiple;

        if (Values != null)
        {
            clone.Values = new List<CustomFieldValue>(Values);
        }

        return clone;
    }
        
    #endregion

    #region Implementation of IValue
    /// <summary>
    /// 
    /// </summary>
    public string Value => Id.ToInvariantString();

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string GetValue(object item)
    {
        return ((CustomFieldValue)item).Info;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[IssueCustomField: Id={Id.ToInvariantString()}, Name={Name}, Multiple={Multiple.ToInvariantString()}]";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IssueCustomField CreateSingle(int id, string name, string value)
    {
        return new IssueCustomField
        {
            Id = id,
            Name = name,
            Values = [new CustomFieldValue { Info = value }]
        };
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static IssueCustomField CreateMultiple(int id, string name, string[] values)
    {
        var isf = new IssueCustomField
        {
            Id = id,
            Name = name,
            Multiple = true,
        };

        if (values is not { Length: > 0 })
        {
            return isf;
        }
            
        isf.Values = new List<CustomFieldValue>(values.Length);
                
        foreach (var value in values)
        {
            isf.Values.Add(new CustomFieldValue { Info = value });    
        }

        return isf;
    }
}