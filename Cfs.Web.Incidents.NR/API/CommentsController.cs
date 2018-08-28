using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Http;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class CommentsController : ApiController
    {

        private Models.IncidentsEntities _db = new Models.IncidentsEntities();


        [Route("api/comments/report/{id}")]
       public IQueryable<Models.Comment> GetReportComments(long id)
        {
            return this._db.Comments.Where(c => c.incidentId == id && c.isDeleted == false);
        }


        public void Post([FromBody]Models.Comment value)
        {
            try
            {
                if (value.adminCommentId == 0)
                {
                    this._db.Comments.Add(value);
                }
                else
                {
                    this._db.Comments.Attach(value);
                    this._db.Entry(value).State = System.Data.Entity.EntityState.Modified;
                }

                this._db.SaveChanges();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                errorMessage += Environment.NewLine + Environment.NewLine;

                if (ex.InnerException != null)
                {
                    errorMessage += Environment.NewLine + ex.InnerException.Message;
                    errorMessage += Environment.NewLine + Environment.NewLine;
                    errorMessage += Environment.NewLine + ex.InnerException.StackTrace;
                }

                MailMessage msg = new MailMessage();
                msg.To.Add(new MailAddress("jbowen@cfsbny.org"));
                msg.From = new MailAddress("no-reply@cfsbny.org");
                msg.Subject = "Non-Residential Incident Reports: Admin Comments Error";
                msg.Body = errorMessage;

                SmtpClient smtp = new SmtpClient("cfs-mailserv");
                smtp.Send(msg);

                smtp.Dispose();
                msg.Dispose();
            }
        }


        [Route("api/comments/delete/{id}")]
        public void Delete(long id)
        {
            var comment = this._db.Comments.Where(c => c.adminCommentId == id).SingleOrDefault();

            if (comment != null)
            {
                comment.isDeleted = true;
                this._db.Comments.Attach(comment);
                this._db.Entry(comment).State = System.Data.Entity.EntityState.Modified;
                this._db.SaveChanges();
            }
        }




        // Send user comments on report.
        //[Route("api/comments/user/{id}")]



        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }
    }
}