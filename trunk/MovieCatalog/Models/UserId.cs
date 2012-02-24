using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieCatalog.Models
{
    /// <summary>
    /// Helper class used to get only id values from collection
    /// </summary>
    public class UserId
    {
        public ObjectId Id { get; set; }
    }

}