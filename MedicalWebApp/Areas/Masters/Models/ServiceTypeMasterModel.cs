using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.Masters.Models
{
    public class ServiceTypeMasterModel
    {
        public int ServiceTypeId { get; set; }
        public string Code { get; set; }
        public string ServiceType { get; set; }
        public int Sequence { get; set; }
        public bool Deactive { get; set; }
        public int InsertedBy { get; set; }
        public DateTime? InsertedOn { get; set; }
        public string InsertedMacName { get; set; }
        public string InsertedMacId { get; set; }
        public string InsertedIpAddress { get; set; }
        public int CategoryId { get; set; }
        public List<ServiceCategoryLinkingModel> Category { get; set; }
        public List<SurgeryMasterModel> Surgery { get; set; }
    }

    public class ServiceCategoryLinkingModel
    {
        public int ServiceTypeId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool State { get; set; }
    }

    public class SurgeryMasterModel
    {
        public int SurgeryID { get; set; }
        public string SurgeryName { get; set; }
        public bool IsCancerRelated { get; set; }
        public string SurgeryDescription { get; set; }
        public int SurgeryTypeID { get; set; }
        public int NoOfDays { get; set; }
        public bool Deactive { get; set; }
    }


    public class CancerSurgeryModel
    {
        public int CancerSurgeryId { get; set; }
        public string Category { get; set; }
        public string SurgeryName { get; set; }
        public string Grade { get; set; }
    }
}