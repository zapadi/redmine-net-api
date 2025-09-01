using System;
using System.Diagnostics;
using Padi.RedmineApi.Internals;

namespace Padi.RedmineApi.Types;

/// <summary>
/// 
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed class TrackerCoreField: IEquatable<TrackerCoreField>
{
    /// <summary>
    /// 
    /// </summary>
    public TrackerCoreField()
    {
            
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    internal TrackerCoreField(string name)
    {
        Name = name;
    }
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; }
        
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private string DebuggerDisplay => $"[TrackerCoreField: Name={Name}]";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(TrackerCoreField other)
    {
        return other != null && string.Equals(Name, other.Name, StringComparison.Ordinal);
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
        return Equals(obj as TrackerCoreField);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var hashCode = 17;
        hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
        return hashCode;
    }
        
    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(TrackerCoreField left, TrackerCoreField right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(TrackerCoreField left, TrackerCoreField right)
    {
        return !Equals(left, right);
    }
}