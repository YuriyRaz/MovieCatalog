using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MovieCatalog.Models
{
    public class Role : IEquatable<Role>
    {
        public string RoleName { get; set; }
        public ActorShortDetail Actor { get; set; }

        public override bool Equals( object obj )
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Role)) return false;
            return Equals((Role) obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Role other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.RoleName, RoleName) && Equals(other.Actor, Actor);
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
                return ((RoleName != null ? RoleName.GetHashCode() : 0)*397) ^ (Actor != null ? Actor.GetHashCode() : 0);
            }
        }
    }
}