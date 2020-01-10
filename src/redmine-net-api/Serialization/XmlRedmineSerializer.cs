using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Serialization
{
    internal sealed class XmlRedmineSerializer : IRedmineSerializer
    {

        public XmlRedmineSerializer()
        {
            XMLWriterSettings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            };
        }

        public XmlRedmineSerializer(XmlWriterSettings xmlWriterSettings)
        {
            XMLWriterSettings = xmlWriterSettings;
        }

        private readonly XmlWriterSettings XMLWriterSettings;

        public T Deserialize<T>(string response) where T : new()
        {
            try
            {
                return XmlDeserializeEntity<T>(response);
            }
            catch (Exception ex)
            {
                throw new RedmineException(ex.Message, ex);
            }
        }

        public PagedResults<T> DeserializeToPagedResults<T>(string response) where T : class, new()
        {
            try
            {
                return XmlDeserializeList<T>(response, false);
            }
            catch (Exception ex)
            {
                throw new RedmineException(ex.Message, ex);
            }
        }

        public int Count<T>(string xmlResponse) where T : class, new()
        {
            try
            {
                var pagedResults = XmlDeserializeList<T>(xmlResponse, true);
                return pagedResults.TotalItems;
            }
            catch (Exception ex)
            {
                throw new RedmineException(ex.Message, ex);
            }
        }

        public string Type { get; } = "xml";

        public string Serialize<T>(T entity) where T : class
        {
            try
            {
                return ToXML(entity);
            }
            catch (Exception ex)
            {
                throw new RedmineException(ex.Message, ex);
            }
        }

        /// <summary>
        ///     XMLs the deserialize list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlResponse">The response.</param>
        /// <param name="onlyCount"></param>
        /// <returns></returns>
        private static PagedResults<T> XmlDeserializeList<T>(string xmlResponse, bool onlyCount) where T : class, new()
        {
            if (xmlResponse.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(xmlResponse), $"Could not deserialize null or empty input for type '{typeof(T).Name}'.");
            }

            using (TextReader stringReader = new StringReader(xmlResponse))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    xmlReader.Read();
                    xmlReader.Read();

                    var totalItems = xmlReader.ReadAttributeAsInt(RedmineKeys.TOTAL_COUNT);
                    if (onlyCount)
                    {
                        return new PagedResults<T>(null, totalItems, 0, 0);
                    }
                    var offset = xmlReader.ReadAttributeAsInt(RedmineKeys.OFFSET);
                    var limit = xmlReader.ReadAttributeAsInt(RedmineKeys.LIMIT);
                    var result = xmlReader.ReadElementContentAsCollection<T>();

                    return new PagedResults<T>(result, totalItems, offset, limit);
                }
            }
        }

        /// <summary>
        ///     Serializes the specified System.Object and writes the XML document to a string.
        /// </summary>
        /// <typeparam name="T">The type of objects to serialize.</typeparam>
        /// <param name="entity">The object to serialize.</param>
        /// <returns>
        ///     The System.String that contains the XML document.
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        // ReSharper disable once InconsistentNaming
        private string ToXML<T>(T entity) where T : class
        {
            if (entity == default(T))
            {
                throw new ArgumentNullException(nameof(entity), $"Could not serialize null of type {typeof(T).Name}");
            }

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, XMLWriterSettings))
                {
                    var serializer = new XmlSerializer(typeof(T));

                    serializer.Serialize(xmlWriter, entity);

                    return stringWriter.ToString();
                }
            }
        }

        /// <summary>
        ///     Deserializes the XML document contained by the specific System.String.
        /// </summary>
        /// <typeparam name="TOut">The type of objects to deserialize.</typeparam>
        /// <param name="xml">The System.String that contains the XML document to deserialize.</param>
        /// <returns>
        ///     The T object being deserialized.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        ///     An error occurred during deserialization. The original exception is available
        ///     using the System.Exception.InnerException property.
        /// </exception>
        // ReSharper disable once InconsistentNaming
        private TOut XmlDeserializeEntity<TOut>(string xml) where TOut : new()
        {
            if (xml.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(xml), $"Could not deserialize null or empty input for type '{typeof(TOut).Name}'.");
            }

            using (var textReader = new StringReader(xml))
            {
                using (var xmlReader = XmlTextReaderBuilder.Create(textReader))
                {
                    var serializer = new XmlSerializer(typeof(TOut));

                    var entity = serializer.Deserialize(xmlReader);

                    if (entity is TOut t)
                    {
                        return t;
                    }

                    return default;
                }
            }
        }
    }
}