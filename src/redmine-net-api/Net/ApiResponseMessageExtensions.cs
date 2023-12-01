using System.Collections.Generic;
using System.Text;
using Redmine.Net.Api.Serialization;

namespace Redmine.Net.Api.Net;

internal static class ApiResponseMessageExtensions
{
    internal static T DeserializeTo<T>(this ApiResponseMessage responseMessage, IRedmineSerializer redmineSerializer) where T : new()
    {
        if (responseMessage?.Content == null)
        {
            return default;
        }
            
        var responseAsString = Encoding.UTF8.GetString(responseMessage.Content);
            
        return redmineSerializer.Deserialize<T>(responseAsString);
    }
        
    internal static PagedResults<T> DeserializeToPagedResults<T>(this ApiResponseMessage responseMessage, IRedmineSerializer redmineSerializer) where T : class, new()
    {
        if (responseMessage?.Content == null)
        {
            return default;
        }
            
        var responseAsString = Encoding.UTF8.GetString(responseMessage.Content);
            
        return redmineSerializer.DeserializeToPagedResults<T>(responseAsString);
    }
        
    internal static List<T> DeserializeToList<T>(this ApiResponseMessage responseMessage, IRedmineSerializer redmineSerializer) where T : class, new()
    {
        if (responseMessage?.Content == null)
        {
            return default;
        }
            
        var responseAsString = Encoding.UTF8.GetString(responseMessage.Content);
            
        return redmineSerializer.Deserialize<List<T>>(responseAsString);
    }
}