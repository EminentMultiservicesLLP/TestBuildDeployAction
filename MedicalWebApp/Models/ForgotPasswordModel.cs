using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CGHSBilling.Models
{
    public class ForgotPasswordModel
    {
        public string UserName { get; set; }
        public int UserID { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string EmailID { get; set; }
        public int LoginFor { get; set; }
        [Display(Name = "Email")]
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        [Display(Name = "Subject")]
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
    }
}