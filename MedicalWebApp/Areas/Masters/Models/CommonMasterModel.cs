namespace CGHSBilling.Areas.Masters.Models
{
    public class CommonMasterModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Sequence { get; set; }
        public bool Deactive{ get; set; }
        public int TransactionId { get; set; }
        public int ServiceId { get; set; }
        public int ParentServiceId { get; set; }
        public string ServiceName { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceType { get; set; }
        public bool State { get; set; }
        public double BillRate { get; set; }
        public double LifeSavingBillRate { get; set; }
        public int RoomTypeId { get; set; }
        public int Qty { get; set; }
        public string CghsCode { get; set; }
        public int RoomPriorityLevel{ get; set; }
        public string ConsumeDate { get; set; }
        public bool IsAllowedChangeInSurgery { get; set; }
        public bool IsValidForEntitlement { get; set; }
        public string ReportHeading { get; set; }
        public bool IsClientState { get; set; }
        public bool IsClientCity { get; set; }
    }
}