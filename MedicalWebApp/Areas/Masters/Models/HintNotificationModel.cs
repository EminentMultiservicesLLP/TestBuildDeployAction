using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.Masters.Models
{
    public class HintNotificationModel
    {
        public int NotificationId { get; set; }
        public string ControlId { get; set; }
        public string ControlName { get; set; }
        public string Message { get; set; }
        public int StepNo { get; set; }
        public int SubControlId { get; set; }
    }
}