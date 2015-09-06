using System;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [XmlRoot("role")]
    public class CustomFieldRole : IdentifiableName, IEquatable<CustomFieldRole>
    {
        #region Implementation of IEquatable<CustomFieldRole>

        public bool Equals(CustomFieldRole other) { if (other == null) return false;
            return Id == other.Id && Name == other.Name;
        }

        public override string ToString()
        {
            return Id + ", " + Name;
        }

        #endregion
    }
}