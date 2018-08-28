using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace Cfs.Web.Incidents.NR.Controllers
{
    public class ReportsController : Controller
    {
       

        public FileStreamResult IncidentReport(long id)
        {




            API.PrintController printContoller = new API.PrintController();
            API.StaffController staffController = new API.StaffController();
            API.ReportSignsController signaturesController = new API.ReportSignsController();


            var reportDetails = printContoller.PrintIncidentReport(id).ToList();
            var incidentStaff = staffController.Get(id).ToList();
            var incidentSignatures = signaturesController.Get(id).ToList();


            printContoller.Dispose();
            staffController.Dispose();
            signaturesController.Dispose();

            Cfs.Web.Incidents.NR.Content.Reports.IncidentReport report = new NR.Content.Reports.IncidentReport();
            report.Load();


            report.Database.Tables["ReportDetails"].SetDataSource(reportDetails);

            var staffSubreport = report.Subreports["StaffInvolvedSubreport"];
            staffSubreport.Database.Tables["IncidentStaff"].SetDataSource(incidentStaff);

            var signaturesSubreport = report.Subreports["SignaturesSubreport"];
            signaturesSubreport.Database.Tables["Signatures"].SetDataSource(incidentSignatures);

            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            report.Close();
            report.Dispose();


            return new FileStreamResult (stream, "application/pdf");
        }



        private void SetReportDatabaseLogon(ReportDocument report)
        {

            ConnectionInfo ci = new ConnectionInfo();

            ci.ServerName = "cfs-abila";
            ci.DatabaseName = "Incidents.NR";
            ci.IntegratedSecurity = true;

            Tables tables = report.Database.Tables;

            foreach (Table table in tables)
            {
                TableLogOnInfo tableLogonInfo = new TableLogOnInfo();

                tableLogonInfo = table.LogOnInfo;
                tableLogonInfo.ConnectionInfo = ci;
                table.ApplyLogOnInfo(tableLogonInfo);

            }
        }

    }
}