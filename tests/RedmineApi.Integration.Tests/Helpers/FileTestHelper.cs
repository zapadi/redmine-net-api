using Padi.RedmineApi;
using Padi.RedmineApi.Net;
using Padi.RedmineApi.Types;

namespace Padi.RedmineAPI.Integration.Tests.Helpers;

internal static class FileTestHelper
{
    private static (string fileNameame, byte[] fileContent) GenerateFile(int sizeInKb)
    {
        var fileName = RandomHelper.GenerateText("test-file", 7);
        var fileContent = sizeInKb >= 1024 
            ? FileGeneratorHelper.GenerateRandomTextFileBytes(sizeInKb) 
            : FileGeneratorHelper.GenerateRandomFileBytes(sizeInKb);
        
        return (fileName, fileContent);
    }
    public static Upload UploadRandomFile(IRedmineManager client, int sizeInKb, RequestOptions? options = null)
    {
        var (fileName, fileContent) = GenerateFile(sizeInKb);
        return client.UploadFile(fileContent, fileName);
    }

    /// <summary>
    /// Helper method to upload a 500KB file.
    /// </summary>
    /// <param name="client">The Redmine API client.</param>
    /// <param name="options">Request options.</param>
    /// <returns>API response message containing the uploaded file information.</returns>
    public static Upload UploadRandom500KbFile(IRedmineManager client, RequestOptions? options = null)
    {
        return UploadRandomFile(client, 500, options);
    }

    /// <summary>
    /// Helper method to upload a 1MB file.
    /// </summary>
    /// <param name="client">The Redmine API client.</param>
    /// <param name="options">Request options.</param>
    /// <returns>API response message containing the uploaded file information.</returns>
    public static Upload UploadRandom1MbFile(IRedmineManager client, RequestOptions? options = null)
    {
        return UploadRandomFile(client, 1024, options);
    }

    public static async Task<Upload> UploadRandomFileAsync(IRedmineManagerAsync client, int sizeInKb, RequestOptions? options = null)
    {
        var (fileName, fileContent) = GenerateFile(sizeInKb);
        
        return await client.UploadFileAsync(fileContent, fileName, options);
    }

    /// <summary>
    /// Helper method to upload a 500KB file.
    /// </summary>
    /// <param name="client">The Redmine API client.</param>
    /// <param name="options">Request options.</param>
    /// <returns>API response message containing the uploaded file information.</returns>
    public static Task<Upload> UploadRandom500KbFileAsync(IRedmineManagerAsync client, RequestOptions? options = null)
    {
        return UploadRandomFileAsync(client, 500, options);
    }

    /// <summary>
    /// Helper method to upload a 1MB file.
    /// </summary>
    /// <param name="client">The Redmine API client.</param>
    /// <param name="options">Request options.</param>
    /// <returns>API response message containing the uploaded file information.</returns>
    public static Task<Upload> UploadRandom1MbFileAsync(IRedmineManagerAsync client, RequestOptions? options = null)
    {
        return UploadRandomFileAsync(client, 1024, options);
    }
    
}