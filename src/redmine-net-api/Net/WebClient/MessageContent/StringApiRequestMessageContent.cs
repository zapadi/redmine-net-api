/*
   Copyright 2011 - 2023 Adrian Popescu

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
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Net.WebClient.MessageContent;

internal sealed class StringApiRequestMessageContent : ByteArrayApiRequestMessageContent
{
    private static readonly Encoding DefaultStringEncoding = Encoding.UTF8;

    public StringApiRequestMessageContent(string content, string mediaType) : this(content, mediaType, DefaultStringEncoding)
    {
    }

    public StringApiRequestMessageContent(string content, string mediaType, Encoding encoding) : base(GetContentByteArray(content, encoding))
    {
        ContentType = mediaType;
    }

    private static byte[] GetContentByteArray(string content, Encoding encoding)
    {
        ArgumentNullThrowHelper.ThrowIfNull(content, nameof(content));
        return (encoding ?? DefaultStringEncoding).GetBytes(content);
    }
}