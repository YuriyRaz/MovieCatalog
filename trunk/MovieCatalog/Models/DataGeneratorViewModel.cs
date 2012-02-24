using System.ComponentModel.DataAnnotations;

namespace MovieCatalog.Models
{
    public class DataGeneratorViewModel
    {
        public const int MinFriendsDefault = 30;
        public const int MaxFriendsDefault = 30;
        public const int MinFavoriteMoviesDefault = 10;
        public const int MaxFavoriteMoviesDefault = 10;
        public const int RecordsNumberForGenerationDefault = 1000;
        
        public DatabaseStatistic DatabaseStatistic { get; set; }

        [Display( Name = "Quantity of records" )]
        [DisplayFormat( DataFormatString = "{0:#,#}" )]
        [Range( 1, int.MaxValue, ErrorMessage = "You specify wrong value." )]
        public int UsersNumberForGeneration { get; set; }

        [Display( Name = "Minimum amount of friends" )]
        [DisplayFormat( DataFormatString = "{0:#,#}" )]
        [Range( 0, int.MaxValue, ErrorMessage = "You specify wrong value." )]
        public int MinFriends { get; set; }

        [Display( Name = "Maximum amount of friends" )]
        [DisplayFormat( DataFormatString = "{0:#,#}" )]
        [Range( 1, int.MaxValue, ErrorMessage = "You specify wrong value." )]
        public int MaxFriends { get; set; }

        [Display( Name = "Minimum amount of favorite movies" )]
        [DisplayFormat( DataFormatString = "{0:#,#}" )]
        [Range( 0, int.MaxValue, ErrorMessage = "You specify wrong value." )]
        public int MinFavoriteMovies { get; set; }

        [Display( Name = "Maximum amount of favorite movies" )]
        [DisplayFormat( DataFormatString = "{0:#,#}" )]
        [Range( 1, int.MaxValue, ErrorMessage = "You specify wrong value." )]
        public int MaxFavoriteMovies { get; set; }

        [Display( Name = "Quantity of records" )]
        [DisplayFormat( DataFormatString = "{0:#,#}" )]
        [Range( 1, int.MaxValue, ErrorMessage = "You specify wrong value." )]
        public int MoviesNumberForGeneration { get; set; }

        [Display( Name = "Quantity of records" )]
        [DisplayFormat( DataFormatString = "{0:#,#}" )]
        [Range( 1, int.MaxValue, ErrorMessage = "You specify wrong value." )]
        public int ActorsNumberForGeneration { get; set; }

        public DataGeneratorViewModel()
        {
            UsersNumberForGeneration = RecordsNumberForGenerationDefault;
            MoviesNumberForGeneration = RecordsNumberForGenerationDefault;
            ActorsNumberForGeneration = RecordsNumberForGenerationDefault;

            MinFriends = MinFriendsDefault;
            MaxFriends = MaxFriendsDefault;

            MinFavoriteMovies = MinFavoriteMoviesDefault;
            MaxFavoriteMovies = MaxFavoriteMoviesDefault;
        }

    }
}