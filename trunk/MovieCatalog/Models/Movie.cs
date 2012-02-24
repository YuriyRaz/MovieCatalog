using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace MovieCatalog.Models
{
    public class Movie
    {
        private ICollection<Role> _roles;

        public ObjectId Id { get; set; }

        [Display( Name = "Name" )]
        public string Name { get; set; }

        [Display( Name = "Description" )]
        public string Description { get; set; }

        public ICollection<Role> Roles
        {
            get { return _roles ?? (_roles = new HashSet<Role>()); }
            set { _roles = new HashSet<Role>( value ?? new Role[] { } ); }
        }
    }
}