using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace CGHSBilling.Areas.AdminPanel.Models
{
    public class ClientMasterModel
    {
        [JsonProperty("ClientId")]
        public int ClientId { get; set; }

        [JsonProperty("ClientCode")]
        [Required(ErrorMessage = " ")]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [JsonProperty("ClientName")]
        [Required(ErrorMessage = " ")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [JsonProperty("ExpiryDate")]
        [Required(ErrorMessage = " ")]
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }
        public string strExpiryDate { get; set; }

        [JsonProperty("Address")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [JsonProperty("Society")]
        [Display(Name = "Society Details")]
        public string Society { get; set; }

        [JsonProperty("Street")]
        [Display(Name = "Street Address")]
        public string Street { get; set; }

        [JsonProperty("Landmark")]
        [Display(Name = "Land Mark")]
        public string Landmark { get; set; }

        [JsonProperty("Pin")]
        [Display(Name = "Pincode")]
        public string Pincode { get; set; }

        [JsonProperty("ContactPerson")]
        [Display(Name = "Name")]
        public string ContactPerson { get; set; }

        [JsonProperty("ContactDesignation")]
        [Display(Name = "Designation")]
        public string ContactDesignation { get; set; }


        [JsonProperty("CreditPeriod")]
        [Display(Name = "Credit Period (In Days)")]
        public int? CreditPeriod { get; set; }

        [JsonProperty("DateOfAssociation")]
        [Display(Name = "Date Of Association")]
        public DateTime? DateOfAssociation { get; set; }

        [JsonProperty("GroupID")]
        public int? GroupID { get; set; }

        [JsonProperty("Fax")]
        [Display(Name = "Fax")]
        public string Fax { get; set; }

        [JsonProperty("Phone1")]
        [Display(Name = "Phone 1")]
        public string Phone1 { get; set; }

        [JsonProperty("Phone2")]
        [Display(Name = "Phone 2")]
        public string Phone2 { get; set; }

        [JsonProperty("CellPhone")]
        [Display(Name = "Mobile")]
        public string CellPhone { get; set; }

        [JsonProperty("Web")]
        [Display(Name = "Web Address")]
        public string Web { get; set; }

        [JsonProperty("Email")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }


        [JsonProperty("CST")]
        [Display(Name = "CST TIN No")]
        public string CST { get; set; }

        [JsonProperty("MST")]
        public string MST { get; set; }

        [JsonProperty("TDS")]
        public string TDS { get; set; }

        [JsonProperty("ExciseCode")]
        [Display(Name = "Excise Code")]
        public string ExciseCode { get; set; }

        [JsonProperty("ExportCode")]
        public string ExportCode { get; set; }

        [JsonProperty("LedgerID")]
        public int? LedgerID { get; set; }

        [JsonProperty("EligableForAdv")]
        [Display(Name = "Eligible For Advance")]
        public bool EligableForAdv { get; set; }

        [JsonProperty("BankName")]
        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [JsonProperty("BankAcNo")]
        [Display(Name = "Account No")]
        public string BankAcNo { get; set; }

        [JsonProperty("MICRNo")]
        [Display(Name = "MICR No.")]
        public string MICRNo { get; set; }

        [JsonProperty("BankBranch")]
        [Display(Name = "Branch")]
        public string BankBranch { get; set; }

        [JsonProperty("Note")]
        public string Note { get; set; }

        [JsonProperty("Proposed")]
        public string Proposed { get; set; }

        [JsonProperty("IncomeTaxNo")]
        [Display(Name = "Income Tax No")]
        public string IncomeTaxNo { get; set; }

        [JsonProperty("SuppType")]
        public string SuppType { get; set; }

        [JsonProperty("AccountId")]
        public int? AccountId { get; set; }

        [JsonProperty("Paytermsid")]
        public int? Paytermsid { get; set; }


        [JsonProperty("RTGSCODE")]
        [Display(Name = "RTGS Code")]
        public string RTGSCODE { get; set; }
        public string GSTIN { get; set; }

        [JsonProperty("IFSCCODE")]
        [Display(Name = "IFSC Code")]
        public string IFSCCODE { get; set; }

        [JsonProperty("ClientCategory")]
        public int? SupplierCategory { get; set; }

        [JsonProperty("SupplierType")]
        public int? SupplierType { get; set; }

        [JsonProperty("VATTINNo")]
        [Display(Name = "VAT TIN No")]
        public string VATTINNo { get; set; }

        [JsonProperty("ServiceTaxNo")]
        [Display(Name = "Service Tax Reg. No.")]
        public string ServiceTaxNo { get; set; }

        [JsonProperty("PANNo")]
        [Display(Name = "Income Tax PAN")]
        public string PANNo { get; set; }
        [JsonProperty("City")]
        [Required(ErrorMessage = " ")]
        [Display(Name = "City")]
        public int? City { get; set; }

        [JsonProperty("State")]
        [Display(Name = "State")]
        public int? State { get; set; }
       
        [JsonProperty("Village")]
        [Display(Name = "Village")]
        public string Village { get; set; }

        [JsonProperty("Country")]
        [Display(Name = "Country")]
        public int? Country { get; set; }

        [Display(Name = "Deduction Amt")]
        public double? DeductionAmt { get; set; }

        //[Display(Name = "No Of Bills")]
        //public int? NoOfBills { get; set; }

        public string DeductionType { get; set; }

        public int? DeductionModeId { get; set; }

        public string UpdatedMacName { get; set; }

        public string UpdatedMacID { get; set; }

        public string UpdatedIPAddress { get; set; }

        public Nullable<int> UpdatedBy { get; set; }

        public Nullable<System.DateTime> UpdatedOn { get; set; }

        public Nullable<int> InsertedBy { get; set; }

        public Nullable<System.DateTime> InsertedON { get; set; }

        public string InsertedMacName { get; set; }

        public string InsertedMacID { get; set; }

        public string InsertedIPAddress { get; set; }

        [Display(Name = "Deactive")]
        [JsonProperty("Deactive")]
        public bool Deactive { get; set; }
        public int? StateId { get; set; }
        public string StateName { get; set; }
        public int? CityId { get; set; }
        public string CityName { get; set; }

        //public List<CityMasterModel> Cities { get; set; }

        //[JsonProperty("States")]
        //public List<StateMasterModel> States { get; set; }

        public string strDateOfAssociation { get; set; }
        public string Message { get; set; }
        public string LogoPath { get; set; }

        public int? HospitalTypeId { get; set; }
        //public string HospitalType { get; set; }
        public int? HospitalServiceCategoryId { get; set; }
        public string HospitalServiceCategory { get; set; }

        public int?  ClientTypeId { get; set; }
        public string TypeName  { get; set; }

        [Display(Name = "Show Link")]
        public bool IsShowLnk { get; set; }

        public bool IsHopeClient { get; set; }
        public bool IsBothClient { get; set; }


    }

    public class ClientConfiguration
    {
        public int ConfigId { get; set; }
        public string ConfigName { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public List<ClientConfigurationDetails> ConfigList { get; set; }
        public bool Deactive { get; set; }
        public Nullable<int> InsertedBy { get; set; }
        public Nullable<System.DateTime> InsertedON { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedON { get; set; }
        public string Message { get; set; }
    }
    public class ClientConfigurationDetails
    {
        public int ServiceTypeId { get; set; }
        public string ServiceType { get; set; }
        public decimal? Percentage { get; set; } = 0;
    }
}