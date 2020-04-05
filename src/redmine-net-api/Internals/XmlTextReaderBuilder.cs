using System.IO;
using System.Xml;

namespace Redmine.Net.Api.Internals
{
    /// <summary>
    /// 
    /// </summary>
    public static class XmlTextReaderBuilder
    {
#if NET20
        private static readonly XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
        {
            ProhibitDtd = true,
            XmlResolver = null,
            IgnoreComments = true,
            IgnoreWhitespace = true,
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringReader"></param>
        /// <returns></returns>
        public static XmlReader Create(StringReader stringReader)
        {
            return XmlReader.Create(stringReader, xmlReaderSettings);

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static XmlReader Create(string xml)
        {
            var stringReader = new StringReader(xml);
            {
                return XmlReader.Create(stringReader, xmlReaderSettings);
            }
        }
#else
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringReader"></param>
        /// <returns></returns>
        public static XmlTextReader Create(StringReader stringReader)
        {
            return new XmlTextReader(stringReader)
            {
                DtdProcessing = DtdProcessing.Prohibit, 
                XmlResolver = null, 
                WhitespaceHandling = WhitespaceHandling.None
            };
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static XmlTextReader Create(string xml)
        {
            var stringReader = new StringReader(xml);
            {
                return new XmlTextReader(stringReader)
                {
                    DtdProcessing = DtdProcessing.Prohibit,
                    XmlResolver = null,
                    WhitespaceHandling = WhitespaceHandling.None
                };
            }
        }
#endif
    }
}