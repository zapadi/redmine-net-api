namespace Padi.RedmineAPI.Integration.Tests.Infrastructure.Options;

/// <summary>
/// Enum defining how containers should be managed
/// </summary>
public enum TestContainerMode
{
    /// <summary>Use existing running containers at specified URL</summary>
    UseExisting,
        
    /// <summary>Create new containers with random ports (CI-friendly)</summary>
    CreateNewWithRandomPorts
}