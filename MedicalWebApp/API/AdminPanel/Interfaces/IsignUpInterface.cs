using CGHSBilling.Models;
using CommonDataLayer.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.API.AdminPanel.Interfaces
{
    public interface IsignUpInterface
    {
       int SaveSignUpDetails(SignUpModel model);
       bool CheckDuplicateItem(string SignUpContactName, string SignUpHospitalName, string EmailID, int typeid, string type);
        int SaveResetPassword(ResetPasswordModel model);
    }
}