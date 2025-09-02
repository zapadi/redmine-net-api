namespace Padi.RedmineApi.HttpClient;

/// <summary>
/// A <see langword="struct"/> that contains info on a pending download
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="bytes"></param>
/// <param name="percentage"></param>
public readonly struct HttpProgress(long bytes, int percentage)
{
    /// <summary>
    /// Gets the total number of downloaded bytes
    /// </summary>
    public long DownloadedBytes { get; } = bytes;

    /// <summary>
    /// Gets the current download percentage
    /// </summary>
    public int Percentage { get; } = percentage;
}