using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.HospitalForms.Models
{
    public class ServicesConsumedModel
    {
       
    }
    public class CommonService
    {
        public int  ID { get; set; }
        public int RequestId { get; set; }
        public int ServiceId { get; set; }
        public bool State { get; set; }
    }

}