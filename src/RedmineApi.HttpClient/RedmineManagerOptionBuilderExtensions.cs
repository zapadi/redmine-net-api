using System;
using Padi.RedmineApi.Builders;

namespace Padi.RedmineApi.HttpClient;

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
    public static RedmineManagerOptionsBuilder UseHttpClient(this RedmineManagerOptionsBuilder builder, Action<HttpClientApiOptions>? configure = null)
    {
        var options = new HttpClientApiOptions();
        configure?.Invoke(options);
        builder.WithClientFactory(new HttpClientFactory(options));
        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="httpClientFactory"></param>
    /// <returns></returns>
    public static RedmineManagerOptionsBuilder UseHttpClient(this RedmineManagerOptionsBuilder builder, Func<System.Net.Http.HttpClient> httpClientFactory)
    {
        //TODO use the client provided by the user without applying any option configuration (split httpclientapioptions & transporter options in two and accept only httpclientoptions)
        return builder;
    }
}