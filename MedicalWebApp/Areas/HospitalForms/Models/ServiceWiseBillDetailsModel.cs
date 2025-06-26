using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.HospitalForms.Models
{
    public class ServiceWiseBillDetailsModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string BillDate { get; set; }
        public string BillNo { get; set; }
        public string PatientName { get; set; }
        public int Quantity { get; set; }
        public string CategoryName { get; set; }
        public double BillRate { get; set; }
        public double Amount { get; set; }
    }
}