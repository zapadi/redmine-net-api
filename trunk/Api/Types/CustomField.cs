/*
   Copyright 2011 Dorin Huzum, Adrian Popescu.

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
using System.Xml;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [Serializable]
    [XmlRoot("custom_field")]
    public class CustomField : IdentifiableName, IEquatable<CustomField>
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlText]
        public String Value { get; set; }

        public override void ReadXml(XmlReader reader)
        {
            Id = Convert.ToInt32(reader.GetAttribute("id"));
            Name = reader.GetAttribute("name");
            Value = reader.ReadContentAsString();
        }

        public bool Equals(CustomField other)
        {
            if (other == null) return false;
            return (Id == other.Id && Name == other.Name && Value == other.Value);
        }
    }
}