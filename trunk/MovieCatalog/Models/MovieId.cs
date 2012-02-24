using MongoDB.Bson;

namespace MovieCatalog.Models
{
    /// <summary>
    /// Helper class used to get only id values from collection
    /// </summary>
    public class MovieId
    {
        public ObjectId Id { get; set; }
    }
}