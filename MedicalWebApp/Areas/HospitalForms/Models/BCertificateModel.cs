using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.HospitalForms.Models
{
    public class BCertificateModel
    {
        public string PatientName { get; set;}
        public string RelationName { get; set; }
        public int RequestId { get; set;}
        public string CompanyName { get; set;}
        public string ClientName { get; set; }
        public string ClientName1 { get; set; }
        public string AdmissionTime { get; set; }
        public string DischargeTime { get; set;}
        public double Bill_Amount { get; set;}
        public double InvBillRate { get; set; }
        public double PharmacyBillRate { get; set; }
        public string Doctor1 { get; set; }
        public string MedicalOfficer { get; set; }
        public string SufferingDate { get; set; }
        public string LabName { get; set; }
        public string Doctor2{ get; set; }
        public string ChiefMedicalOfficer { get; set; }
        public string Place { get; set; }
        public string Place2 { get; set; }
        public DateTime AdmissionDateTime { get; set; }
        public DateTime DischargeDateTime { get; set; }
        public DateTime SufferingDateTime { get; set; }



    }
}