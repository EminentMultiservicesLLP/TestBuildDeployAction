using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.CommonArea.Models
{
    public class CircularModel
    {
        public int CircularID { get; set; }
        public string CircularName { get; set; }
        public string Location { get; set; }
        public string DownloadFileNameAs { get; set; }

        public List<CircularModel> CircularModelData { get; set; }
    }
}