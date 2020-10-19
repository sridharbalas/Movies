using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RightPoint.Framework.Utilities;
using RightPoint.Business.CachedData;
using Newtonsoft608.Json;
using System.Text;

namespace Movies.Web.Controllers
{
    public class HomeController : Controller
    {

        private const string SEARCHTYPE_ANYTHING = "Anything";

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}