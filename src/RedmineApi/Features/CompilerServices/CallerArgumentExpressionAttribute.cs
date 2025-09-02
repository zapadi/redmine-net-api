#if NETFRAMEWORK
#pragma warning disable
#nullable enable annotations

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// An attribute that allows parameters to receive the expression of other parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="global::System.Runtime.CompilerServices.CallerArgumentExpressionAttribute"/> class.
        /// </summary>
        /// <param name="parameterName">The condition parameter value.</param>
        public CallerArgumentExpressionAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        /// <summary>
        /// Gets the parameter name the expression is retrieved from.
        /// </summary>
        public string ParameterName { get; }
    }
}
#endif