namespace Redmine.Net.Api;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICloneable<out T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal T Clone(bool resetId);
}