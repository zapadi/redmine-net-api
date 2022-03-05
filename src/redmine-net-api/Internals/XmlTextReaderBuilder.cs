/*
   Copyright 2011 - 2022 Adrian Popescu

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
        private static readonly XmlReaderSettings XmlReaderSettings = new XmlReaderSettings()
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
            return XmlReader.Create(stringReader, XmlReaderSettings);

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
                return XmlReader.Create(stringReader, XmlReaderSettings);
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