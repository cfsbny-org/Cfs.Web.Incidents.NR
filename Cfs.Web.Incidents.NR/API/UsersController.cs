using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class UsersController : ApiController
    {

        private Models.CfsMasterDbEntities _db = new Models.CfsMasterDbEntities();


        [Route("api/users/{userName}")]
        public Models.User Get(string userName)
        {
            return this._db.Users.Where(u => u.userName == userName).SingleOrDefault();
        }


        [Route("api/staff/suggest/{query}"), HttpGet]
        public IQueryable<Models.Presentation.StaffSuggestions> SuggestStaff(string query)
        {
            var staff = from u in this._db.Users
                        where u.lastName.Contains(query) || u.firstName.Contains(query)
                            && u.activeFlag == true
                        orderby u.lastName, u.firstName
                        select new Models.Presentation.StaffSuggestions
                        {
                            userId = u.userId,
                            staffName = u.firstName + " " + u.lastName,
                            staffTitle = u.jobTitle
                        };

            return staff;
        }

        [Route("api/supervisor/{id}")]
        public Models.User GetUserSupervisor(long id)
        {
            return this._db.Users.Where(u => u.userId == id).SingleOrDefault();
        }





        [HttpPost, Route("api/user/validate")]
        public HttpResponseMessage ValidateUser([FromBody]string password)
        {

            HttpResponseMessage response = new HttpResponseMessage();

            string userName = User.Identity.Name.Substring(5).ToLower();


            DirectoryEntry domain = new DirectoryEntry("LDAP://main.cfsbny.org", @"MAIN\" + userName, password);

            using (domain)
            {
                try
                {
                    object nativeObject = domain.NativeObject;
                    DirectorySearcher searcher = new DirectorySearcher(domain);

                    searcher.Filter = "(SAMAccountName=" + userName + ")";
                    //searcher.PropertiesToLoad.Add("cn");
                    SearchResult result = searcher.FindOne();

                    if (result == null)
                    {
                        response.StatusCode = HttpStatusCode.InternalServerError;
                        response.Content = new StringContent("Unable to validate user.  User name and password do not match.");

                        throw new HttpResponseException(response);
                    }


                }
                catch (Exception ex)
                {
                    string errorMessage = ex.Message.Trim();
                    if (ex.InnerException != null)
                    {
                        errorMessage += Environment.NewLine + "Inner Exception: " + ex.InnerException.Message;
                    }

                    //errorMessage += Environment.NewLine + "User Name: " + userName;
                    //errorMessage += Environment.NewLine + "Password: " + password;

                    //List<string> sendTo = new List<string>();
                    //sendTo.Add("jbowen@cfsbny.org");

                    //Cfs.Intranet.Utilities.SendNotificationEmail(sendTo, "jbowen@cfsbny.org", "ESS: Signature Validation Failure", errorMessage);

                    response.StatusCode = HttpStatusCode.InternalServerError;
                    //response.Content = new StringContent("User name: " + userName + "; " + errorMessage);
                    response.Content = new StringContent(string.Format("User name: {0}; Password: {1}; Error message: {2}", userName, password, errorMessage));

                    throw new HttpResponseException(response);

                }

                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StringContent("true");
                return response;
            }
        }




        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }


    }
}