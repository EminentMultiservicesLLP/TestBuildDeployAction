using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.AdminPanel.Models
{
    public class UserCreationModel
    {
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public string PasswordValidate { get; set; }
        public int UserID { get; set; }
        public string UpdatedMacName { get; set; }
        public string UpdatedMacID { get; set; }
        public string UpdatedIPAddress { get; set; }
        public Nullable<int> UpdatedByUserID { get; set; }
        public Nullable<System.DateTime> UpdatedON { get; set; }
        public Nullable<int> InsertedBy { get; set; }
        public Nullable<System.DateTime> InsertedON { get; set; }
        public string InsertedMacName { get; set; }
        public string InsertedMacID { get; set; }
        public string InsertedIPAddress { get; set; }
        public string Message { get; set; }
        public bool IsDeactive { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string EmailID { get; set; }
        public string strLastLoginDate { get; set; }
        public string strExpiryDate { get; set; }
    }
}