using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class AttachmentsController : ApiController
    {



        private Models.IncidentsEntities _db = new Models.IncidentsEntities();





        public void Post([FromBody]List<Models.IncidentAttachment> value)
        {

            foreach (var attachment in value)
            {
                this._db.IncidentAttachments.Add(attachment);
            }
            this._db.SaveChanges();
        }




        [HttpPost, Route("api/attachments/single")]
        public void PostSingle([FromBody]Models.IncidentAttachment value)
        {
            this._db.IncidentAttachments.Add(value);
            this._db.SaveChanges();

        }



        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }



    }
}