using CGHSBilling.API.Masters.Interfaces;
using CGHSBilling.API.Masters.Repositories;
using CGHSBilling.Areas.Masters.Models;
using CommonLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CGHSBilling.Areas.Masters.Controllers
{
    public class UpdateRatesController : Controller
    {
        //
        IUpdateRates _updateRates;
        private static readonly ILogger Loggger = Logger.Register(typeof(ServiceMasterController));

        public UpdateRatesController()
        {
            _updateRates = new UpdateRatesRepository();
        }
        // GET: /Masters/UpdateRates/

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetRates(int StateId,int CityId)
        {
            JsonResult jlResult;
            List<UpdateRatesModel> list = new List<UpdateRatesModel>();
            try
            {                
                list = _updateRates.GetRates(StateId, CityId);
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = Json(new {data= jResult.Data,   success = true }, JsonRequestBehavior.AllowGet);
                jlResult.MaxJsonLength = int.MaxValue;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetRates :" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new {  success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }

        [HttpGet]
        public ActionResult GetServiceTariff(int? ServiceId)
        {
            JsonResult jlResult;
            List<UpdateRatesModel> list = new List<UpdateRatesModel>();
            try
            {
                list = _updateRates.GetServiceTariff(ServiceId);
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = Json(new { data = jResult.Data, success = true }, JsonRequestBehavior.AllowGet);
                jlResult.MaxJsonLength = int.MaxValue;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetServiceTariff :" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }

        [HttpPost]
        public ActionResult UpdateRates(List<UpdateRatesModel> UpdateRates)
        {
            JsonResult jlResult;
            int result = _updateRates.UpdateRates(UpdateRates);
            if (result>0 )
            jlResult = Json(new { success = true }, JsonRequestBehavior.AllowGet);
            else jlResult = Json(new { success = false }, JsonRequestBehavior.AllowGet);
            return jlResult;
        }
    }
}
