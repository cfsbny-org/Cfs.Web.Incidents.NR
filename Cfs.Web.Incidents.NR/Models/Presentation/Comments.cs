using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cfs.Web.Incidents.NR.Models.Presentation
{
    public class AdminCommentsView
    {
        public long adminCommentId { get; set; }
        public long incidentId { get; set; }
        public long adminUserId { get; set; }
        public string adminUserName { get; set; }
        public string adminCommentText { get; set; }
        public DateTime adminCommentStamp { get; set; }
    }
}