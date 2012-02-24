using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieCatalog.Models
{
    public class UserShortDetail
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }

}