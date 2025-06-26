using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.Masters.Models
{
    public class HospitalServicelinkingModel
    {
        public int HospitalServiceCategoryId { get; set; }
        public string HospitalServiceCategory { get; set; }
        public int ManagementTypeId { get; set; }
        public string ManagementType { get; set; }
        public List<HospitalServicelinkingModel> ServiceType_ManagementTypeData { get; set; }
        public bool State { get; set; }
    }

    //public class ServiceManagementTypeLinkingModel
    //{
    //    public int HospitalServiceCategoryId { get; set; }
    //    public int ManagementTypeId { get; set; }
    //    public string ManagementType { get; set; }
    //    public bool State { get; set; }
    //    public string HospitalServiceCategory { get; set; }
    //}
}