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
    public class CertificateController : Controller
    {
        ICertificateInterface _data;
        private static readonly ILogger Loggger = Logger.Register(typeof(HopeController));
        ConnectionString _connectionString;
        //
        // GET: /HospitalForms/Certificate/
        public CertificateController()
        {
            _data = new CertificateRepository();
            _connectionString = new ConnectionString();
        }
        public ActionResult GetCertificateDetail(BCertificateModel model)
        {
            JsonResult jsonResult = null;
            List<BCertificateModel> list = new List<BCertificateModel>();
            TryCatch.Run(() =>
            {
                list = _data.GetAllDetail(model.RequestId);
                jsonResult = Json(list, JsonRequestBehavior.AllowGet);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in class:CommonMasterController method :GetAllDetail :" + Environment.NewLine + ex.StackTrace);
            });

            return jsonResult;
        }
         public ActionResult SaveCertificate(BCertificateModel model)
        {
            
            DateTime oDate = Convert.ToDateTime(model.AdmissionTime);
            model.AdmissionDateTime = oDate;
            DateTime oDate1 = Convert.ToDateTime(model.DischargeTime);
            model.DischargeDateTime = oDate1;
       



            try
            {
               

                


                    var newId = _data.CreateCertificate(model);
                   




               

                
            }
            catch (Exception ex)
            {

                Loggger.LogError("Error in Save Client :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }
            return Json(new { success = true });

        }
        public ActionResult Index()
        {
            return View();
        }

    }
}
