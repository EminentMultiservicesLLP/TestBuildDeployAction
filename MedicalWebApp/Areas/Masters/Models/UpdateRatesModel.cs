using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.Masters.Models
{
    public class UpdateRatesModel
    {
        public int TariffDtlId { get; set; }
        public int ServiceId { get; set; }
        public int TariffMasterId { get; set; }
        public string ServiceName { get; set; }
        public string RoomType { get; set; }
        public string PatientType { get; set; }
        public string ServiceType { get; set; }
        public string Code { get; set; }
        public double NABHRate { get; set; }
        public double NonNABHRate { get; set; }
        public string CghsCode { get; set; }
        public string CityName { get; set; }
    }
}