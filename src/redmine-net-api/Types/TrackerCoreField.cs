using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Serialization.Json;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
    [XmlRoot(RedmineKeys.FIELD)]
    public sealed class TrackerCoreField: IXmlSerializable, IJsonSerializable, IEquatable<TrackerCoreField>
    {
        /// <summary>
        /// 
        /// </summary>
        public TrackerCoreField()
        {
        }

        internal TrackerCoreField(string name)
        {
            Name = name;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string DebuggerDisplay => $"[TrackerCoreField: Name={Name}]";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            reader.Read();
            if (reader.NodeType == XmlNodeType.Text)
            {
                Name = reader.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public void WriteJson(JsonWriter writer) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        public void ReadJson(JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    return;
                }

                if (reader.TokenType != JsonToken.PropertyName)
                {
                    continue;
                }

                switch (reader.Value)
                {
                    case RedmineKeys.PERMISSION: Name = reader.ReadAsString(); break;
                    default: reader.Read(); break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TrackerCoreField other)
        {
            return other != null && string.Equals(Name, other.Name, StringComparison.Ordinal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as TrackerCoreField);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 17;
                hashCode = HashCodeHelper.GetHashCode(Name, hashCode);
                return hashCode;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(TrackerCoreField left, TrackerCoreField right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(TrackerCoreField left, TrackerCoreField right)
        {
            return !Equals(left, right);
        }
    }
}