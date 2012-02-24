using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.Security;
using MongoDB.Bson;
using MovieCatalog.Models;

namespace MovieCatalog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            if (Request.IsAuthenticated)
            {
                var profile = ProfileBase.Create( User.Identity.Name );
                var userId = (string)profile["UserId"];
                return RedirectToAction( "UserInfo", "Home", new { id = userId } );
            }

            return View();
        }

        public ActionResult UserInfo( string id )
        {
            var repo = RepositoryFactory.GetRepository();
            var user = repo.GetUser( id );
            if (user == null)
            {
                return RedirectToAction( "UserNotFound" );
            }
            else
            {
                ViewBag.Message = "User info";
                ViewBag.FriendsFavoriteMovies = repo.GetFriendsFavotiteMovies( user );
                return View( user );
            }
        }

        public ActionResult UserNotFound()
        {
            ViewBag.Message = "User not found!";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

    }
}
