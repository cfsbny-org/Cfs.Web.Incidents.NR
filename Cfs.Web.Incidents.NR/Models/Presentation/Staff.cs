using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cfs.Web.Incidents.NR.Models.Presentation
{
    public class StaffSuggestions
    {
        public long userId { get; set; }
        public string staffName { get; set; }
        public string staffTitle { get; set; }
    }
}