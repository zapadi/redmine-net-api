using System;

namespace Redmine.Net.Api.Http.Helpers;

internal static class ClientHelper
{
    internal static void ReportProgress(IProgress<int>progress, long total, long bytesRead)
    {
        if (progress == null || total <= 0)
        {
            return;
        }
        var percent = (int)(bytesRead * 100L / total);
        progress.Report(percent);
    }
}