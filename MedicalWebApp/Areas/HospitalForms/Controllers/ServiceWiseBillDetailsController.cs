using CGHSBilling.API.HospitalForms.Intefaces;
using CGHSBilling.API.HospitalForms.Repository;
using CGHSBilling.Areas.HospitalForms.Models;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Common;
using CommonLayer;
using CommonLayer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static CGHSBilling.Controllers.HomeController;

namespace CGHSBilling.Areas.HospitalForms.Controllers
{
    [ValidateHeaderAntiForgeryTokenAttribute]
    public class ServiceWiseBillDetailsController : Controller
    {
        IServiceWiseBillDetailsRepository _Billdata;
        private static readonly ILogger Loggger = Logger.Register(typeof(ServiceWiseBillDetailsController));
        ConnectionString _connectionString;

        public ServiceWiseBillDetailsController()
        {
            _Billdata = new ServiceWiseBillDetailsRepository();
            _connectionString = new ConnectionString();
        }

        public ActionResult GetServiceMasterList()
        {
            Loggger.LogInfo("GetServiceMasterList Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            JsonResult jsonResult = null;
            List<ServiceWiseBillDetailsModel> list = new List<ServiceWiseBillDetailsModel>();
            TryCatch.Run(() =>
            {
                list = _Billdata.GetServiceMasterList();
                jsonResult = Json(new { data = list }, JsonRequestBehavior.AllowGet);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in class:ServiceWiseBillDetailsController method :GetServiceMasterList :", ex);
            });
            Loggger.LogInfo("GetServiceMasterList Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            return jsonResult;
        }

        public ActionResult GetAllCategories()
        {
            Loggger.LogInfo("GetAllCategories Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            JsonResult jsonResult = null;
            List<CommonMasterModel> list = new List<CommonMasterModel>();
            TryCatch.Run(() =>
            {
                list = _Billdata.GetAllCategories();
                jsonResult = Json(new { data = list }, JsonRequestBehavior.AllowGet);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in class:ServiceWiseBillDetailsController method :GetAllCategories :", ex);
            });
            Loggger.LogInfo("GetAllCategories Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            return jsonResult;
        }


        [HttpGet]
        public ActionResult GetAllBillNo()
        {         
            JsonResult jsonResult = null;
            int userid = Convert.ToInt32(Session["AppUserId"]);
            List<RequestSubmissionBillNoModel> list = new List<RequestSubmissionBillNoModel>();
            TryCatch.Run(() =>
            {
                 list = _Billdata.GetAllBillNo(userid);
                jsonResult = Json(new { data = list }, JsonRequestBehavior.AllowGet);
            }).IfNotNull(ex =>
            {
               Loggger.LogError("Error in class:ServiceWiseBillDetailsController method :GetAllBillNo :", ex);
            });
            
            return jsonResult;
        }



    }
}
