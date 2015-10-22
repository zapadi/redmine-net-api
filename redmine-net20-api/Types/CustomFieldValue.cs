/*
   Copyright 2011 - 2015 Adrian Popescu, Dorin Huzum.

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
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [XmlRoot(RedmineKeys.VALUE)]
    public class CustomFieldValue : IEquatable<CustomFieldValue>
    {
        [XmlText]
        public string Info { get; set; }

        public bool Equals(CustomFieldValue other)
        {
            return Info.Equals(other.Info);
        }

        public override string ToString()
        {
            return Info;
        }
    }
}