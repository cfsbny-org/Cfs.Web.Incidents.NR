using System.Web.Mvc;

namespace Cfs.Web.Incidents.NR.Controllers
{
    public class IncidentsController : Controller
    {
        // GET: Incidents
        public ActionResult Index()
        {

            string userName = User.Identity.Name.Substring(5);
            string userIpAddress = Request.UserHostName;



            API.UsersController usersController = new API.UsersController();

            Models.User userDetails = usersController.Get(userName);

            ViewBag.UserId = userDetails.userId;
            ViewBag.StaffName = userDetails.firstName + " " + userDetails.lastName;
            ViewBag.StaffTitle = userDetails.jobTitle;
            ViewBag.StaffEmail = userDetails.eMail;
            ViewBag.StationName = Helpers.Converters.GetComputerName(userIpAddress);

            return View();
        }


        public ActionResult Edit(long id)
        {
            ViewBag.IncidentId = id;

            string userName = User.Identity.Name.Substring(5);
            string userIpAddress = Request.UserHostName;



            API.UsersController usersController = new API.UsersController();

            Models.User userDetails = usersController.Get(userName);

            ViewBag.UserId = userDetails.userId;
            ViewBag.StaffName = userDetails.firstName + " " + userDetails.lastName;
            ViewBag.StaffTitle = userDetails.jobTitle;
            ViewBag.StaffEmail = userDetails.eMail;
            ViewBag.StationName = Helpers.Converters.GetComputerName(userIpAddress);

            return View();
        }



        public ActionResult Review(long id)
        {
            ViewBag.IncidentId = id;

            string userName = User.Identity.Name.Substring(5);
            string userIpAddress = Request.UserHostName;



            API.UsersController usersController = new API.UsersController();

            Models.User userDetails = usersController.Get(userName);

            ViewBag.UserId = userDetails.userId;
            ViewBag.StaffName = userDetails.firstName + " " + userDetails.lastName;
            ViewBag.StaffTitle = userDetails.jobTitle;
            ViewBag.StaffEmail = userDetails.eMail;
            ViewBag.StationName = Helpers.Converters.GetComputerName(userIpAddress);


            return View();
        }




        public ActionResult NoAccess()
        {
            return View();
        }
    }
}