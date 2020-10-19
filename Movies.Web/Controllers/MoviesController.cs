using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RightPoint.Framework.Utilities;
using RightPoint.Business.CachedData;
using System.Text;
using Newtonsoft608.Json;
using System.Data.SqlClient;

namespace Movies.Web.Controllers
{
    public class MoviesController : Controller
    {

        [HttpGet]
        public ActionResult Index(string searchTitle, string searchType, string pageNumber)
        {

            try
            {
                #region Validate and initialize values

                /*  Comments for review - 
                    This region can be refactored by creating a Request class with 
                    a Validate() method by passing parameters to its constructor if there are lot of validations. 
                */
                const string MOVIE = "movie";
                const string SERIES = "series";

                searchTitle = string.IsNullOrEmpty(searchTitle) ? string.Empty : searchTitle;
                searchType = string.IsNullOrEmpty(searchType) ? string.Empty : searchType;
                searchType = (searchType.ToLower() == MOVIE || searchType.ToLower() == SERIES) ? searchType : string.Empty;
                int.TryParse(pageNumber, out int validPageNumber);

                #endregion

                #region Business Logic
                /* Comments for review - 
                   This can be created as a wrapper service to build our own Movie service that consumes OMDB API to build sorting or different page size 
                   or custom implementation instead of OMDB API returning always up to page size of 10.
                */

                RightPoint.Business.Movies movies = new RightPoint.Business.Movies();
                RightPoint.Business.Models.Movies moviesModel = movies.GetMovies(searchTitle, searchType, validPageNumber);

                #endregion

                SetViewBag(searchTitle, searchType, validPageNumber, movies.PageCount);

                return View(moviesModel);
            }
            catch (Exception ex)
            {
                // Log the exception with stack trade and return status code with friendly message.
                throw (ex);
            }

        }

        private void SetViewBag(string searchTitle, string searchType, int validPageNumber, int pageCount)
        {
            ViewBag.SearchTitle = searchTitle;
            ViewBag.SearchType = searchType;
            ViewBag.PageNumber = validPageNumber;
            ViewBag.PageCount = pageCount;
        }
    }
}