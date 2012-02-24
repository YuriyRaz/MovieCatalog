using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MovieCatalog.Models.Interfaces
{
    public interface IRepository
    {
        IEnumerable<User> GetUserList();

        User GetUser( string userId );
        Movie GetMovie( string movieId );
        IEnumerable<MovieShortDetail> GetFriendsFavotiteMovies( User user );
        IEnumerable<UserShortDetail> GetUsersWhoLikeTheMovie( Movie movie );

        DatabaseStatistic GetDatabaseStatistic();

        MembershipCreateStatus RegisterUser( RegisterModel model, bool logon );
        

        RepositoryOperationResult CurrentGeneratorOperationResult { get; }

        RepositoryOperationResult GenerateUserRecords( long recordsNumber );
        RepositoryOperationResult GenerateFriendshipRelations( int minFriendsNumber, int maxFriendsNumber );
        RepositoryOperationResult GenerateFavoriteMovies( int minFavoriteMoviesNumber, int maxFavoriteMoviesNumber );
        RepositoryOperationResult RemoveAllUsers();

        RepositoryOperationResult GenerateMovieRecords( long recordsNumber );
        RepositoryOperationResult RemoveAllMovies();

        RepositoryOperationResult GenerateActorRecords( long recordsNumber );
        RepositoryOperationResult RemoveAllActors();

    }
}