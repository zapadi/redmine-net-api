using System;

#if NETFRAMEWORK || !NET7_0_OR_GREATER
#nullable disable
namespace Padi.RedmineApi.Features.CodeAnalysis;

/// <summary>Specifies that the attributed code should be excluded from code coverage information. </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Event | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
public sealed class ExcludeFromCodeCoverageAttribute : Attribute
{
}
#endif