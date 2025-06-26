using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.QueryCollection.AdminPanel
{
    public class AdminPanelQueries
    {

        public const string GetUserCode = "dbsp_ap_GetUserCode";
        public const string GetUserDetails = "dbsp_ap_GetUserDetails";
        public const string SaveUser = "dbsp_ap_SaveUser";
        public const string GetClientTracker = "dbsp_ap_GetClientTracker";


        public const string GetMenuByUser = "dbsp_ap_GetMenuByUser";
        public const string SaveUserAccess = "dbsp_ap_SaveUserAccess";
        public const string GetAllStdSub = "dbsp_ap_GetAllStdSub";

        public const string InsertClient = "dbsp_ap_InsUpdClient";
        public const string GetAllClient = "dbsp_ap_GetAllClient";
        public const string GetAllStates = "dbsp_ap_GetAllStates";
        public const string GetCityById = "dbsp_ap_GetCityById";
        public const string SaveSubStandardAccess = "dbsp_ap_SaveSubStandardAccess";
        public const string GetStdSubjectAccess = "dbsp_ap_GetStdSubjectAccess";
        public const string GetUserDetailsByUserId = "dbsp_ap_GetUserDetailsByUserId";
        public const string CreateScandoc = "dbsp_mst_CreateScandoc";
        public const string GetScanDocUrl = "dbsp_GetAllScandocs";
        public const string SaveBillingConfiguration = "dbsp_SaveBillingConfiguration";
        public const string GetAllBill = "dbsp_GetAllBill";
        public const string GetAllDeductedBills = "dbsp_GetAllDeductedBills";
        public const string SaveSignUpDetails = "dbsp_SaveSignUpDetails";

        public const string UpdatePassword = "dbsp_mst_SaveForgotPassword";
        public const string SaveResetPassword = "dbsp_mst_SaveResetPassword";

        public const string SaveClientConfiguration = "dbsp_SaveClientConfiguration";
        public const string GetClientConfiguration = "dbsp_GetClientConfiguration";
        public const string GetClientConfigDetails = "dbsp_GetClientConfigDetails";
    }
}