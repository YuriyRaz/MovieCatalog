using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace MovieCatalog.Models
{
    public class Actor : IEquatable<Actor>
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }

        public override bool Equals( object obj )
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Actor)) return false;
            return Equals((Actor) obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Actor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Id.Equals(Id) /* && Equals(other.Name, Name) && Equals(other.Biography, Biography)*/;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = Id.GetHashCode();
                //result = (result*397) ^ (Name != null ? Name.GetHashCode() : 0);
                //result = (result*397) ^ (Biography != null ? Biography.GetHashCode() : 0);
                return result;
            }
        }
    }
}