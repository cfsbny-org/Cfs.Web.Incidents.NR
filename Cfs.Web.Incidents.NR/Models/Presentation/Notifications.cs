using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cfs.Web.Incidents.NR.Models.Presentation
{
    public class NotificationsView
    {
        public long notificationId { get; set; }
        public long incidentId { get; set; }
        public int notifyPartyId { get; set; }
        public string notifyPartyText { get; set; }
        public DateTime notifyDateTime { get; set; }
        public string notifyContact { get; set; }
        public string notifyMethod { get; set; }
        public string notifyComments { get; set; }
    }
}