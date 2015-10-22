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
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Identifiable<T> where T : Identifiable<T>, IEquatable<T>
    {

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [XmlAttribute(RedmineKeys.ID)]
        public int Id { get; set; }


        public bool Equals(Identifiable<T> other)
        {
            if (other == null) return false;

            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(obj as Identifiable<T>);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 13;
                hashCode = (hashCode * 397) ^ Id.GetHashCode();

                return hashCode;
            }
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