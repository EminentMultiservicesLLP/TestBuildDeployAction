using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CGHSBilling.Areas.Masters.Models;

namespace CGHSBilling.Areas.HospitalForms.Models
{
    public class RequestSubmissionModel
    {
        public int RequestId { get; set; }

        public string RequestNo { get; set; }
        [Required]
        public string FileNo { get; set; }
        [Required]
        public int HospitalTypeId { get; set; }
        public string HospitalType { get; set; }
        [Required]
        public int ManagementTypeId { get; set; }
        public string ManagementType { get; set; }
        [Required]
        public int RoomEntitleTypeId { get; set; }
        public string RoomEntitleType { get; set; }
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
        public string IpdNo{ get; set; }
        public double BillAmount { get; set; }
        public int InsertedBy { get; set; }
        public DateTime? InsertedOn { get; set; }
        public string StrInsertedOn { get; set; }
        public string StrAdmissionDate { get; set; }
        public string StrDischrgeDate { get; set; }
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
        public string LeftDcDetail { get; set; }
        public bool IgnoreSurgeryValidation { get; set; }
        public List<string> TotalDates { get; set; }
        public List<BedCharges> BedCharges { get; set; }
        public  List<AdmissionSummary> AdmissionSummaries { get; set; }
        public List<SurgerySummary> SurgerySummaries { get; set; }
        public List<CommonMasterModel> ConsumeDiv { get; set; }
        public List<CommonMasterModel> PharmacyDetails { get; set; }
        public List<CommonMasterModel> ManullyAddedService { get; set; }
        public List<SurgeryManualServices> SurgeryManullyAddedService { get; set; }
        public bool IsLumpsum { get; set; }
        //public List<BillSummary> BillSummary { get; set; }

        public bool   IsDeactive { get; set; }
        public string Comment { get; set; }
        public string DoctorIncharge { get; set; }
        public string RegistrationNo { get; set; }
        public bool IsReportPrinted { get; set; }
        public PatientModel Patient { get; set; }
        public bool IsHopePatientBill { get; set; }
    }

    public class RequestSubmissionModel_Report
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
        public string IpdNo { get; set; }
        public double BillAmount { get; set; }
        public string BillDate { get; set; }
        public string StrAdmissionDate { get; set; }
        public string StrDischrgeDate { get; set; }
        public string RoomType { get; set; }
        public string TypeOfAddmission { get; set; }
        public string ClientName { get; set; }
        public string LeftDcDetail { get; set; }
        public List<BedCharges> BedCharges { get; set; }
        public List<SurgerySummary> SurgerySummaries { get; set; }
        public List<CommonMasterModel> PharmacyDetails { get; set; }
        public List<CommonMasterModel> ManullyAddedService { get; set; }
        public List<CommonMasterModel> DefaultService { get; set; }
        public List<CommonMasterModel> LinkedService { get; set; }
        public List<CommonMasterModel> Investigation { get; set; }
        public List<SurgeryManualServices> SurgeryManullyAddedService { get; set; }
        public List<BillSummary> BillSummary { get; set; }
        public string DoctorIncharge { get; set; }
        public string RegistrationNo { get; set; }
        public bool IsReportPrinted { get; set; }
     
    }

    public class AdmissionSummary
    {
        public int RequestId { get; set; }
        public int RoomTypeId { get; set; }
        public double Qty { get; set; }
        public string RoomType { get; set; }
        public double BillRate { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceCode { get; set; }
        public int OtType { get; set; }
        public DateTime AdmissionDateTime { get; set; }
        public DateTime DischargeDateTime { get; set; }
        public string StrAdmissionDateTime { get; set; }
        public string StrDischargeDateTime { get; set; }
        public string SurgeryAdmissionDateTime { get; set; }
        public string SurgerydDischargeDateTime { get; set; }
    }

    public class SurgerySummary
    {
        public int RequestId { get; set; }
        public int SurgeryID { get; set; }
        public string SurgeryName { get; set; }
        public bool IsCancerSurgery { get; set; }
        public string RoomType { get; set; }
        public double BillRate { get; set; }
        public DateTime SurgeryDateTime { get; set; }
        public string StrSurgeryDateTime { get; set; }
        public string CghsCode { get; set; }
        public int NoOfDays { get; set; }
        public int Qty { get; set; }
        public int ServiceTypeId { get; set; }
    }

    public class BedCharges
    {
        public int RequestId { get; set; }
        public int RoomTypeId { get; set; }
        public int Qty { get; set; }
        public string ServiceName { get; set; }
        public string ConsumeDate { get; set; }
        public double BillRate { get; set; }
        public string CghsCode { get; set; }
    }

    public class BillSummary
    {
        public string ServiceName { get; set; }
        public double BillAmount { get; set; }
    }

    public class DefaultService
    {
        public int RequestId { get; set; }
        public string ServiceType { get; set; }
        public string ServiceName { get; set; }
        public double BillRate { get; set; }
        public int Qty { get; set; }
    }

    public class SurgeryManualServices
    {
        public int RequestId { get; set; }
        public string SurgeryName { get; set; }
        public string ConsumeDate { get; set; }
        public DateTime ConsumeDateTime { get; set; }
        public double OTCharges { get; set; }
        public double AnesthesiaCharges { get; set; }
        public double SurgeonCharges { get; set; }
        public double OtherCharges { get; set; }
        public double ExtraCharges { get; set; }
    }


    public class PatientModel
    {
        public string RegName { get; set; }
        public int PatientId { get; set; }
        public int ParentPatientId { get; set; }
        public bool IsDependent { get; set; }
        public string CompanyName { get; set; }
        public string PatientName { get; set; }
        public double Age { get; set; }
        public int GenderId { get; set; }
        public string Address { get; set; }
        public int RoomTypeId { get; set; }
        public int RequestId { get; set; }

    }

    public class RequestSubmissionBillNoModel
    {
        public string RequestNo { get; set; }
        public int RequestId { get; set; }
    }
}