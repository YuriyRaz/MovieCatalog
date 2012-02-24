using System.Web.Mvc;
using MovieCatalog.Models;

namespace MovieCatalog.Controllers
{
    public class MovieController : Controller
    {
        //
        // GET: /Movie/Details/5

        public ActionResult Details( string id )
        {
            var repo = RepositoryFactory.GetRepository();
            var movie = repo.GetMovie( id );
            ViewBag.UsersWhoLikeTheMovie = repo.GetUsersWhoLikeTheMovie( movie );
            return View( movie );
        }
    }
}
