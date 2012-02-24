using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace MovieCatalog.Models
{
    /// <summary>
    /// Helper class used to get only id values from collection
    /// </summary>
    public class ActorId
    {
        public ObjectId Id { get; set; }
    }
}