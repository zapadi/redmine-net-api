using System.IO;
using System.Xml;

namespace Redmine.Net.Api.Internals
{
    public static class XmlTextReaderBuilder
    {
#if NET20
        public static XmlReader Create(StringReader stringReader)
        {
            return XmlReader.Create(stringReader, new XmlReaderSettings()
            {
                ProhibitDtd = true,
                XmlResolver = null, 
                IgnoreComments = true,
                IgnoreWhitespace = true,
            });

        }
        
        public static XmlReader Create(string xml)
        {
            using (var stringReader = new StringReader(xml))
            {
                return XmlReader.Create(stringReader, new XmlReaderSettings()
                {
                    ProhibitDtd = true,
                    XmlResolver = null,
                    IgnoreComments = true,
                    IgnoreWhitespace = true,
                });
            }
        }
#else
        public static XmlTextReader Create(StringReader stringReader)
        {
            return new XmlTextReader(stringReader)
            {
                DtdProcessing = DtdProcessing.Prohibit, 
                XmlResolver = null, 
                WhitespaceHandling = WhitespaceHandling.None
            };
        }
        
        public static XmlTextReader Create(string xml)
        {
            using (var stringReader = new StringReader(xml))
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