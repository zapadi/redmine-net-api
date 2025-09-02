using System;
using Padi.RedmineApi.Builders;

namespace Padi.RedmineApi.WebClient;

/// <summary>
/// 
/// </summary>
public static class RedmineManagerOptionBuilderExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    /// <summary>
    /// Configures the Redmine client to use WebClient with options
    /// </summary>
    public static RedmineManagerOptionsBuilder UseWebClient(this RedmineManagerOptionsBuilder builder, Action<WebClientApiOptions>? configure = null)
    {
        var factory = new WebClientFactory(configure);
        builder.WithClientFactory(factory);
        return builder;
    }
    
    /// <summary>
    /// Configures the Redmine client to use a custom WebClient factory
    /// </summary>
    public static RedmineManagerOptionsBuilder UseWebClient(this RedmineManagerOptionsBuilder builder, Func<System.Net.WebClient> webClientFactory)
    {
        var factory = new WebClientFactory(webClientFactory);
        builder.WithClientFactory(factory);
        return builder;
    }
}