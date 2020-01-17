using System;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Serialization
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