/*
   Copyright 2011 - 2025 Adrian Popescu

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Text;
using Padi.RedmineApi.Internals;

namespace Padi.RedmineApi.Net;


/// <summary>
/// 
/// </summary>
public sealed class ApiRequestContent : IDisposable
{
    private static readonly byte[] _emptyByteArray = [];
    
    /// <summary>
    /// Gets the content type of the request.
    /// </summary>
    public string ContentType { get; }

    /// <summary>
    /// Gets the body of the request.
    /// </summary>
    public byte[] Body { get; }
    
    /// <summary>
    /// Gets the length of the request body.
    /// </summary>
    public int Length => Body?.Length ?? 0;

    /// <summary>
    /// Creates a new instance of RedmineApiRequestContent.
    /// </summary>
    /// <param name="contentType">The content type of the request.</param>
    /// <param name="body">The body of the request.</param>
    /// <exception cref="ArgumentNullException">Thrown when the contentType is null.</exception>
    public ApiRequestContent(string contentType, byte[] body)
    {
        ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        Body = body ?? _emptyByteArray;
    }
    
    /// <summary>
    /// Creates a text-based request content with the specified MIME type.
    /// </summary>
    /// <param name="text">The text content.</param>
    /// <param name="mimeType">The MIME type of the content.</param>
    /// <param name="encoding">The encoding to use (defaults to UTF-8).</param>
    /// <returns>A new RedmineApiRequestContent instance.</returns>
    public static ApiRequestContent CreateString(string text, string mimeType, Encoding encoding = null)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new ApiRequestContent(mimeType, _emptyByteArray);
        }
        
        encoding ??= Encoding.UTF8;
        return new ApiRequestContent(mimeType == RedmineConstants.XML 
                ? HttpConstants.ContentTypes.ApplicationXml 
                : HttpConstants.ContentTypes.ApplicationJson
            , encoding.GetBytes(text));
    }
    
    /// <summary>
    /// Creates a binary request content.
    /// </summary>
    /// <param name="data">The binary data.</param>
    /// <returns>A new RedmineApiRequestContent instance.</returns>
    public static ApiRequestContent CreateBinary(byte[] data)
    {
        return new ApiRequestContent(HttpConstants.ContentTypes.ApplicationOctetStream, data);
    }
    
    /// <summary>
    /// Disposes the resources used by this instance.
    /// </summary>
    public void Dispose()
    {
    }
}