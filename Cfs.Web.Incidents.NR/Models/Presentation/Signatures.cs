using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cfs.Web.Incidents.NR.Models.Presentation
{

    public class ReportIdsForSignature
    {
        public long reportId { get; set; }
        public long userId { get; set; }
        public string comments { get; set; }
        public string stationName { get; set; }
    }


    public class SignatureDelegation
    {
        public long reportSigId { get; set; }
        public string comments { get; set; }
        public string stationName { get; set; }
    }


    public class ReportSignatures
    {
        public long reportSigId { get; set; }
        public long incidentId { get; set; }
        public long reportSiguserId { get; set; }
        public string reportSigTypeText { get; set; }
        public string staffName { get; set;}
        public string staffTitle { get; set; }
        public int approvalStatusId { get; set; }
        public string approvalStatusText { get; set; }
        public string approvalComments { get; set; }
        public DateTime reportSigStamp { get; set; }

    }
}