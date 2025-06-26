using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.API.ScanDoc
{
    public class ScanDocEntity
    {
        public int ScanDocId { get; set; }
        public int ScanDocTypeId { get; set; }
        public string ScanDocType { get; set; }
        public int ScanDocSubTypeId { get; set; }
        public string ScanDocSubType { get; set; }
        public int FileId { get; set; }
        public string FileNames { get; set; }
        public string FilePath { get; set; }
        public int InsertedBy { get; set; }
        public Nullable<DateTime> InsertedON { get; set; }
        public string InsertedMacName { get; set; }
        public string InsertedMacID { get; set; }
        public string InsertedIPAddress { get; set; }
        public bool Deactive { get; set; }
    }
}