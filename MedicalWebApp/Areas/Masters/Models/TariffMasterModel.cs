using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.Masters.Models
{
    public class TariffMasterModel
    {
        public int TariffMasterId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public int PatientTypeId { get; set; }
        public int RoomTypeId { get; set; }
        public int Sequence { get; set; }
        public bool Deactive { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string RoomType { get; set; }
        public string PatientType { get; set; }
        public int InsertedBy { get; set; }
        public DateTime? InsertedOn { get; set; }
        public string InsertedMacName { get; set; }
        public string InsertedMacId { get; set; }
        public string InsertedIpAddress { get; set; }
        public  List<TariffDetailModel> Tariffdtl { get; set; }
    }

    public class TariffDetailModel
    {
        public int TariffMasterId { get; set; }
        public int TariffDetailId { get; set; }
        public int ServiceId { get; set; }
        public double NABHRate { get; set; }
        public double NonNABHRate { get; set; }
        public double AIMSRate { get; set; }
        public string ServiceType { get; set; }
        public string ServiceName { get; set; }
        public string Code { get; set; }
        public double BillRate { get; set; }
        public int InsertedBy { get; set; }
        public DateTime? InsertedOn { get; set; }
        public string InsertedMacName { get; set; }
        public string InsertedMacId { get; set; }
        public string InsertedIpAddress { get; set; }
        public int RoomTypeId { get; set; }
    }
}