using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class ReportHeadersController : ApiController
    {

        private Models.IncidentsEntities _db = new Models.IncidentsEntities();



        [Route("api/report/header/new")]
        public Models.IncidentReport GetNew()
        {
            return new Models.IncidentReport();
        }



        [Route("api/report/header/{id}")]
        public Models.IncidentReport Get(long id)
        {
            return this._db.IncidentReports.Where(r => r.incidentId == id).SingleOrDefault();
        }



        public IQueryable<Models.IncidentReport> GetIncidentForPrint(long id)
        {
            return this._db.IncidentReports.Where(r => r.incidentId == id);
        }



        [Route("api/reports/{id}")]
        public IQueryable<Models.IncidentReport> GetMyReports(long id)
        {
            return this._db.IncidentReports.Where(r => r.userId == id && (r.statusId == 1 || r.statusId == 2));
        }



        [Route("api/reports/signatures/{id}")]
        public IQueryable<Models.IncidentReport> GetMySignatureReports(long id)
        {
            var reports = (from r in this._db.IncidentReports
                           join s in this._db.ReportStatus
                                on r.statusId equals s.reportStatusId
                           join g in
                               (
                                   from rs in this._db.ReportSigns
                                   where rs.reportSigUserId == id
                                   select rs.incidentId
                               ).Distinct()
                               on r.incidentId equals g
                           where s.reportIsActive == true && r.statusId == 5
                           select r
                           );

            return reports;
        }




        [HttpPost, Route("api/reports/user-access")]
        public Models.Presentation.UserReportAccess GetUserAccess([FromBody]Models.Presentation.ReportIdsForSignature value)
        {

            var report = this._db.IncidentReports.Where(r => r.incidentId == value.reportId).SingleOrDefault();

            if (User.IsInRole(@"MAIN\NonResIncidentAdmins"))
            {
                return Models.Presentation.UserReportAccess.ReadOnlyAccess;
            }
            else
            {

                if (report != null)  // if report with ID = value.reportId exists
                {
                    if (report.userId == value.userId) // if user is requesting access to own report
                    {
                        if (report.statusId != 6 && report.statusId != 7) // if report is not closed and not voided
                        {
                            return Models.Presentation.UserReportAccess.ReadWriteAccess;
                        }
                        else
                        {
                            return Models.Presentation.UserReportAccess.ReadOnlyAccess;
                        }
                    }
                    else
                    {

                        return Models.Presentation.UserReportAccess.NoAccess;

                    }
                }
                else
                {
                    return Models.Presentation.UserReportAccess.NoAccess;
                }
            }
        }



        [HttpPost, Route("api/reports/approver-access")]
        public Models.Presentation.UserReportAccess GetApproverAccess([FromBody]Models.Presentation.ReportIdsForSignature value)
        {

            var report = this._db.IncidentReports.Where(r => r.incidentId == value.reportId).SingleOrDefault();
            var signatures = this._db.ReportSigns.Where(s => s.incidentId == value.reportId).ToList();
            var signatureUsers = signatures.Select(s => s.reportSigUserId).Distinct().ToList();
            var signaturesPending = signatures.Where(s => s.approvalStatusId != 3 && s.approvalStatusId != 5).Select(s => s.reportSigUserId).Distinct().ToList();


            if (report != null)  // if report with ID = value.reportId exists
            {
                if (User.IsInRole(@"MAIN\NonResIncidentAdmins"))
                {
                    return Models.Presentation.UserReportAccess.ReadOnlyAccess;
                }
                else
                {
                    if (report.statusId != 6 && report.statusId != 7) // if report is not closed and not voided
                    {
                        if (signatureUsers.Contains(value.userId)) // User is a signatory for this report
                        {
                            if (signaturesPending.Contains(value.userId)) // User has already signed report
                            {
                                return Models.Presentation.UserReportAccess.ReadWriteAccess;
                            }
                            else
                            {
                                return Models.Presentation.UserReportAccess.ReadOnlyAccess;
                            }
                        }
                        else
                        {
                            return Models.Presentation.UserReportAccess.NoAccess;
                        }
                    }
                    else
                    {

                        return Models.Presentation.UserReportAccess.NoAccess;

                    }
                }
            }
            else
            {
                return Models.Presentation.UserReportAccess.NoAccess;
            }
        }




        public long Post([FromBody]Models.IncidentReport value)
        {
            try
            {
                Models.IncidentReport report = value;

                if (value.incidentId == 0)
                {
                    value.createdStamp = DateTime.Now;
                    value.lastModified = DateTime.Now;

                    this._db.IncidentReports.Add(report);

                }
                else
                {
                    this._db.IncidentReports.Attach(report);
                    this._db.Entry(report).State = System.Data.Entity.EntityState.Modified;
                }

                this._db.SaveChanges();

                return report.incidentId;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += Environment.NewLine + ex.InnerException.Message;
                }

                string currentUser = RequestContext.Principal.Identity.Name;

                Helpers.Mailer.SendExceptionDetail("post:/api/reportheaders", errorMessage, ex.StackTrace, currentUser, value);


                throw new Exception(errorMessage);
            }
        }





        [HttpPost, Route("api/reports/search")]
        public IQueryable<Models.Presentation.SearchResults> Search([FromBody]Models.Presentation.SearchCriteria value)
        {
            bool isIncidentsAdmin = User.IsInRole(@"MAIN\IncidentAdmins");

            if (isIncidentsAdmin)
            {
                var reports = from r in this._db.IncidentReports
                              join s in this._db.ReportStatus
                                on r.statusId equals s.reportStatusId
                              join p in this._db.IncidentPrograms
                                  on r.programId equals p.incidentProgramId
                              where
                                r.clientName.Contains(value.clientName)
                                && r.createdStamp >= value.startDate
                                && r.createdStamp <= value.endDate
                              select new Models.Presentation.SearchResults
                              {
                                  incidentId = r.incidentId,
                                  incidentDate = r.incidentDate,
                                  createdStamp = r.createdStamp,
                                  clientName = r.clientName,
                                  staffName = r.staffName,
                                  statusId = r.statusId,
                                  reportStatusText = s.reportStatusText,
                                  programTitle = p.programTitle,
                                  incidentLocation = r.incidentLocation
                              };

                return reports;
            }
            else
            {
                var reports = from r in this._db.IncidentReports
                              join s in this._db.ReportStatus
                                on r.statusId equals s.reportStatusId
                              join p in this._db.IncidentPrograms
                                  on r.programId equals p.incidentProgramId
                              where
                                r.clientName.Contains(value.clientName)
                                && r.createdStamp >= value.startDate
                                && r.createdStamp <= value.endDate
                                && r.userId == value.userId
                              select new Models.Presentation.SearchResults
                              {
                                  incidentId = r.incidentId,
                                  incidentDate = r.incidentDate,
                                  createdStamp = r.createdStamp,
                                  clientName = r.clientName,
                                  staffName = r.staffName,
                                  statusId = r.statusId,
                                  reportStatusText = s.reportStatusText,
                                  programTitle = p.programTitle,
                                  incidentLocation = r.incidentLocation
                              };

                return reports;
            }

        }


        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }

    }
}