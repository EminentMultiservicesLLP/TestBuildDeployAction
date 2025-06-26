using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CGHSBilling.Areas.Masters.Models;

namespace CGHSBilling.Areas.HospitalForms.Models
{
    public class RequestSubmissionOPDModel
    {
        public int RequestId { get; set; }

        public string RequestNo { get; set; }
        [Required]
        public string RegistrationNo { get; set; }
        [Required]
        public int HospitalTypeId { get; set; }
        public string HospitalType { get; set; }
        [Required]
        public string NameOfDoctor1 { get; set; }
        public string NameOfDoctor2 { get; set; }
        public string NameOfDoctor3 { get; set; }
        [Required]
        public int GenderId { get; set; }
        public string Gender { get; set; }
        [Required]
        public int PatientTypeId { get; set; }
        public string PatientType { get; set; }
        [Required]
        public string PatientName { get; set; }
        [Required]
        public double PatientAge { get; set; }
        [Required]
        public string PatientAddress{ get; set; }
        [Required]
        public string OPDNo{ get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime OPDDate { get; set; }
        public string StrOPDDate { get; set; }
        public double BillAmount { get; set; }
        public int InsertedBy { get; set; }
        public DateTime? InsertedOn { get; set; }
        public string StrInsertedOn { get; set; }
       
        public string RoomType { get; set; }
        public string InsertedMacName { get; set; }
        public string InsertedMacId { get; set; }
        public string InsertedIpAddress { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public int ClientId { get; set; }
        public int TypeOfAddmissionId { get; set; }
        public string TypeOfAddmission { get; set; }
        public string ClientName { get; set; }
        public string Comment { get; set; }
        public List<CommonMasterModel> ConsumeDiv { get; set; }
        public List<CommonMasterModel> DefaultServices { get; set; }
        public List<CommonMasterModel> ManullyAddedService { get; set; }
        public string CompanyName { get; set; }

        public bool IsDeactive { get; set; }
        public string DeactiveComment { get; set; }
        public bool IsHopePatientBill { get; set; }
        public PatientModel Patient { get; set; }
    }

    public class RequestSubmissionOPDModel_Report
    {
        public string RequestNo { get; set; }
        public string RegistrationNo { get; set; }
        public string HospitalType { get; set; }
        public string Gender { get; set; }
        public string PatientType { get; set; }
        public string PatientName { get; set; }
        public double PatientAge { get; set; }
        public string PatientAddress { get; set; }
        public string OPDNo { get; set; }
        public string OPDDate { get; set; }
        public double BillAmount { get; set; }
        public string BillDate { get; set; }
        public string TypeOfAddmission { get; set; }
        public string ClientName { get; set; }
        public string Comment { get; set; }
        public string NameOfDoctor1 { get; set; }
        public string NameOfDoctor2 { get; set; }
        public string NameOfDoctor3 { get; set; }
        public List<CommonMasterModel> DefaultServices { get; set; }
        public List<CommonMasterModel> ManullyAddedService { get; set; }
        public List<CommonMasterModel> ConsumeServices { get; set; }
        public List<BillSummary> BillSummary { get; set; }
        public string CompanyName { get; set; }
    }

}