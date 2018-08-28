using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class IncidentTypesController : ApiController
    {


        private Models.IncidentsEntities _db = new Models.IncidentsEntities();




        public IQueryable<Models.IncidentType> Get()
        {
            return this._db.IncidentTypes.Where(t => t.incidentReportTypeId == 2 && t.isActive == true);
        }


        public Models.IncidentType Get(int id)
        {
            return this._db.IncidentTypes.Where(t => t.incidentTypeId == id).SingleOrDefault();
        }



        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }


    }
}