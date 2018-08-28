using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class ProgramsController : ApiController
    {

        private Models.IncidentsEntities _db = new Models.IncidentsEntities();



        public IQueryable<Models.IncidentProgram> Get()
        {
            return this._db.IncidentPrograms.Where(p => p.incidentReportTypeId == 2 && p.isActive == true).OrderBy(p => p.programTitle);
        }

        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }
    }
}