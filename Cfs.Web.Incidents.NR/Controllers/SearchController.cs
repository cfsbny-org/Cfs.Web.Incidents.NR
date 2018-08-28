using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cfs.Web.Incidents.NR.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult Index()
        {
            string userName = User.Identity.Name.Substring(5);
            

            API.UsersController usersController = new API.UsersController();

            Models.User userDetails = usersController.Get(userName);

            ViewBag.UserId = userDetails.userId;
            

            return View();
        }
    }
}