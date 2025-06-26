using CGHSBilling.API.AdminPanel.Interfaces;
using CGHSBilling.API.AdminPanel.Repositories;
using CGHSBilling.Areas.CommonArea.Models;
using CommonLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CGHSBilling.Common;
using CGHSBilling.Controllers;

namespace CGHSBilling.Areas.AdminPanel.Controllers
{
    public class BillingConfigurationController : Controller
    {
        IBillingConfigurationRepository _action;
        private static readonly ILogger Loggger = Logger.Register(typeof(BillingConfigurationController));
        ConnectionString _ConnectionString;
        public BillingConfigurationController()
        {
            _action = new BillingConfigurationRepository();
            _ConnectionString = new ConnectionString();
        }
        public async Task<JsonResult> SaveBillingConfiguration(BillConfigurationModel model)
        {

            bool isSuccess = false;
            model.InsertedBy = Convert.ToInt32(Session["AppUserId"].ToString());
            model.InsertedOn = DateTime.Now;
            model.InsertedIpAddress= Common.Constants.IpAddress;
            model.InsertedMacId = Common.Constants.MacId;
            model.InsertedMacName = Common.Constants.MacName;
            string ConnectionString = _ConnectionString.getConnectionStringName();
            try
            {
                double? total = AddNullableDoubles(model.RecieveAmount , model.BalanceAmount);
                model.BalanceAmount = total;
                isSuccess = _action.SaveBillingConfiguration(model, ConnectionString);
            }
            catch (Exception ex)
            {

                Loggger.LogError("Error in Save Client :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }

            if (!isSuccess)
                return Json(new { success = false, message = "Operation failed" });
            else
                return Json(new { success = true, message = "Record saved successfully"});
        }

        public ActionResult GetAllBill(int ClientId)
        {
            try
            {
                var balance = GetBalanceAmount(ClientId,false);
                return balance;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllBill :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }
        }

        public ActionResult GetBalanceAmount(int ClientId,bool IsSendMail)
        {
            JsonResult jResult;
            List<BillConfigurationModel> list = new List<BillConfigurationModel>();
            try
            {
                if (ClientId == 0)
                    ClientId = Convert.ToInt32(Session["ClientId"]);
                list = _action.GetAllBill(ClientId);
                list[0].Billdetail = _action.GetAllDeductedBills(ClientId);
                list[0].TotalRecieveAmount = list.Sum(o => o.RecieveAmount);
                list[0].TotalAmount = 0;
                foreach (var item in list[0].Billdetail)
                {
                    list[0].TotalAmount = AddNullableDoubles(list[0].TotalAmount, item.DeductedAmount);
                }
                list[0].BalanceAmount = list[0].TotalRecieveAmount - list[0].TotalAmount;
              
                if (IsSendMail && list[0].BalanceAmount <0 && list[0].DeductionModeId!= 3)
                {                    
                    new EmailController().LimitExceededMail();
                }

                System.Web.HttpContext.Current.Session["DeductionMode"] = list[0].DeductionModeId;

                if (list[0].DeductionModeId == 1)
                {
                   
                    System.Web.HttpContext.Current.Session["BalanceAmount"] = "Rs. : " + list[0].BalanceAmount.ToString();
                }
                else if(list[0].DeductionModeId == 2)
                {                 
                    System.Web.HttpContext.Current.Session["BalanceAmount"] = ("Bills: ") + list[0].BalanceAmount.ToString();
                }
                else
                {
                    System.Web.HttpContext.Current.Session["BalanceAmount"] = "";
                }

                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetBalanceAmount :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }
        }

        static double? AddNullableDoubles(double? a, double? b)
        {
            if (!a.HasValue && !b.HasValue)  // guard clause, if they are both null return null
                return a;

            double aNum = a.HasValue ? a.Value : 0;  // if a has a value, assign it to aNum, if not assign 0 to aNum
            double bNum = b.HasValue ? b.Value : 0;  // same thing for b

            return aNum + bNum;
        }

    }
}
