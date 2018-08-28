using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class NotificationsController : ApiController
    {


        private Models.IncidentsEntities _db = new Models.IncidentsEntities();




        public IQueryable<Models.Presentation.NotificationsView> Get(long id)
        {
            var notifications = from n in this._db.Notifications
                                join p in this._db.NotifyParties
                                    on n.notifyPartyId equals p.notifyPartyId
                                where n.incidentId == id
                                select new Models.Presentation.NotificationsView
                                {
                                    notificationId = n.notificationId,
                                    incidentId = n.incidentId,
                                    notifyPartyId = n.notifyPartyId,
                                    notifyPartyText = p.notifyPartyText,
                                    notifyDateTime = n.notifyDateTime,
                                    notifyContact = n.notifyContact,
                                    notifyMethod = n.notifyMethod,
                                    notifyComments = n.notifyComments
                                };


            return notifications;

        }


        public void Post([FromBody]Models.Notification value)
        {
            this._db.Notifications.Add(value);
            this._db.SaveChanges();
        }



        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }


    }
}