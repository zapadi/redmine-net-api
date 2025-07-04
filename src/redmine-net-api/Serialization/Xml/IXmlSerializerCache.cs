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
using System.Xml.Serialization;

namespace Redmine.Net.Api.Serialization.Xml
{
    internal interface IXmlSerializerCache
    {
        XmlSerializer GetSerializer(Type type, string defaultNamespace);

        XmlSerializer GetSerializer(Type type, XmlRootAttribute root);

        XmlSerializer GetSerializer(Type type, XmlAttributeOverrides overrides);

        XmlSerializer GetSerializer(Type type, Type[] types);

        XmlSerializer GetSerializer(Type type, XmlAttributeOverrides overrides, Type[] types, XmlRootAttribute root, string defaultNamespace);
    }
}