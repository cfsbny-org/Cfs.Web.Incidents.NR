using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class StaffController : ApiController
    {



        private Models.IncidentsEntities _db = new Models.IncidentsEntities();





        public IQueryable<Models.IncidentStaff> Get(long id)
        {
            return this._db.IncidentStaffs.Where(s => s.incidentId == id);
        }




        public void Post([FromBody]List<Models.IncidentStaff> value)
        {
            foreach(var staff in value)
            {
                this._db.IncidentStaffs.Add(staff);
            }
            this._db.SaveChanges();
        }




        [HttpPost, Route("api/staff/single")]
        public void PostSingle([FromBody]Models.IncidentStaff value)
        {
            this._db.IncidentStaffs.Add(value);
            this._db.SaveChanges();
        }



        public void Delete(long id)
        {
            var staff = this._db.IncidentStaffs.Where(s => s.incidentStaffId == id).SingleOrDefault();

            if (staff != null)
            {
                this._db.IncidentStaffs.Attach(staff);
                this._db.Entry(staff).State = System.Data.Entity.EntityState.Deleted;
                this._db.SaveChanges();
            }
        }

        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }
    }
}