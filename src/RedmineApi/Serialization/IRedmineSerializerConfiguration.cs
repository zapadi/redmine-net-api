namespace Padi.RedmineApi.Serialization;

/// <summary>
/// Defines a configuration mechanism for Redmine serializers
/// </summary>
public interface IRedmineSerializerConfiguration
{
    /// <summary>
    /// Gets the serializer factory function
    /// </summary>
    /// <returns>A function that creates a new serializer instance</returns>
    IRedmineSerializer CreateSerializer();
}
