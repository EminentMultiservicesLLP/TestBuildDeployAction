using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.HospitalForms.Models
{
    public class ViewBillsModel_Report
    {
        
            public string RequestNo { get; set; }
            public string FileNo { get; set; }
            public string HospitalType { get; set; }
            public string ManagementType { get; set; }
            public string RoomEntitleType { get; set; }
            public string Gender { get; set; }
            public string PatientType { get; set; }
            public string PatientName { get; set; }
            public double PatientAge { get; set; }
            public string PatientAddress { get; set; }
            public string BillNo { get; set; }
            public double BillAmount { get; set; }
            public string BillDate { get; set; }
            public string StrAdmissionDate { get; set; }
            public string StrDischrgeDate { get; set; }
            public string RoomType { get; set; }
            public string TypeOfAddmission { get; set; }
            public string ClientName { get; set; }                
            public string DoctorIncharge { get; set; }
            public string RegistrationNo { get; set; }
            public bool IsReportPrinted { get; set; }
            public string IsDeactive { get; set; }
            public string Comment { get; set; }
            public string LoginName { get; set; }
            public string FromDate { get; set; }
            public string ToDate { get; set; }
            public int TypeofReport { get; set; }
        
    }
}