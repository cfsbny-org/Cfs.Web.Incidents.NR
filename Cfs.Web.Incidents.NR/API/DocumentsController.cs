using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Cfs.Web.Incidents.NR.API
{
    public class DocumentsController : ApiController
    {

        private Models.IncidentsEntities _db = new Models.IncidentsEntities();





        public IQueryable<Models.Document> Get(long id)
        {
            var documents = from d in this._db.Documents
                            join a in this._db.IncidentAttachments
                                on d.documentId equals a.documentId
                            where a.incidentId == id
                            select d;

            return documents;
        }


        public Task<HttpResponseMessage> Post()
        {

            string documentPath = System.Web.Hosting.HostingEnvironment.MapPath("~/content/Documents/");

            string todayString = DateTime.Today.ToString("yyyy-MM-dd");
            string todayFileStamp = DateTime.Now.ToString("yyyyMMddhhmmss");
            documentPath = Path.Combine(documentPath, todayString);

            if (!Directory.Exists(documentPath))
            {
                Directory.CreateDirectory(documentPath);
            }

            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new Helpers.CustomMultipartFormDataStreamProvider(documentPath);

            var task = request.Content.ReadAsMultipartAsync(provider)
                .ContinueWith<HttpResponseMessage>(o =>
                {

                    if (o.IsFaulted || o.IsCanceled)
                    {
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    }

                    var fileName = provider.FileData.Select(i => {
                        var info = new FileInfo(i.LocalFileName);
                        return info.Name;
                    });

                    long userId = long.Parse(provider.FormData["uploadedBy"]);
                    string documentTitle = provider.FormData["documentTitle"];
                    string documentComments = provider.FormData["documentComments"];


                    Models.Document document = new Models.Document();

                    document.fileTypeId = 1;
                    document.documentUrl = string.Format("/content/Documents/{0}/{1}", todayString, fileName.First());
                    document.documentFileName = fileName.First();
                    document.documentTitle = documentTitle;
                    document.documentComments = documentComments;
                    document.uploadedBy = userId;
                    document.uploadedStamp = DateTime.Now;
                    document.isVoided = false;

                    this._db.Documents.Add(document);
                    this._db.SaveChanges();

                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(document.documentId.ToString())
                    };

                });

            return task;
            
            //string fileNameGuid = string.Format("{0}.{1}", Guid.NewGuid().ToString(), fileExtension);


            //string fileNamePath = Path.Combine(documentPath, fileNameGuid);
            //File.WriteAllBytes(fileNamePath, value.documentBlob);


            
        }






        protected override void Dispose(bool disposing)
        {
            this._db.Dispose();
            base.Dispose(disposing);
        }


    }
}