using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace MovieCatalog.Models
{
    public class ActorShortDetail
    {
        public ActorShortDetail()
        {
        }

        public ActorShortDetail(Actor actor)
        {
            Id = actor.Id;
            Name = actor.Name;
        }

        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}