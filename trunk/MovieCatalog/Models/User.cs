using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieCatalog.Models
{
    public class User
    {
        private ICollection<Friend> _friends;
        private ICollection<MovieShortDetail> _favoriteMovies;
        private ICollection<MovieShortDetail> _friendsFavoriteMovies;

        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        [BsonRepresentation( BsonType.String )]
        public Gender Gender { get; set; }

        public int Age { get; set; }

        public ICollection<Friend> Friends
        {
            get { return _friends ?? (_friends = new HashSet<Friend>()); }
            set { _friends = new HashSet<Friend>( value ?? new Friend[] { } ); }
        }

        public ICollection<MovieShortDetail> FavoriteMovies
        {
            get { return _favoriteMovies ?? (_favoriteMovies = new HashSet<MovieShortDetail>()); }
            set { _favoriteMovies = new HashSet<MovieShortDetail>( value ?? new MovieShortDetail[] { } ); }
        }

        public ICollection<MovieShortDetail> FriendsFavoriteMovies
        {
            get { return _friendsFavoriteMovies ?? (_friendsFavoriteMovies = new HashSet<MovieShortDetail>()); }
            set { _friendsFavoriteMovies = new HashSet<MovieShortDetail>( value ?? new MovieShortDetail[] { } ); }
        }
    }

}