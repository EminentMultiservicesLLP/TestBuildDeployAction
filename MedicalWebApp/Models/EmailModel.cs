using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CGHSBilling.Models
{
    public class EmailModel
    {
        public string Name { get; set; }


         [Display(Name = "Email")]
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        [Display(Name = "Subject")]
        public string EmailSubject { get; set; }
        
        [Display(Name = "Description")]
        public string EmailBody { get; set; }

        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        [Display(Name = "Contact No")]
        public string Phone { get; set; }
    }
}