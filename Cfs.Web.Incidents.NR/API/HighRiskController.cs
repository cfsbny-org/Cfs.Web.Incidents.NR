using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class HighRiskController : ApiController
    {



        private Models.IncidentsEntities _db = new Models.IncidentsEntities();



        public IQueryable<Models.HighRisk> Get()
        {
            return this._db.HighRisks.Where(h => h.dateRemoved == null);
        }


        public void Post([FromBody]Models.HighRisk value)
        {
            if (value.highRiskClientId == 0)
            {
                Models.HighRisk newHighRisk = new Models.HighRisk();


                this._db.HighRisks.Add(newHighRisk);
            }
            else
            {
            }

            this._db.SaveChanges();
        }



        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }
    }
}