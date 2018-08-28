using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cfs.Web.Incidents.NR.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            string userName = User.Identity.Name.Substring(5);
            

            API.UsersController usersController = new API.UsersController();

            Models.User userDetails = usersController.Get(userName);
            usersController.Dispose();

            ViewBag.UserId = userDetails.userId;
            ViewBag.Greeting = userDetails.firstName;


            System.Web.HttpBrowserCapabilitiesBase browser = Request.Browser;

            ViewBag.Browser = browser.Browser;

            return View();  
        }



        public ActionResult Sandbox()
        {
            return View();
        }
    }
}