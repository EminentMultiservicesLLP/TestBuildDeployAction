using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.CommonArea.Models
{
    public class RoomEntitlement
    {
        public int BillType { get; set; }
        public int YearOfRetirement  { get; set; }        
        public string LastSalaryDrawn { get; set; }
       

        public string BillName { get; set; }
        public string RoomType{ get; set; }

    }

}