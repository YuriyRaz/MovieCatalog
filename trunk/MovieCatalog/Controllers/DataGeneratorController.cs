using System;
using System.Web.Mvc;
using MovieCatalog.Controllers.Extension;
using MovieCatalog.Models;
using MovieCatalog.Models.Interfaces;

namespace MovieCatalog.Controllers
{
    public class DataGeneratorController : Controller
    {
        //
        // GET: /DataGenerator/

        public ActionResult GenerateData()
        {
            var repo = RepositoryFactory.GetRepository();
            var model = new DataGeneratorViewModel
                            {
                                DatabaseStatistic = repo.GetDatabaseStatistic()
                            };

            ViewBag.Result = repo.CurrentGeneratorOperationResult;
            return View( model );
        }

        ActionResult DoRepositoryAction( DataGeneratorViewModel model, Func<IRepository, RepositoryOperationResult> action )
        {
            var repo = RepositoryFactory.GetRepository();
            var result = repo.CurrentGeneratorOperationResult;
            if (ModelState.IsValid && result.State != RepositoryOperationResultState.InProcess)
            {
                result = action( repo );
                model.DatabaseStatistic = repo.GetDatabaseStatistic();
            }
            ViewBag.Result = result;

            return View( "GenerateData", model );
        }

        [HttpPost]
        [ActionName( "GenerateData" )]
        [ButtonPressed( ButtonName = "GenerateUserRecords" )]
        public ActionResult GenerateUserRecords( DataGeneratorViewModel model )
        {
            return DoRepositoryAction(model, repo => repo.GenerateUserRecords( model.UsersNumberForGeneration ));
        }

        [HttpPost]
        [ActionName( "GenerateData" )]
        [ButtonPressed( ButtonName = "GenerateFriendshipRelations" )]
        public ActionResult GenerateFriendshipRelations( DataGeneratorViewModel model )
        {
            return DoRepositoryAction( model, repo => repo.GenerateFriendshipRelations( model.MinFriends, model.MaxFriends ) );
        }

        [HttpPost]
        [ActionName( "GenerateData" )]
        [ButtonPressed( ButtonName = "GenerateFavoriteMovies" )]
        public ActionResult GenerateFavoriteMovies( DataGeneratorViewModel model )
        {
            return DoRepositoryAction( model, repo => repo.GenerateFavoriteMovies( model.MinFavoriteMovies, model.MaxFavoriteMovies ) );
        }

        [HttpPost]
        [ActionName( "GenerateData" )]
        [ButtonPressed( ButtonName = "RemoveAllUsers" )]
        public ActionResult RemoveAllUsers( DataGeneratorViewModel model )
        {
            return DoRepositoryAction( model, repo => repo.RemoveAllUsers() );
        }

        [HttpPost]
        [ActionName( "GenerateData" )]
        [ButtonPressed( ButtonName = "GenerateMovieRecords" )]
        public ActionResult GenerateMovieRecords( DataGeneratorViewModel model )
        {
            return DoRepositoryAction( model, repo => repo.GenerateMovieRecords( model.MoviesNumberForGeneration ) );
        }

        [HttpPost]
        [ActionName( "GenerateData" )]
        [ButtonPressed( ButtonName = "RemoveAllMovies" )]
        public ActionResult RemoveAllMovies( DataGeneratorViewModel model )
        {
            return DoRepositoryAction( model, repo => repo.RemoveAllMovies() );
        }

        [HttpPost]
        [ActionName( "GenerateData" )]
        [ButtonPressed( ButtonName = "GenerateActorRecords" )]
        public ActionResult GenerateActorRecords( DataGeneratorViewModel model )
        {
            return DoRepositoryAction( model, repo => repo.GenerateActorRecords( model.ActorsNumberForGeneration ) );
        }

        [HttpPost]
        [ActionName( "GenerateData" )]
        [ButtonPressed( ButtonName = "RemoveAllActors" )]
        public ActionResult RemoveAllActors( DataGeneratorViewModel model )
        {
            return DoRepositoryAction( model, repo => repo.RemoveAllActors() );
        }
    }
}
