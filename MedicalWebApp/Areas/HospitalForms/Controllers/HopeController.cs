using CGHSBilling.API.HospitalForms.Intefaces;
using CGHSBilling.API.HospitalForms.Repository;
using CGHSBilling.Areas.HospitalForms.Models;
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
    public class HopeController : Controller
    {
        IHopeRepository _data;
        private static readonly ILogger Loggger = Logger.Register(typeof(HopeController));
        ConnectionString _connectionString;


        public HopeController()
        {
            _data = new HopeRepository();
            _connectionString = new ConnectionString();
        }

        public ActionResult GetHopePatients()
        {
            Loggger.LogInfo("GetHopePatients Started for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            JsonResult jlResult;
            List<PatientModel> list = new List<PatientModel>();
            string ConnectionString = _connectionString.getHopeConnectionString();
            try
            {

                list = _data.GetHopePatients(ConnectionString);
                jlResult = Json(list, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetHopePatients :", ex);
                return Json("error");
            }
            finally
            {
                Loggger.LogInfo("GetHopePatients Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            }
            return jlResult;
        }


        public ActionResult GetClientConfiguration() {
            JsonResult jsonResult = null;         
            TryCatch.Run(() =>
            {
               int UserId = Convert.ToInt32(Session["AppUserId"]);           
               var ClientModel =  _data.GetClientConfiguration(UserId);

           jsonResult = Json(new { isHopeClient = ClientModel.IsHopeClient,isShowLnk = ClientModel.IsShowLnk, IsBothClient =ClientModel.IsBothClient }, JsonRequestBehavior.AllowGet);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in class:HopeController method :GetClientConfiguration :", ex);
            });


            Loggger.LogInfo("GetClientConfiguration Completed for " + Convert.ToInt32(Session["AppUserId"]) + " at :" + DateTime.Now.ToLongTimeString());
            return jsonResult;
        }

    }
}
