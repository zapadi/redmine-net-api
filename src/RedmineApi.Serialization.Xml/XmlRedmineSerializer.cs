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
using System.IO;
using System.Text;
using System.Xml;
using Padi.RedmineApi.Exceptions;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Serialization.Xml.Extensions;
using Padi.RedmineApi.Types;
using StringReader = System.IO.StringReader;

namespace Padi.RedmineApi.Serialization.Xml;

internal sealed class XmlRedmineSerializer : IRedmineSerializer
{
    private readonly XmlWriterSettings? _writerSettings;

    private static XmlWriterSettings DefaultXmlWriterSettings { get; set; } = new XmlWriterSettings
    {
        OmitXmlDeclaration = true,
        #if DEBUG
        Indent = true,
        #endif
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writerSettings"></param>
    public XmlRedmineSerializer( XmlWriterSettings? writerSettings = null)
    {
        _writerSettings = writerSettings ?? DefaultXmlWriterSettings;
    }

    public string Format { get; } = "xml";
    
    public string ContentType { get; } = "application/xml";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="RedmineSerializerException"></exception>
    public string Serialize<T>(T entity) where T : class
    {
        if (entity == null)
        {
            throw new RedmineException(RedmineApiErrorCode.SerializationError,"Could not serialize null");
        }

        var converter = XmlConverterRegistry.GetConverter<T>();
        
        try
        {
            using var utf8StringWriter = new Utf8StringWriter();
            using var xmlWriter = XmlWriter.Create(utf8StringWriter, _writerSettings);
            converter.WriteXml(xmlWriter, (T)(object)entity);
            xmlWriter.Flush();
            return utf8StringWriter.ToString();
        }
        catch (Exception ex)
        {
            throw new RedmineException(RedmineApiErrorCode.SerializationError,ex.GetBaseException().Message, ex);
        }
    }
    
    /// <summary>
    /// Deserializes XML string to an object
    /// </summary>
    /// <param name="input">The XML string to deserialize</param>
    /// <returns>An object of type T containing the deserialized data</returns>
    public T? Deserialize<T>(string input) where T : new()
    {
        //  SerializationHelper.EnsureDeserializationInputIsNotNullOrWhiteSpace(xml, nameof(xml), typeof(TOut));
    
        var converter = XmlConverterRegistry.GetConverter<T>();
        
        try
        {
            using var textReader = new StringReader(input);
            using var xmlReader = XmlTextReaderBuilder.Create(textReader);
    
            var entity = converter.ReadXml(xmlReader);

            return entity;
        }
        catch (Exception ex)
        {
            throw new RedmineException(RedmineApiErrorCode.DeserializationError,ex.GetBaseException().Message, ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public string SerializeUserId(int userId)
    {
        return $$"""{"user_id":"{{userId.ToInvariantString()}}"}""";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="response"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="RedmineSerializerException"></exception>
    public PagedResults<T> DeserializeToPagedResults<T>(string response) where T : class, new()
    {
        try
        {
            return XmlDeserializeList<T>(response, false);
        }
        catch (Exception ex)
        {
            throw new RedmineException(RedmineApiErrorCode.DeserializationError,ex.GetBaseException().Message, ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xmlResponse"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="RedmineSerializerException"></exception>
    public int Count<T>(string xmlResponse) where T : class, new()
    {
        try
        {
            var pagedResults = XmlDeserializeList<T>(xmlResponse, true);
            return pagedResults.TotalItems;
        }
        catch (Exception ex)
        {
            throw new RedmineException(RedmineApiErrorCode.DeserializationError,ex.GetBaseException().Message, ex);
        }
    }
    
    private static PagedResults<T> XmlDeserializeList<T>(string xmlResponse,  bool onlyCount = false) 
        where T : class, new()
    {
        using var stringReader = new StringReader(xmlResponse);
        using var xmlReader = XmlTextReaderBuilder.Create(stringReader);
        while (xmlReader.NodeType == XmlNodeType.None || xmlReader.NodeType == XmlNodeType.XmlDeclaration)
        {
            xmlReader.Read();
        }

        var totalItems = xmlReader.ReadAttributeAsInt(RedmineKeys.TOTAL_COUNT);

        if (onlyCount)
        {
            return new PagedResults<T>(null, totalItems, 0, 0);
        }

        var offset = xmlReader.ReadAttributeAsInt(RedmineKeys.OFFSET);
        var limit = xmlReader.ReadAttributeAsInt(RedmineKeys.LIMIT);
        
        var result = xmlReader.ReadElementContentAsCollection<T>();

        if (totalItems == 0 && result?.Count > 0)
        {
            totalItems = result.Count;
        }

        return new PagedResults<T>(result, totalItems, offset, limit);
    }
    
    private sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}