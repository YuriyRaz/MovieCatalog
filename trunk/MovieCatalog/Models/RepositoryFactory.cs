using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MovieCatalog.Models.Interfaces;

namespace MovieCatalog.Models
{
    public static class RepositoryFactory
    {
        private static readonly Lazy<IRepository> Repository;

        static RepositoryFactory()
        {
            Repository = new Lazy<IRepository>(() => new MongoRepository());
        }

        public static IRepository GetRepository()
        {
            return Repository.Value;
        }
    }
}