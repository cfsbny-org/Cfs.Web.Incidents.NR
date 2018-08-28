using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cfs.Web.Incidents.NR.Models.Presentation
{
    public class DocumentsPosting
    {
        public byte[] documentBlob { get; set; }
        public string fileName { get; set; }
        public int fileTypeId { get; set; }
        public string documentTitle { get; set; }
        public string documentComments { get; set; }
        public long uploadedBy { get; set; }
    }
}