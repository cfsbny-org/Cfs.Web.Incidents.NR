using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cfs.Web.Incidents.NR.Models.Presentation
{
    public class SearchCriteria
    {
        public long userId { get; set; }
        public string clientName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }



    public class SearchResults
    {
        public long incidentId { get; set; }
        public DateTime incidentDate { get; set; }
        public DateTime createdStamp { get; set; }
        public string clientName { get; set; }
        public string staffName { get; set; }
        public int statusId { get; set; }
        public string reportStatusText { get; set; }
        public string programTitle { get; set; }
        public string incidentLocation { get; set; }
    }



    public class IncidentReportPrint
    {
        public long incidentId { get; set; }
        public string staffName { get; set; }
        public string staffTitle { get; set; }
        public string clientName { get; set; }
        public string clientId { get; set; }
        public DateTime clientDob { get; set; }
        public string clientGender { get; set; }
        public DateTime incidentDate { get; set; }
        public string incidentTime { get; set; }
        public DateTime dateDiscovered { get; set; }
        public bool isApproximate { get; set; }
        public string programTitle { get; set; }
        public string incidentLocation { get; set; }
        public DateTime createdStamp { get; set; }
        public string reportStatusText { get; set; }
        public bool isClientHighRisk { get; set; }
        public bool addClientToHighRisk { get; set; }
        public string incidentTypeText { get; set; }
        public string incidentDetails { get; set; }
        public string harmLevel { get; set; }
        public string riskLevel { get; set; }
        public string contactInfo { get; set; }
        public string incidentComments { get; set; }
        public bool internInvolved { get; set; }
        public bool volunteerInvolved { get; set; }
        public bool visitorInvolved { get; set; }
        public bool otherInvolved { get; set; }
        public string otherInvolvedDetails { get; set; }

    }


}