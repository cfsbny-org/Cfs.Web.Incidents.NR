using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class ReportSignsController : ApiController
    {


        private Models.IncidentsEntities _db = new Models.IncidentsEntities();



        [Route("api/signatures/report/{id}")]
        public IQueryable<Models.Presentation.ReportSignatures> Get(long id)
        {
            var signatures = from s in this._db.ReportSigns
                             join t in this._db.ReportSigTypes
                                on s.reportSigType equals t.reportSigTypeId
                             join a in this._db.ApprovalStatus
                                 on s.approvalStatusId equals a.approvalStatusId
                             where s.incidentId == id
                             select new Models.Presentation.ReportSignatures
                             {
                                  reportSigId = s.reportSigId,
                                  incidentId = s.incidentId,
                                  reportSigTypeText = t.reportSigTypeText,
                                  reportSiguserId = s.reportSigUserId,
                                  staffName = s.staffName,
                                  staffTitle = s.staffTitle,
                                  approvalStatusId = s.approvalStatusId,
                                  approvalStatusText = a.approvalStatusText,
                                  approvalComments = s.approvalComments,
                                  reportSigStamp = s.reportSigStamp == null ? DateTime.MinValue : (DateTime)s.reportSigStamp
                             };

            return signatures;
        }




        [HttpPost, Route("api/signatures/create")]
        public void CreateReportSignatures([FromBody]Models.Presentation.ReportIdsForSignature value)
        {

            try
            {

                var report = this._db.IncidentReports.Where(r => r.incidentId == value.reportId).SingleOrDefault();

                if (report != null)
                {
                    report.statusId = 5;
                    this._db.SaveChanges();
                }
                else
                {
                    throw new Exception("Report not found!");
                }


                Models.CfsMasterDbEntities _userDb = new Models.CfsMasterDbEntities();
                List<string> sendTos = new List<string>();



                var userDetails = _userDb.Users.Where(u => u.userId == value.userId).SingleOrDefault();

                if (userDetails == null)
                {
                    var errorMessage = "Current user not found.";
                    Helpers.Mailer.SendExceptionDetail("post:/api/signatures/create", errorMessage, string.Empty, string.Empty, value);
                    throw new Exception(errorMessage);
                }
                else
                {



                    Models.ReportSign employeeSignature = new Models.ReportSign();

                    employeeSignature.incidentId = value.reportId;
                    employeeSignature.reportSigUserId = value.userId;
                    employeeSignature.staffName = string.Format("{0} {1}", userDetails.firstName, userDetails.lastName);
                    employeeSignature.staffTitle = userDetails.jobTitle;
                    employeeSignature.reportSigType = "A";
                    employeeSignature.approvalStatusId = 3;
                    employeeSignature.incidentMedicalId = 0;
                    employeeSignature.reportSigCreated = DateTime.Now;
                    employeeSignature.reportSigStamp = DateTime.Now;
                    employeeSignature.reportSigStation = value.stationName;


                    this._db.ReportSigns.Add(employeeSignature);
                    this._db.SaveChanges();

                    sendTos.Add(userDetails.eMail);


                    int costCenterId = userDetails.costCenterId;


                    var programDetails = this._db.IncidentPrograms.Where(p => p.incidentProgramId == report.programId).SingleOrDefault();
                    var managerId = programDetails.managerId;
                    var directorId = programDetails.directorId;

                    var costCenter = _userDb.CostCenters.Where(c => c.costCenterId == userDetails.costCenterId).SingleOrDefault();
                    var supervisor = _userDb.Users.Where(u => u.userId == userDetails.supervisorId).SingleOrDefault();
                    //var manager = _userDb.Users.Where(u => u.userId == costCenter.directorId).SingleOrDefault();
                    //var director = _userDb.Users.Where(u => u.userId == costCenter.vpId).SingleOrDefault();
                    var manager = _userDb.Users.Where(u => u.userId == managerId).SingleOrDefault();
                    var director = _userDb.Users.Where(u => u.userId == directorId).SingleOrDefault();


                    if (supervisor == null)
                    {
                        string errorMessage = "Supervisor is missing or not set.";
                        Helpers.Mailer.SendExceptionDetail("post:/api/signatures/create", errorMessage, string.Empty, userDetails.userName, value);
                        throw new Exception(errorMessage);
                    }
                    else
                    {
                        Models.ReportSign supervisorSignature = new Models.ReportSign();

                        supervisorSignature.incidentId = value.reportId;
                        supervisorSignature.reportSigUserId = supervisor.userId;
                        supervisorSignature.staffName = string.Format("{0} {1}", supervisor.firstName, supervisor.lastName);
                        supervisorSignature.staffTitle = supervisor.jobTitle;
                        supervisorSignature.reportSigType = "U";
                        supervisorSignature.approvalStatusId = 1;
                        supervisorSignature.reportSigCreated = DateTime.Now;
                        supervisorSignature.incidentMedicalId = 0;


                        this._db.ReportSigns.Add(supervisorSignature);
                        this._db.SaveChanges();

                        sendTos.Add(supervisor.eMail);

                    }

                    if (costCenter == null)
                    {
                        string errorMessage = "Cost Center was not found for user.";
                        Helpers.Mailer.SendExceptionDetail("post:/api/signatures/create", errorMessage, string.Empty, userDetails.userName, value);
                        throw new Exception(errorMessage);
                    }


                    

                    if (manager != null)
                    {
                        Models.ReportSign managerSignature = new Models.ReportSign();

                        managerSignature.incidentId = value.reportId;
                        managerSignature.reportSigUserId = manager.userId;
                        managerSignature.staffName = string.Format("{0} {1}", manager.firstName, manager.lastName);
                        managerSignature.staffTitle = manager.jobTitle;
                        managerSignature.reportSigType = "M";
                        managerSignature.approvalStatusId = 1;
                        managerSignature.reportSigCreated = DateTime.Now;
                        managerSignature.incidentMedicalId = 0;


                        this._db.ReportSigns.Add(managerSignature);
                        this._db.SaveChanges();

                        sendTos.Add(manager.eMail);
                    }

                   

                    if (director != null)
                    {
                        Models.ReportSign directorSignature = new Models.ReportSign();

                        directorSignature.incidentId = value.reportId;
                        directorSignature.reportSigUserId = director.userId;
                        directorSignature.staffName = string.Format("{0} {1}", director.firstName, director.lastName);
                        directorSignature.staffTitle = director.jobTitle;
                        directorSignature.reportSigType = "D";
                        directorSignature.approvalStatusId = 1;
                        directorSignature.reportSigCreated = DateTime.Now;
                        directorSignature.incidentMedicalId = 0;


                        this._db.ReportSigns.Add(directorSignature);
                        this._db.SaveChanges();

                        sendTos.Add(director.eMail);
                    }




                    // SEND NOTIFICATION E-MAIL TO ALL
                    Controllers.ReportsController reportsController = new Controllers.ReportsController();
                    Stream reportStream = reportsController.IncidentReport(value.reportId).FileStream;
                    string attachmentName = "Incident Report.pdf";


                    StringBuilder messageBody = new StringBuilder();
                    messageBody.Append("<h1>Incident Report</h1>");
                    messageBody.Append("<p>An incident report has been submitted by " + report.staffName + " for client " + report.clientName + ".");
                    messageBody.Append("Please review attached report.</p>");
                    messageBody.Append("<p><a href=\"http://cfs-incidentsnr\">Click here to access the incident reports application.</p>");
                    messageBody.Append("<p><a href=\"http://cfs-incidentsnr/incidents/review/" + value.reportId + "\"> Click here to access the incident report directly.</p>");


                    Helpers.Mailer.SendNotificationEmail(sendTos, "Incident Report Posted", messageBody.ToString(), reportStream, attachmentName);

                    reportStream.Dispose();
                    reportsController.Dispose();




                }  // if userDetails == null





                

                _userDb.Dispose();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += Environment.NewLine + ex.InnerException.Message;
                }

                string currentUser = RequestContext.Principal.Identity.Name;

                Helpers.Mailer.SendExceptionDetail("post:/api/signatures/create", errorMessage, ex.StackTrace, value.userId.ToString(), value);


                throw new Exception(errorMessage);
            }
        }



        [HttpPost, Route("api/signatures/compliance")]
        public void AddComplianceToReport([FromBody]long id)
        {
            var signatures = this._db.ReportSigns.Where(s => s.incidentId == id && s.reportSigType == "C");
            List<string> sendTos = new List<string>();

            if (signatures.Count() == 0)
            {
                
                var notifiers = this._db.SpecialNotifiers.Where(n => n.notifierType == "Compliance" && n.isActive == true);

                if (notifiers.Count() > 0)
                {
                    foreach (var notifier in notifiers)
                    {
                        var compliance = new Models.ReportSign();
                        compliance.incidentId = id;
                        compliance.approvalStatusId = 1;
                        compliance.reportSigType = "C";
                        compliance.staffName = notifier.notifierName;
                        compliance.staffTitle = notifier.notifierTitle;
                        compliance.reportSigUserId = notifier.notifierUserId;
                        compliance.reportSigCreated = DateTime.Now;
                        compliance.incidentMedicalId = 0;

                        this._db.ReportSigns.Add(compliance);

                        sendTos.Add(notifier.notifierEmail);


                        var notification = new Models.Notification();

                        notification.incidentId = id;
                        notification.notifyPartyId = 3;  // COMPLIANCE
                        notification.notifyDateTime = DateTime.Now;
                        notification.notifyContact = notifier.notifierName;
                        notification.notifyMethod = "Automatic E-mail";
                        notification.notifyStaffId = notifier.notifierUserId;

                        this._db.Notifications.Add(notification);
                    }


                    this._db.SaveChanges();


                    Controllers.ReportsController reportsController = new Controllers.ReportsController();
                    Stream reportStream = reportsController.IncidentReport(id).FileStream;
                    string attachmentName = "Incident Report.pdf";


                    StringBuilder messageBody = new StringBuilder();
                    messageBody.Append("<h1>Incident Report</h1>");
                    messageBody.Append("<p>You have been added to an incident report.  Please review attached report.</p>");
                    messageBody.Append("<p><a href=\"http://cfs-incidentsnr\">Click here to access the incident reports application.</p>");
                    messageBody.Append("<p><a href=\"http://cfs-incidentsnr/incidents/review/" + id + "\"> Click here to access this incident report.</p>");


                    Helpers.Mailer.SendNotificationEmail(sendTos, "Compliance Notification", messageBody.ToString(), reportStream, attachmentName);

                    reportStream.Dispose();
                    reportsController.Dispose();
                }

            }


        }



        [HttpPost, Route("api/signatures/coo")]
        public void AddCooToReport([FromBody]long id)
        {
            var signatures = this._db.ReportSigns.Where(s => s.incidentId == id && s.reportSigType == "O");
            List<string> sendTos = new List<string>();

            if (signatures.Count() == 0)
            {

                var notifiers = this._db.SpecialNotifiers.Where(n => n.notifierType == "COO");

                if (notifiers.Count() > 0)
                {
                    foreach (var notifier in notifiers)
                    {
                        var coo = new Models.ReportSign();
                        coo.incidentId = id;
                        coo.approvalStatusId = 1;
                        coo.reportSigType = "O";
                        coo.staffName = notifier.notifierName;
                        coo.staffTitle = notifier.notifierTitle;
                        coo.reportSigUserId = notifier.notifierUserId;
                        coo.reportSigCreated = DateTime.Now;
                        coo.incidentMedicalId = 0;

                        this._db.ReportSigns.Add(coo);

                        sendTos.Add(notifier.notifierEmail);



                        var notification = new Models.Notification();

                        notification.incidentId = id;
                        notification.notifyPartyId = 4;  // COO
                        notification.notifyDateTime = DateTime.Now;
                        notification.notifyContact = notifier.notifierName;
                        notification.notifyMethod = "Automatic E-mail";
                        notification.notifyStaffId = notifier.notifierUserId;

                        this._db.Notifications.Add(notification);
                    }


                    this._db.SaveChanges();


                    Controllers.ReportsController reportsController = new Controllers.ReportsController();
                    Stream reportStream = reportsController.IncidentReport(id).FileStream;
                    string attachmentName = "Incident Report.pdf";


                    StringBuilder messageBody = new StringBuilder();
                    messageBody.Append("<h1>Incident Report</h1>");
                    messageBody.Append("<p>You have been added to an incident report.  Please review attached report.</p>");
                    messageBody.Append("<p><a href=\"http://cfs-incidentsnr\">Click here to access the incident reports application.</p>");
                    messageBody.Append("<p><a href=\"http://cfs-incidentsnr/incidents/review/" + id + "\"> Click here to access this incident report.</p>");


                    Helpers.Mailer.SendNotificationEmail(sendTos, "COO Notification", messageBody.ToString(), reportStream, attachmentName);

                    reportStream.Dispose();
                    reportsController.Dispose();
                }

            }


        }



        // Final approval for adminsitrators
        [HttpPost, Route("api/signature/final-approve")]
        public void FinalApproveIncidentReport([FromBody]Models.Presentation.ReportIdsForSignature value)
        {
            var signatures = this._db.ReportSigns.Where(s => s.incidentId == value.reportId && s.reportSigUserId == value.userId).ToList();

            foreach (var signature in signatures)
            {

                signature.approvalStatusId = 3;
                signature.reportSigStamp = DateTime.Now;
                signature.approvalComments = value.comments;
                signature.reportSigStation = value.stationName;

                this._db.ReportSigns.Attach(signature);
                this._db.Entry(signature).State = System.Data.Entity.EntityState.Modified;

            }

            this._db.SaveChanges();





            signatures = this._db.ReportSigns.Where(s => s.incidentId == value.reportId).ToList();


            bool allApproved = true;

            foreach (var signature in signatures)
            {
                if (signature.approvalStatusId != 3)
                {
                    if (signature.approvalStatusId != 6)
                    {
                        allApproved = false;
                        break;
                    }
                }
            }

            var report = this._db.IncidentReports.Where(r => r.incidentId == value.reportId).SingleOrDefault();


            if (report != null)
            {
                if (allApproved)
                {
                    report.statusId = 6;  // Closed
                }
                else
                {
                    report.statusId = 5;  // Locked for Review
                }
            }

            this._db.SaveChanges();


            

            

        }




        // Final approval for adminsitrators
        [HttpPost, Route("api/signature/delegated-approval")]
        public void ApproveDelegation([FromBody]Models.Presentation.SignatureDelegation value)
        {
            var signature = this._db.ReportSigns.Where(s => s.reportSigId == value.reportSigId).SingleOrDefault();

            if (signature != null)
            {
                signature.approvalStatusId = 6;
                signature.reportSigStamp = DateTime.Now;
                signature.approvalComments = value.comments;
                signature.reportSigStation = value.stationName;

                this._db.ReportSigns.Attach(signature);
                this._db.Entry(signature).State = System.Data.Entity.EntityState.Modified;
                this._db.SaveChanges();
            }
        }



        [HttpPost, Route("api/signature/reject")]
        public void RejectReport([FromBody]Models.Presentation.ReportIdsForSignature value)
        {
            var signatures = this._db.ReportSigns.Where(s => s.incidentId == value.reportId && s.reportSigUserId == value.userId).ToList();

            foreach (var signature in signatures)
            {

                signature.approvalStatusId = 4;     // SET STATUS TO DENIED
                signature.reportSigStamp = DateTime.Now;
                signature.approvalComments = value.comments;
                signature.reportSigStation = value.stationName;

                this._db.ReportSigns.Attach(signature);
                this._db.Entry(signature).State = System.Data.Entity.EntityState.Modified;

            }

            this._db.SaveChanges();


            var report = this._db.IncidentReports.Where(r => r.incidentId == value.reportId).SingleOrDefault();


            if (report != null)
            {
                report.statusId = 2;  // SET STATUS TO PENDING USER RE-SUBMISSION
            }

            this._db.SaveChanges();


            Models.CfsMasterDbEntities _userDb = new Models.CfsMasterDbEntities();
            List<string> sendTos = new List<string>();



            var userDetails = _userDb.Users.Where(u => u.userId == value.userId).SingleOrDefault();



            StringBuilder messageBody = new StringBuilder();
            messageBody.Append("<h1>Incident Report Rejected</h1>");
            messageBody.Append("<p>Your incident report for client " + report.clientName + " has been rejected by " + report.staffName + ".");
            messageBody.Append("Please review comments, make any necessary corrections and re-submit.</p>");
            messageBody.Append("<p>Comments:<br /><b>" + value.comments + "</b></p>");
            messageBody.Append("<p><a href=\"http://cfs-incidentsnr\">Click here to access the incident reports application.</p>");
            messageBody.Append("<p><a href=\"http://cfs-incidentsnr/incidents/review/" + value.reportId + "\"> Click here to access the incident report directly.</p>");


            Helpers.Mailer.SendNotificationEmail(sendTos, "Incident Report Posted", messageBody.ToString());

            _userDb.Dispose();
        }



        [HttpPost, Route("api/signatures/available")]
        public IQueryable<Models.Presentation.ReportSignatures> AvailableReportSignatures([FromBody]Models.Presentation.ReportIdsForSignature value)
        {
            var signatures = from s in this._db.ReportSigns
                             join t in this._db.ReportSigTypes
                                on s.reportSigType equals t.reportSigTypeId
                             join a in this._db.ApprovalStatus
                                 on s.approvalStatusId equals a.approvalStatusId
                             where s.incidentId == value.reportId 
                                    && s.reportSigUserId != value.userId
                                    && (s.approvalStatusId == 1 || s.approvalStatusId == 2) 
                             select new Models.Presentation.ReportSignatures
                             {
                                 reportSigId = s.reportSigId,
                                 staffName = s.staffName + ": " + s.staffTitle
                             };

            

            return signatures;
        }




        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }
    }
}