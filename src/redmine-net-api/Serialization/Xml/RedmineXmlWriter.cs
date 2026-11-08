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
using System.Xml;

namespace Redmine.Net.Api.Serialization
{
    internal sealed class RedmineXmlWriter : XmlWriter
    {
        private readonly XmlWriter writer;

        internal RedmineXmlWriter(TextWriter output, XmlWriterSettings settings, int timeEntryCommentsMaxLength)
        {
            writer = Create(output, settings);
            TimeEntryCommentsMaxLength = timeEntryCommentsMaxLength;
        }

        internal int TimeEntryCommentsMaxLength { get; }

        public override XmlWriterSettings Settings => writer.Settings;

        public override WriteState WriteState => writer.WriteState;

        public override XmlSpace XmlSpace => writer.XmlSpace;

        public override string XmlLang => writer.XmlLang;

        public override void Flush() => writer.Flush();

        public override void Close() => writer.Close();

        public override string LookupPrefix(string ns) => writer.LookupPrefix(ns);

        public override void WriteBase64(byte[] buffer, int index, int count) => writer.WriteBase64(buffer, index, count);

        public override void WriteCData(string text) => writer.WriteCData(text);

        public override void WriteCharEntity(char ch) => writer.WriteCharEntity(ch);

        public override void WriteChars(char[] buffer, int index, int count) => writer.WriteChars(buffer, index, count);

        public override void WriteComment(string text) => writer.WriteComment(text);

        public override void WriteDocType(string name, string pubid, string sysid, string subset) => writer.WriteDocType(name, pubid, sysid, subset);

        public override void WriteEndAttribute() => writer.WriteEndAttribute();

        public override void WriteEndDocument() => writer.WriteEndDocument();

        public override void WriteEndElement() => writer.WriteEndElement();

        public override void WriteEntityRef(string name) => writer.WriteEntityRef(name);

        public override void WriteFullEndElement() => writer.WriteFullEndElement();

        public override void WriteProcessingInstruction(string name, string text) => writer.WriteProcessingInstruction(name, text);

        public override void WriteRaw(char[] buffer, int index, int count) => writer.WriteRaw(buffer, index, count);

        public override void WriteRaw(string data) => writer.WriteRaw(data);

        public override void WriteStartAttribute(string prefix, string localName, string ns) => writer.WriteStartAttribute(prefix, localName, ns);

        public override void WriteStartDocument() => writer.WriteStartDocument();

        public override void WriteStartDocument(bool standalone) => writer.WriteStartDocument(standalone);

        public override void WriteStartElement(string prefix, string localName, string ns) => writer.WriteStartElement(prefix, localName, ns);

        public override void WriteString(string text) => writer.WriteString(text);

        public override void WriteSurrogateCharEntity(char lowChar, char highChar) => writer.WriteSurrogateCharEntity(lowChar, highChar);

        public override void WriteWhitespace(string ws) => writer.WriteWhitespace(ws);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((IDisposable)writer).Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
