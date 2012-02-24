using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace MovieCatalog.Models
{
    public class MovieShortDetail : IEquatable<MovieShortDetail>
    {
        public MovieShortDetail() {}
        public MovieShortDetail(Movie movie)
        {
            MovieId = movie.Id;
            Name = movie.Name;
        }
        public ObjectId MovieId { get; set; }
        public string Name { get; set; }

        public override bool Equals( object obj )
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MovieShortDetail)) return false;
            return Equals((MovieShortDetail) obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(MovieShortDetail other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.MovieId.Equals(MovieId) /*&& Equals(other.Name, Name)*/;
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
                return MovieId.GetHashCode();
                //return (MovieId.GetHashCode()*397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }
    }
}