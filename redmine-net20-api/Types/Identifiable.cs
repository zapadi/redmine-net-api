/*
   Copyright 2011 - 2015 Adrian Popescu

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

using System.Xml.Serialization;

namespace Redmine.Net.Api.Types
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Identifiable<T> where T : Identifiable<T>
    {
        private int? oldHashCode;

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as T;
            if (other == null) return false;

            var thisIsNew = Equals(Id, default(int));
            var otherIsNew = Equals(other.Id, default(int));

            if (thisIsNew && otherIsNew)
                return ReferenceEquals(this, other);

            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            if (oldHashCode.HasValue)
                return oldHashCode.Value;

            var thisIsNew = Equals(Id, default(int));
            if (thisIsNew)
            {
                oldHashCode = base.GetHashCode();
                return oldHashCode.Value;
            }
            return Id.GetHashCode();
        }

        public static bool operator ==(Identifiable<T> left, Identifiable<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Identifiable<T> left, Identifiable<T> right)
        {
            return !Equals(left, right);
        }
    }
}