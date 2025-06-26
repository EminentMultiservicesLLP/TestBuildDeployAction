using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.Masters.Models
{
    public class ServiceMasterModel
    {
        public int ServiceId { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceType { get; set; }
        public string Code { get; set; }
        public string ServiceName { get; set; }
        public int CategoryId { get; set; }
        public double BillRate { get; set; }
        public string CategoryName { get; set; }
        public int Sequence { get; set; }
        public string ConsumeDate { get; set; }
        public string RoomType { get; set; }
        public int RoomTypeId { get; set; }
        public bool Deactive { get; set; }
        [Display(Name = "Allow to Change Rate")]
        public bool AllowToChangeRate { get; set; }
        [Display(Name = "Set As Default")]
        public bool Default { get; set; }
        public bool Surgery { get; set; }
        [Display(Name = "Number of Days")]
        public int NoOfDays { get; set; }
        public int InsertedBy { get; set; }
        public DateTime? InsertedOn { get; set; }
        public string InsertedMacName { get; set; }
        public string InsertedMacId { get; set; }
        public string InsertedIpAddress { get; set; }
        public bool State { get; set; }
        public int Qty { get; set; }
        public string CghsCode { get; set; }
        public int GenderId { get; set; }
        public int PatientTypeId { get; set; }
        public int HospitalTypeId { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public bool IsDefaultService { get; set; }
        public List<ServiceGenderLinking> ServiceGender { get; set; }
        public bool IsAllowedChangeInSurgery { get; set; }
    }

    public class ServiceGenderLinking
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public bool State { get; set; }

    }
}