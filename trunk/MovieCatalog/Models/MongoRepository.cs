using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Profile;
using System.Web.Security;
using FluentMongo.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MovieCatalog.Models.Interfaces;

namespace MovieCatalog.Models
{
    /// <summary>
    /// Repository business logic implementation for MongoDB database.
    /// </summary>
    public class MongoRepository : IRepository
    {
        private const string IdFieldName = "_id";
        private const string FavoriteMoviesFieldName = "FavoriteMovies";
        private const string FriendsFavoriteMoviesFieldName = "FriendsFavoriteMovies";
        private const string FriendsFieldName = "Friends";
        private const string NameFieldName = "Name";
        private const string UserIdFieldName = "UserId";
        private const string MovieIdFieldName = "MovieId";
        private static readonly string FavoriteMoviesMovieIdFieldName = string.Format( "{0}.{1}", FavoriteMoviesFieldName, MovieIdFieldName );

        private const int MinRolesNumber = 3;
        private const int MaxRolesNumber = 10;

        private const string UserCollectionName = "User";
        private const string MovieCollectionName = "Movie";
        private const string ActorCollectionName = "Actor";

        private const string ConnectionStringKey = "MongoConncetion"; //"mongodb://localhost";

        private const string DefaultUserNameFormat = "User{0:0000000000000}";
        private const string DefaultUserPassword = "123456";

        private const string DefaultMovieNameFormat = "Movie{0:0000000000000}";
        private const string DefaultRoleNameFormat = "Role{0:000}";

        private const string DefaultActorNameFormat = "Actor{0:000000}";
        private const string DefaultActorBiographyTextFormat = "Biography text ({0:000000}).";

        private const string UserNamePattern = @"^User(\d+)";
        private const string MovieNamePattern = @"^Movie(\d+)";
        private const string ActorNamePattern = @"^Actor(\d+)";

        private const string NAOperationName = "N/A";
        private const string GenerateFriendshipRelationsOperationName = "Generate friendship relations";
        private const string GenerateFavoriteMoviesOperationName = "Generate favorite movies lists for users";
        private const string GenerateUserRecordsOperationName = "Generate user records";
        private const string RemoveAllUsersOperationName = "Remove all users";
        private const string GenerateMovieRecordsOperationName = "Generate movie records";
        private const string RemoveAllMoviesOperationName = "Remove all movies";
        private const string GenerateActorRecordsOperationName = "Generate actor records";
        private const string RemoveAllActorsOperationName = "Remove all actors";

        private readonly MongoServer _server;
        private readonly MongoDatabase _dataBase;
        private readonly string _dataBaseName;
        private readonly MongoCredentials _dataBaseCredentials;

        private readonly Regex _userRegex = new Regex( UserNamePattern, RegexOptions.IgnoreCase );
        private readonly Regex _movieRegex = new Regex( MovieNamePattern, RegexOptions.IgnoreCase );
        private readonly Regex _actorRegex = new Regex( ActorNamePattern, RegexOptions.IgnoreCase );

        public RepositoryOperationResult CurrentGeneratorOperationResult { get; private set; }

        private Task _generateUserRecordsTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoRepository"/> class.
        /// </summary>
        public MongoRepository()
        {
            var connectionStringElement = ConfigurationManager.ConnectionStrings[ConnectionStringKey];
            Debug.Assert( connectionStringElement != null );

            string connectionString = connectionStringElement.ConnectionString;
            Debug.Assert( connectionString != null );

            var url = MongoUrl.Create( connectionString );
            _dataBaseName = url.DatabaseName;
            _dataBaseCredentials = url.DefaultCredentials;

            //InitializeClassMap();

            _server = MongoServer.Create( connectionString );
            _dataBase = GetDatabase();

            // Create additional index
            _dataBase.GetCollection( UserCollectionName )
                .EnsureIndex( FavoriteMoviesMovieIdFieldName );

            CurrentGeneratorOperationResult = new RepositoryOperationResult( NAOperationName );
        }

        /// <summary>
        /// NOTE: Usually this method is not needed.
        /// Initializes the class map.
        /// </summary>
        //private void InitializeClassMap()
        //{
        //    BsonClassMap.RegisterClassMap<User>();
        //    BsonClassMap.RegisterClassMap<UserId>( classMap => classMap.MapProperty( obj => obj.Id ) );

        //    BsonClassMap.RegisterClassMap<MovieDetailed>();
        //    BsonClassMap.RegisterClassMap<MovieId>( classMap => classMap.MapProperty( obj => obj.Id ) );
        //    BsonClassMap.RegisterClassMap<MovieShortDetail>();

        //    BsonClassMap.RegisterClassMap<Actor>();
        //    BsonClassMap.RegisterClassMap<ActorId>( classMap => classMap.MapProperty( obj => obj.Id ) );
        //    BsonClassMap.RegisterClassMap<ActorShortDetail>();
        //}

        /// <summary>
        /// Gets the current database instance.
        /// </summary>
        /// <returns></returns>
        private MongoDatabase GetDatabase()
        {
            return _server.GetDatabase( _dataBaseName, _dataBaseCredentials );
        }

        /// <summary>
        /// Gets the user list.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> GetUserList()
        {
            return _dataBase.GetCollection<User>( UserCollectionName ).FindAll();
        }

        /// <summary>
        /// Gets the user by Id.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public User GetUser( string userId )
        {
            ObjectId id;
            return ObjectId.TryParse( userId, out id )
                       ? _dataBase
                             .GetCollection( UserCollectionName )
                             .FindOneByIdAs<User>( id )
                       : null;
        }

        public Movie GetMovie( string movieId )
        {
            ObjectId id;
            return ObjectId.TryParse( movieId, out id )
                       ? _dataBase
                             .GetCollection( MovieCollectionName )
                             .FindOneByIdAs<Movie>( id )
                       : null;
        }

        public IEnumerable<MovieShortDetail> GetFriendsFavotiteMovies( User user )
        {
            var userCollection = _dataBase.GetCollection( UserCollectionName );
            var friendIds = user.Friends.Select( u => (BsonValue)u.UserId );
            IMongoQuery selectQuery = Query.In( IdFieldName, friendIds );
            var result = userCollection.Distinct( FavoriteMoviesFieldName, selectQuery )
                .Select(doc => BsonSerializer.Deserialize<MovieShortDetail>((BsonDocument) doc));

            return result;
        }

        public IEnumerable<UserShortDetail> GetUsersWhoLikeTheMovie( Movie movie )
        {
            var userCollection = _dataBase.GetCollection( UserCollectionName );
            var result = userCollection
                .FindAs<UserShortDetail>( Query.EQ( FavoriteMoviesMovieIdFieldName, movie.Id ) )
                .SetFields( Fields.Include( NameFieldName ) );

            return result;
        }

        /// <summary>
        /// Gets the database statistic.
        /// </summary>
        /// <returns>Database statistic object</returns>
        public DatabaseStatistic GetDatabaseStatistic()
        {
            var statistic = new DatabaseStatistic
                                {
                                    UsersCount = _dataBase
                                        .GetCollection<User>( UserCollectionName )
                                        .AsQueryable()
                                        .Count(),
                                    MoviesCount = _dataBase
                                        .GetCollection<Movie>( MovieCollectionName )
                                        .AsQueryable()
                                        .Count(),
                                    ActorsCount = _dataBase
                                        .GetCollection<Actor>( ActorCollectionName )
                                        .AsQueryable()
                                        .Count(),
                                };
            return statistic;
        }

        /// <summary>
        /// Generates the user records.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <returns>Repository operation result object</returns>
        public RepositoryOperationResult GenerateUserRecords( long quantity )
        {
            return StartTask(
                state => GenerateUserRecordsTask(
                    quantity,
                    (RepositoryOperationResult)state ), GenerateUserRecordsOperationName );
        }

        /// <summary>
        /// Generates the friendship relations between users.
        /// </summary>
        /// <param name="minFriendsNumber">The min friends number.</param>
        /// <param name="maxFriendsNumber">The max friends number.</param>
        /// <returns>Repository operation result object</returns>
        public RepositoryOperationResult GenerateFriendshipRelations( int minFriendsNumber, int maxFriendsNumber )
        {
            return StartTask(
                state => GenerateFriendshipRelationsTask(
                    minFriendsNumber,
                    maxFriendsNumber,
                    (RepositoryOperationResult)state ), GenerateFriendshipRelationsOperationName );
        }

        /// <summary>
        /// Generates the favorite movies lists for users.
        /// </summary>
        /// <param name="minFavoriteMoviesNumber">The min favorite movies number.</param>
        /// <param name="maxFavoriteMoviesNumber">The max favorite movies number.</param>
        /// <returns>Repository operation result object</returns>
        public RepositoryOperationResult GenerateFavoriteMovies( int minFavoriteMoviesNumber, int maxFavoriteMoviesNumber )
        {
            return StartTask(
                state => GenerateFavoriteMoviesTask(
                    minFavoriteMoviesNumber,
                    maxFavoriteMoviesNumber,
                    (RepositoryOperationResult)state ), GenerateFavoriteMoviesOperationName );
        }

        /// <summary>
        /// Removes all users.
        /// </summary>
        /// <returns>Repository operation result object</returns>
        public RepositoryOperationResult RemoveAllUsers()
        {
            return StartTask(
                state => RemoveAllUsersTask(
                    (RepositoryOperationResult)state ), RemoveAllUsersOperationName );
        }

        /// <summary>
        /// Generates the movie records.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <returns>Repository operation result object</returns>
        public RepositoryOperationResult GenerateMovieRecords( long quantity )
        {
            return StartTask(
                state => GenerateMovieRecordsTask(
                    quantity,
                    (RepositoryOperationResult)state ), GenerateMovieRecordsOperationName );
        }

        /// <summary>
        /// Removes all movies.
        /// </summary>
        /// <returns>Repository operation result object</returns>
        public RepositoryOperationResult RemoveAllMovies()
        {
            return StartTask(
                state => RemoveAllMoviesTask(
                    (RepositoryOperationResult)state ), RemoveAllMoviesOperationName );
        }

        /// <summary>
        /// Generates the actor records.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <returns>Repository operation result object</returns>
        public RepositoryOperationResult GenerateActorRecords( long quantity )
        {
            return StartTask(
                state => GenerateActorRecordsTask(
                    quantity,
                    (RepositoryOperationResult)state ), GenerateActorRecordsOperationName );
        }

        /// <summary>
        /// Removes all actors.
        /// </summary>
        /// <returns>Repository operation result object</returns>
        public RepositoryOperationResult RemoveAllActors()
        {
            return StartTask(
                state => RemoveAllActorsTask(
                    (RepositoryOperationResult)state ), RemoveAllActorsOperationName );
        }

        /// <summary>
        /// Starts the task in background.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="operationName">Name of the operation.</param>
        /// <returns>Repository operation result object</returns>
        private RepositoryOperationResult StartTask( Action<object> action, string operationName )
        {
            if ((_generateUserRecordsTask == null || _generateUserRecordsTask.IsCompleted) &&
                (CurrentGeneratorOperationResult == null || CurrentGeneratorOperationResult.State != RepositoryOperationResultState.InProcess))
            {
                CurrentGeneratorOperationResult = new RepositoryOperationResult( operationName );
                _generateUserRecordsTask =
                    Task.Factory.StartNew(
                        action,
                        CurrentGeneratorOperationResult,
                        TaskCreationOptions.LongRunning );
            }
            return CurrentGeneratorOperationResult;
        }

        private void GenerateActorRecordsTask( long quantity, RepositoryOperationResult state )
        {
            state.StartDateTime = DateTime.Now;
            state.MaxIteration = quantity;
            state.State = RepositoryOperationResultState.InProcess;

            var actorCollection = _dataBase.GetCollection( ActorCollectionName );

            var lastNumber = FindLastNumber( actorCollection, NameFieldName, _actorRegex ) + 1;

            for (int i = 0; i < quantity; i++, lastNumber++)
            {
                var actor = new Actor
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = string.Format( DefaultActorNameFormat, lastNumber ),
                    Biography = string.Format( DefaultActorBiographyTextFormat, lastNumber ),
                };

                actorCollection.Insert( actor );

                state.IterationDone++;
            }

            state.EndDateTime = DateTime.Now;
            state.State = RepositoryOperationResultState.Success;
        }

        /// <summary>
        /// Generates the movie records. Task method.
        /// </summary>
        /// <param name="recordsNumber">The number of records.</param>
        /// <param name="state">The repository operation result state object.</param>
        private void GenerateMovieRecordsTask( long recordsNumber, RepositoryOperationResult state )
        {
            state.StartDateTime = DateTime.Now;
            state.MaxIteration = recordsNumber;
            state.State = RepositoryOperationResultState.InProcess;

            var movieCollection = _dataBase.GetCollection( MovieCollectionName );
            var actorCollection = _dataBase.GetCollection<Actor>( ActorCollectionName );
            var actorIds = actorCollection
                .FindAllAs<ActorId>()
                .SetFields( Fields.Include( IdFieldName ) )
                .ToList();

            var lastNumber = FindLastNumber( movieCollection, NameFieldName, _movieRegex ) + 1;

            for (int i = 0; i < recordsNumber; i++, lastNumber++)
            {
                var movie = new Movie
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = string.Format( DefaultMovieNameFormat, lastNumber ),
                    Roles = GenerateRoles( MinRolesNumber, MaxRolesNumber, actorIds, actorCollection ),
                };

                movieCollection.Insert( movie );

                state.IterationDone++;
            }

            state.EndDateTime = DateTime.Now;
            state.State = RepositoryOperationResultState.Success;
        }

        /// <summary>
        /// Generates the roles.
        /// </summary>
        /// <param name="minRolesNumber">The min roles number.</param>
        /// <param name="maxRolesNumber">The max roles number.</param>
        /// <param name="actorIds">The actor ids.</param>
        /// <param name="actors">The actors.</param>
        /// <returns>Roles collection</returns>
        private ICollection<Role> GenerateRoles( int minRolesNumber, int maxRolesNumber, ICollection<ActorId> actorIds, MongoCollection<Actor> actors )
        {
            var random = new Random();
            int roleRecordsNumber = random.Next( maxRolesNumber - minRolesNumber + 1 ) + minRolesNumber;
            if (actorIds.Count < roleRecordsNumber) roleRecordsNumber = actorIds.Count;

            var randomActorIds = GetRandomActorIds( actorIds, roleRecordsNumber );

            var roles = new List<Role>( roleRecordsNumber );
            for (int i = 0; i < roleRecordsNumber; i++)
            {
                var nActorId = random.Next( randomActorIds.Count );
                var actor = actors.FindOneById( randomActorIds[nActorId].Id );
                var role = new Role
                               {
                                   Actor = new ActorShortDetail( actor ),
                                   RoleName = string.Format( DefaultRoleNameFormat, i ),
                               };
                roles.Add( role );

                randomActorIds.RemoveAt( nActorId );
            }
            return roles;
        }

        private IList<ActorId> GetRandomActorIds( IEnumerable<ActorId> actorIds, int quantity )
        {
            var tempActorIds = actorIds.ToList();

            Random random = new Random();
            var list = new List<ActorId>();

            for (int i = 0; i < quantity; i++)
            {
                var randomActorId = tempActorIds[random.Next( tempActorIds.Count )];
                list.Add( randomActorId );
                tempActorIds.Remove( randomActorId );
            }
            return list;
        }

        private void RemoveAllMoviesTask( RepositoryOperationResult state )
        {
            state.StartDateTime = DateTime.Now;
            state.State = RepositoryOperationResultState.InProcess;

            state.MaxIteration = 1;

            var movieCollection = _dataBase.GetCollection( MovieCollectionName );
            movieCollection.RemoveAll();

            var userCollection = _dataBase.GetCollection( MovieCollectionName );
            // Remove all favorite movies
            userCollection.Update( Query.Null, Update.Unset( FavoriteMoviesFieldName ) );
            // Remove all friends favorite movies
            userCollection.Update( Query.Null, Update.Unset( FriendsFavoriteMoviesFieldName ) );

            state.IterationDone++;

            state.EndDateTime = DateTime.Now;
            state.State = RepositoryOperationResultState.Success;
        }


        private void RemoveAllActorsTask( RepositoryOperationResult state )
        {
            state.StartDateTime = DateTime.Now;
            state.State = RepositoryOperationResultState.InProcess;

            state.MaxIteration = 1;

            var actorCollection = _dataBase.GetCollection( ActorCollectionName );
            actorCollection.RemoveAll();

            state.IterationDone++;

            state.EndDateTime = DateTime.Now;
            state.State = RepositoryOperationResultState.Success;
        }

        private void RemoveAllUsersTask( RepositoryOperationResult state )
        {
            state.StartDateTime = DateTime.Now;
            state.State = RepositoryOperationResultState.InProcess;

            var allRegistredUsers = Membership.GetAllUsers();

            state.MaxIteration = allRegistredUsers.Count + 1;

            foreach (MembershipUser registredUser in allRegistredUsers)
            {
                Membership.DeleteUser( registredUser.UserName, true );
                state.IterationDone++;
            }

            var users = _dataBase.GetCollection( UserCollectionName );
            users.RemoveAll();
            state.IterationDone++;

            state.EndDateTime = DateTime.Now;
            state.State = RepositoryOperationResultState.Success;
        }

        private void GenerateFavoriteMoviesTask( int minFavoriteMoviesNumber, int maxFavoriteMoviesNumber, RepositoryOperationResult state )
        {
            state.StartDateTime = DateTime.Now;
            state.State = RepositoryOperationResultState.InProcess;

            var userCollection = _dataBase.GetCollection<User>( UserCollectionName );
            var userIds = userCollection
                .FindAllAs<UserId>()
                .SetFields( Fields.Include( IdFieldName ) )
                .ToList();

            var movieCollection = _dataBase.GetCollection<Movie>( MovieCollectionName );
            var movieIds = movieCollection
                .FindAllAs<MovieId>()
                .SetFields( Fields.Include( IdFieldName ) )
                .ToList();

            state.MaxIteration = userIds.Count;

            int deltaFavoriteMoviesNumber = maxFavoriteMoviesNumber - minFavoriteMoviesNumber;

            Random random = new Random();
            foreach (var userId in userIds)
            {
                User user = userCollection.FindOneById( userId.Id );
                int moviesNumber = random.Next( deltaFavoriteMoviesNumber + 1 ) + minFavoriteMoviesNumber;
                int iMovie = user.FavoriteMovies.Count;

                while (iMovie < moviesNumber)
                {
                    int jMovie = random.Next( movieIds.Count );
                    var movieId = movieIds[jMovie].Id;
                    if (user.FavoriteMovies.Any( m => m.MovieId.Equals( movieId ) )) continue;

                    Movie favoriteMovie = movieCollection.FindOneById( movieId );
                    Debug.Assert( favoriteMovie != null );

                    AddUserFavoriteMovie( user, favoriteMovie, userCollection );
                    iMovie++;
                }

                state.IterationDone++;
            }

            state.EndDateTime = DateTime.Now;
            state.State = RepositoryOperationResultState.Success;
        }

        private void GenerateFriendshipRelationsTask( int minFriendsNumber, int maxFriendsNumber, RepositoryOperationResult state )
        {
            state.StartDateTime = DateTime.Now;
            state.State = RepositoryOperationResultState.InProcess;

            var userCollection = _dataBase.GetCollection<User>( UserCollectionName );
            var userIds = userCollection
                .FindAllAs<UserId>()
                .SetFields( Fields.Include( IdFieldName ) )
                .ToList();

            state.MaxIteration = userIds.Count;

            int deltaFriendsNumber = maxFriendsNumber - minFriendsNumber;

            Random random = new Random();
            int iUser = 0;
            foreach (var userId in userIds)
            {
                User user = userCollection.FindOneById( userId.Id );
                int friendsNumber = random.Next( deltaFriendsNumber + 1 ) + minFriendsNumber;
                if (friendsNumber > userIds.Count - 1) friendsNumber = userIds.Count - 1;
                int iFriend = user.Friends.Count;
                while (iFriend < friendsNumber)
                {
                    int jUser = random.Next( userIds.Count );
                    if (iUser == jUser) continue;
                    var friendId = userIds[jUser].Id;
                    if (user.Friends.Any( userFriend => userFriend.UserId.Equals( friendId ) )) continue;

                    User friend = userCollection.FindOneById( friendId );
                    Debug.Assert( friend != null );

                    MakeFriends( user, friend, userCollection );
                    iFriend++;
                }
                iUser++;
                state.IterationDone++;
            }

            state.EndDateTime = DateTime.Now;
            state.State = RepositoryOperationResultState.Success;
        }

        private void AddUserFavoriteMovie( User user, Movie favoriteMovie, MongoCollection<User> userCollection )
        {
            var movieShort = new MovieShortDetail( favoriteMovie );
            var movieShortBson = movieShort.ToBsonDocument();

            // Add permanently to model without saving
            user.FavoriteMovies.Add( movieShort );
            // Add atomically to database
            userCollection.Update( Query.EQ( IdFieldName, user.Id ),
                                  Update.AddToSet( FavoriteMoviesFieldName, movieShortBson ) );

            // Update favorite movie collections for user's friends
            var friendIds = user.Friends.Select( f => f.UserId ).ToArray();
            var friends = userCollection.AsQueryable().Where( u => friendIds.Contains( u.Id ) );

            foreach (var friend in friends)
            {
                userCollection.Update( Query.EQ( IdFieldName, friend.Id ),
                                      Update.AddToSet( FriendsFavoriteMoviesFieldName, movieShortBson ) );
            }
        }

        private void MakeFriends( User first, User second, MongoCollection userCollection )
        {
            var firstFriend = new Friend( first );
            var secondFriend = new Friend( second );

            // Add permanently to model without saving
            first.Friends.Add( secondFriend );
            second.Friends.Add( firstFriend );

            // Add atomically to database
            userCollection.Update( Query.EQ( IdFieldName, first.Id ),
                                  Update.AddToSet( FriendsFieldName,
                                                  secondFriend.ToBsonDocument() ) );

            userCollection.Update( Query.EQ( IdFieldName, second.Id ),
                                  Update.AddToSet( FriendsFieldName,
                                                  firstFriend.ToBsonDocument() ) );

            // Update favorite movie collections for two users
            userCollection.Update( Query.EQ( IdFieldName, first.Id ),
                                  Update.AddToSetEachWrapped(
                                      FriendsFavoriteMoviesFieldName,
                                      (IEnumerable<MovieShortDetail>)second.FavoriteMovies ) ); /* We need this explicit type cast because driver 
                                                                                                * wrong interpret ICollection and added whole 
                                                                                                * collection as one element. */
            userCollection.Update( Query.EQ( IdFieldName, second.Id ),
                                  Update.AddToSetEachWrapped(
                                      FriendsFavoriteMoviesFieldName,
                                      (IEnumerable<MovieShortDetail>)first.FavoriteMovies ) );
        }

        private void GenerateUserRecordsTask( long quantity, RepositoryOperationResult state )
        {
            state.StartDateTime = DateTime.Now;
            state.MaxIteration = quantity;
            state.State = RepositoryOperationResultState.InProcess;

            var users = _dataBase.GetCollection( UserCollectionName );

            var lastNumber = FindLastNumber( users, NameFieldName, _userRegex ) + 1;

            var random = new Random();
            for (int i = 0; i < quantity; i++, lastNumber++)
            {
                var userModel = new RegisterModel
                                    {
                                        UserName = string.Format( DefaultUserNameFormat, lastNumber ),
                                        Age = random.Next( 91 ) + 10,
                                        Gender = (Gender)(random.Next( 2 ) + 1),
                                        Password = DefaultUserPassword,
                                    };

                userModel.Email = userModel.UserName + "@maildomain.com";

                var userMembership = Membership.GetUser( userModel.UserName );
                if (userMembership != null) Membership.DeleteUser( userModel.UserName );

                RegisterUser( userModel, false );

                state.IterationDone++;
            }

            state.EndDateTime = DateTime.Now;
            state.State = RepositoryOperationResultState.Success;
        }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="logon">if set to <c>true</c> user was logged on after successful registration.</param>
        /// <returns>Creation status</returns>
        public MembershipCreateStatus RegisterUser( RegisterModel model, bool logon )
        {
            // Attempt to register the user
            MembershipCreateStatus createStatus;
            Membership.CreateUser( model.UserName, model.Password, model.Email, null, null, true, null, out createStatus );

            if (createStatus == MembershipCreateStatus.Success)
            {
                AddUserToRepository( model );

                if (logon)
                {
                    FormsAuthentication.SetAuthCookie( model.UserName, false /* createPersistentCookie */ );
                }
            }

            return createStatus;
        }

        private void AddUserToRepository( RegisterModel model )
        {
            var users = _dataBase.GetCollection<User>( UserCollectionName );

            var user = users.AsQueryable().FirstOrDefault( u => u.Name == model.UserName ) ??
                       new User
                           {
                               Id = ObjectId.GenerateNewId()
                           };
            //var user = users.FindOne( Query.EQ( "Name", model.UserName ) ) ?? new User
            //                                                                  {
            //                                                                      Id = ObjectId.GenerateNewId(),
            //                                                                  };

            user.Name = model.UserName;
            user.Email = model.Email;
            user.Age = model.Age;
            user.Gender = model.Gender;
            users.Save( user );

            var profile = ProfileBase.Create( user.Name );
            profile[UserIdFieldName] = user.Id.ToString();
            profile.Save();
        }

        /// <summary>
        /// Find the latest number of a name in the collection by accordance conventions.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="field">The field.</param>
        /// <param name="nameMatchingRegex">The name matching regular expression.</param>
        /// <returns></returns>
        private long FindLastNumber( MongoCollection<BsonDocument> collection, string field, Regex nameMatchingRegex )
        {
            long lastNumber = -1;

            var resultSet = collection
                .Find( Query.Matches( field, new BsonRegularExpression( nameMatchingRegex ) ) )
                .SetSortOrder( SortBy.Descending( field ) );
            resultSet.Limit = 1;

            var lastUser = resultSet.FirstOrDefault();

            if (lastUser != null)
            {
                var match = nameMatchingRegex.Match( lastUser[field].ToString() );

                if (match.Success)
                {
                    string number = match.Groups[1].Value;
                    long.TryParse( number, out lastNumber );
                }
            }

            if (lastNumber < 0) lastNumber = 0;
            return lastNumber;
        }
    }
}