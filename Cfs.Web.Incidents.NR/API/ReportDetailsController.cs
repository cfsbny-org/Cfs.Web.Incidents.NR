using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class ReportDetailsController : ApiController
    {


        private Models.IncidentsEntities _db = new Models.IncidentsEntities();




        [Route("api/report/details/new")]
        public Models.IncidentDetail GetNew()
        {
            return new Models.IncidentDetail();
        }


        [Route("api/report/details/{id}")]
        public Models.IncidentDetail Get(long id)
        {
            return this._db.IncidentDetails.Where(d => d.incidentId == id).SingleOrDefault();
        }



        public IQueryable<Models.IncidentDetail> GetDetailsForPrint(long id)
        {
            return this._db.IncidentDetails.Where(d => d.incidentId == id);
        }



        public void Post([FromBody]Models.IncidentDetail value)
        {
            Models.IncidentDetail detail = value;

            try
            {
                if (detail.incidentDetailId == 0)
                {
                    this._db.IncidentDetails.Add(detail);
                }
                else
                {
                    this._db.IncidentDetails.Attach(detail);
                    this._db.Entry(detail).State = System.Data.Entity.EntityState.Modified;
                }

                this._db.SaveChanges();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += Environment.NewLine + ex.InnerException.Message;
                }

                string currentUser = RequestContext.Principal.Identity.Name;

                Helpers.Mailer.SendExceptionDetail("post:/api/reportdetails", errorMessage, ex.StackTrace, currentUser, value);


                throw new Exception(errorMessage);
            }
        }




        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }
    }
}