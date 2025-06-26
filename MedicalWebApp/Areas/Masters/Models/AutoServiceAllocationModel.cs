using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.Masters.Models
{
    public class AutoServiceAllocationModel
    {
        public int AutoAllocationId { get; set; }
        public int ServiceTypeId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public int ServiceDaysId { get; set; }
        public int InsertedBy { get; set; }
        public string ServiceType { get; set; }
        public DateTime? InsertedOn { get; set; }
        public string InsertedMacName { get; set; }
        public string InsertedMacId { get; set; }
        public string InsertedIpAddress { get; set; }
        public List<AutoServiceAllocationDtlModel> AllocationDtl { get; set; }
        public List<AutoServiceAllocationDtlModel> AllocationLeftDtl { get; set; }
    }
    public class AutoServiceAllocationDtlModel
    {
        public int AutoAllocationDtlId { get; set; }
        public int AutoAllocationId { get; set; }
        public int LinkedWithServiceTypeId { get; set; }
        public int ServiceId { get; set; }
        public int Qty { get; set; }
        public string ServiceName { get; set; }
        public bool State { get; set; }
        public string MasterServiceName { get; set; }
        public bool AllowToChangeRate { get; set; }
        public double BillRate { get; set; }
        public double ChangeBillRate { get; set; }
    }
}