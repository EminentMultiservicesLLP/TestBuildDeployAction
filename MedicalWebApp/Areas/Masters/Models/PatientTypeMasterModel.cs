using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.Masters.Models
{
    public class PatientTypeMasterModel
    {
        public int PatientTypeId { get; set; }
        public string Code { get; set; }
        public string PatientType { get; set; }
        public int Sequence { get; set; }
        public bool Deactive { get; set; }
        public int InsertedBy { get; set; }
        public DateTime? InsertedOn { get; set; }
        public string InsertedMacName { get; set; }
        public string InsertedMacId { get; set; }
        public string InsertedIpAddress { get; set; }
    }
}