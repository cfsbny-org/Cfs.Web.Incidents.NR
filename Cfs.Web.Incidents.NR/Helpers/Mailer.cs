using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;

namespace Cfs.Web.Incidents.NR.Helpers
{
    public class Mailer
    {

        private const string singleLineBreak = "<br />";
        private const string doubleLineBreak = "<br /><br />";

        public static void SendExceptionDetail(string apiRoute, string errorMessage, string stackTrace, string currentUser, object entityObject)
        {

            PropertyInfo[] objectProperties = entityObject.GetType().GetProperties();

            StringBuilder msgBody = new StringBuilder();


            msgBody.Append(string.Format("Page: {0}; User: {1}", apiRoute, currentUser)).Append(doubleLineBreak);
            msgBody.Append(errorMessage).Append(doubleLineBreak);
            msgBody.Append(stackTrace).Append(doubleLineBreak);

            foreach (PropertyInfo objectProperty in objectProperties)
            {
                string propertyValue = string.Empty;

                if (objectProperty.GetValue(entityObject, null) == null)
                {
                    propertyValue = "<b style=\"color: #ff0000;\">(null)</b>";
                }
                else
                {
                    propertyValue = objectProperty.GetValue(entityObject, null).ToString();
                }

                msgBody.Append(string.Format("{0}: {1}{2}", objectProperty.Name, propertyValue, singleLineBreak));
            }

            //objectProperty.GetValue(entityObject, null).ToString()

           



            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress("jbowen@cfsbny.org"));
            msg.From = new MailAddress("techservices@cfsbny.org");
            msg.Subject = "Non-Residential Incident Reports Exception";
            msg.IsBodyHtml = true;
            msg.Body = msgBody.ToString();

            SmtpClient smtp = new SmtpClient("cfs-mailserv");
            smtp.Send(msg);

            msg.Dispose();
            smtp.Dispose();


        }



        public static void SendNotificationEmail(List<string> sendTos, string subjectLine, string messageBody)
        {
            MailMessage msg = new MailMessage();

            foreach(string sendTo in sendTos)
            {
                msg.To.Add(new MailAddress(sendTo));
            }
            msg.Bcc.Add(new MailAddress("jbowen@cfsbny.org"));
            msg.From = new MailAddress("no-reply@cfsbny.org");
            msg.Subject = "CFS Incident Reports: " + subjectLine;
            msg.IsBodyHtml = true;

            string cssFile = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/css/email.css");
            string emailCss = System.IO.File.ReadAllText(cssFile);

            msg.Body = "<style type=\"text/css\">" + emailCss + "</style>";
            msg.Body += messageBody;


            SmtpClient smtp = new SmtpClient("cfs-mailserv");
            smtp.Send(msg);

            
            smtp.Dispose();
            msg.Dispose();
        }



        public static void SendNotificationEmail(List<string> sendTos, string subjectLine, string messageBody, Stream attachmentStream, string attachmentName)
        {
            MailMessage msg = new MailMessage();

            foreach (string sendTo in sendTos)
            {
                msg.To.Add(new MailAddress(sendTo));
            }
            msg.Bcc.Add(new MailAddress("jbowen@cfsbny.org"));
            msg.From = new MailAddress("no-reply@cfsbny.org");
            msg.Subject = "CFS Incident Reports: " + subjectLine;
            msg.IsBodyHtml = true;

            msg.Attachments.Add(new System.Net.Mail.Attachment(attachmentStream, attachmentName, "application/pdf"));

            string cssFile = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/css/email.css");
            string emailCss = System.IO.File.ReadAllText(cssFile);

            msg.Body = "<style type=\"text/css\">" + emailCss + "</style>";
            msg.Body += messageBody;


            SmtpClient smtp = new SmtpClient("cfs-mailserv");
            smtp.Send(msg);


            smtp.Dispose();
            msg.Dispose();
        }
    }
}