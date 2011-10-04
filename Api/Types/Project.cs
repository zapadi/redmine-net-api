using System;
using System.Xml;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    [Serializable]
    [XmlRoot("project")]
    public class Project : IdentifiableName
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [XmlElement("identifier")]
        public String Identifier { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [XmlElement("description")]
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        [XmlElement("parent")]
        public IdentifiableName Parent { get; set; }

        /// <summary>
        /// Gets or sets the home page.
        /// </summary>
        /// <value>The home page.</value>
        [XmlElement("homepage")]
        public String HomePage { get; set; }

        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>The created on.</value>
        [XmlElement("created_on")]
        public DateTime? CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the updated on.
        /// </summary>
        /// <value>The updated on.</value>
        [XmlElement("updated_on")]
        public DateTime? UpdatedOn { get; set; }

        public override void ReadXml(XmlReader reader)
        {
            reader.Read();
            while (!reader.EOF)
            {
                if (reader.IsEmptyElement && !reader.HasAttributes)
                {
                    reader.Read();
                    continue;
                }

                switch (reader.Name)
                {
                    case "id": Id = reader.ReadElementContentAsInt();
                        break;

                    case "name": Name = reader.ReadElementContentAsString(); break;

                    case "identifier": Identifier = reader.ReadElementContentAsString(); break;

                    case "description": Description = reader.ReadElementContentAsString(); break;

                    case "parent": Parent = new IdentifiableName(reader); break;

                    case "homepage": HomePage = reader.ReadElementContentAsString(); break;

                    case "created_on": CreatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    case "updated_on": UpdatedOn = reader.ReadElementContentAsNullableDateTime(); break;

                    default:
                        reader.Read();
                        break;
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("id", Id.ToString());
            writer.WriteElementString("name", Name);
            writer.WriteElementString("identifier", Identifier);
            writer.WriteElementString("description", Description);
            writer.WriteElementString("homepage", HomePage);
            writer.WriteElementString("parent_id", Parent != null ? Parent.Id.ToString() : "");
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var project = obj as Project;
            return project.Identifier.Equals(Identifier) && project.Name.Equals(Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Identifier.GetHashCode();
        }
    }
}