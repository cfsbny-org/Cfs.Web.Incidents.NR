using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class NotifyPartiesController : ApiController
    {


        private Models.IncidentsEntities _db = new Models.IncidentsEntities();





        public IQueryable<Models.NotifyParty> Get()
        {
            return this._db.NotifyParties.Where(p => p.isActive == true);
        }



        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }
    }
}