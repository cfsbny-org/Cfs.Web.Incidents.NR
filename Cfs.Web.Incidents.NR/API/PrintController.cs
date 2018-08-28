using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class PrintController : ApiController
    {

        private Models.IncidentsEntities _db = new Models.IncidentsEntities();




        [HttpGet, Route("api/print/report/{id}")]
        public IQueryable<Models.Presentation.IncidentReportPrint> PrintIncidentReport(long id)
        {
            var report = from r in this._db.IncidentReports
                         join d in this._db.IncidentDetails
                            on r.incidentId equals d.incidentId
                         join p in this._db.IncidentPrograms
                            on r.programId equals p.incidentProgramId
                         join s in this._db.ReportStatus
                            on r.statusId equals s.reportStatusId
                         join t in this._db.IncidentTypes
                            on r.incidentTypeId equals t.incidentTypeId
                         join hl in this._db.HarmRiskLevels
                             on d.harmLevel equals hl.levelId
                         join rl in this._db.HarmRiskLevels
                            on d.riskLevel equals rl.levelId
                         where r.incidentId == id
                         select new Models.Presentation.IncidentReportPrint
                         {
                             incidentId = r.incidentId,
                             staffName = r.staffName,
                             staffTitle = r.staffTitle,
                             clientName = r.clientName,
                             clientDob = r.clientDob,
                             clientGender = r.clientGender,
                             clientId = r.clientId,
                             incidentDate = r.incidentDate,
                             incidentTime = r.incidentTime,
                             isApproximate = r.isApproximate,
                             dateDiscovered = r.dateDiscovered == null ? DateTime.MinValue : (DateTime)r.dateDiscovered,
                             programTitle = p.programTitle,
                             incidentLocation = r.incidentLocation,
                             createdStamp = r.createdStamp,
                             reportStatusText = s.reportStatusText,
                             isClientHighRisk = (bool)r.isClientHighRisk,
                             addClientToHighRisk = r.addClientToHighRisk,
                             incidentTypeText = t.incidentTypeText,
                             incidentDetails = d.incidentDetails,
                             harmLevel = hl.levelText,
                             riskLevel = rl.levelText,
                             contactInfo = d.contactInfo,
                             incidentComments = d.incidentComments,
                             internInvolved = d.internInvolved,
                             volunteerInvolved = d.volunteerInvolved,
                             visitorInvolved = d.visitorInvolved,
                             otherInvolved = d.otherInvolved,
                             otherInvolvedDetails = d.otherInvolvedDetails
                         };

            return report;
        }




        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }
    }
}