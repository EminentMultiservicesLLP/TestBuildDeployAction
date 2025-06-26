using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.CommonArea.Models
{
    public class BillConfigurationModel
    {
        public int BillConfigurationId { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public double? RecieveAmount { get; set; }
        public double? BalanceAmount { get; set; }
        public double? TotalRecieveAmount { get; set; }
        public double? TotalAmount { get; set; }
        public double? TotalDeduction { get; set; }
        public int? InsertedBy { get; set; }
        public DateTime? InsertedOn { get; set; }
        public string InsertedMacName { get; set; }
        public string InsertedMacId { get; set; }
        public string InsertedIpAddress { get; set; }
        public bool IsDeactive { get; set; }
        public string BillDate { get; set; }
        public string Comment { get; set; }      
        public int DeductionModeId { get; set; }
        public double? DeductedAmount { get; set; }
        public List<BillConfigurationDtlModel> Billdetail { get; set; }
    }

    public class BillConfigurationDtlModel
    {
        public int BillConfigurationId { get; set; }
        public string RequestNo { get; set; }
        public double? DeductedAmount { get; set; }
        public double? BalanceAmount { get; set; }
        public int DeductionModeId { get; set; }

    }
}