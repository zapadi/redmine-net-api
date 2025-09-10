using System.Text;
using File = System.IO.File;

namespace Padi.RedmineAPI.Integration.Tests.Helpers;

internal static class FileGeneratorHelper
{
    private static readonly string[] Extensions = [".txt", ".doc", ".pdf", ".xml", ".json"];

    /// <summary>
    /// Generates random file content with a specified size.
    /// </summary>
    /// <param name="sizeInKb">Size of the file in kilobytes.</param>
    /// <returns>Byte array containing the file content.</returns>
    public static byte[] GenerateRandomFileBytes(int sizeInKb)
    {
        var sizeInBytes = sizeInKb * 1024;
        var bytes = new byte[sizeInBytes];
        RandomHelper.FillRandomBytes(bytes);
        return bytes;
    }

    /// <summary>
    /// Generates a random text file with a specified size.
    /// </summary>
    /// <param name="sizeInKb">Size of the file in kilobytes.</param>
    /// <returns>Byte array containing the text file content.</returns>
    public static byte[] GenerateRandomTextFileBytes(int sizeInKb)
    {
        var roughCharCount = sizeInKb * 1024;
        
        var sb = new StringBuilder(roughCharCount);
        
        while (sb.Length < roughCharCount)
        {
            sb.AppendLine(RandomHelper.GenerateText(RandomHelper.GetRandomNumber(5, 80)));
        }
        
        var text = sb.ToString();

        if (text.Length > roughCharCount)
        {
            text = text[..roughCharCount];
        }
        
        return Encoding.UTF8.GetBytes(text);
    }

    /// <summary>
    /// Creates a random file with a specified size and returns its path.
    /// </summary>
    /// <param name="sizeInKb">Size of the file in kilobytes.</param>
    /// <param name="useTextContent">If true, generates text content; otherwise, generates binary content.</param>
    /// <returns>Path to the created temporary file.</returns>
    public static string CreateRandomFile(int sizeInKb, bool useTextContent = true)
    {
        var extension = Extensions[RandomHelper.GetRandomNumber(Extensions.Length)];
        var fileName = RandomHelper.GenerateText("test-file", 7);
        var filePath = Path.Combine(Path.GetTempPath(), $"{fileName}{extension}");
        
        var content = useTextContent 
            ? GenerateRandomTextFileBytes(sizeInKb)
            : GenerateRandomFileBytes(sizeInKb);
        
        File.WriteAllBytes(filePath, content);
        return filePath;
    }

}