using CGHSBilling.API.CommomArea.Interface;
using CGHSBilling.API.CommomArea.Repository;
using CGHSBilling.Areas.CommonArea.Models;
using CommonLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CGHSBilling.Areas.CommonArea.Controllers
{
    public class CircularController : Controller
    {
        ICircularInterface _Circular;
        private static readonly ILogger _logger = Logger.Register(typeof(CircularController));

        public CircularController(ICircularInterface Circular)
        {
            _Circular = Circular;
        }
        public CircularController()
        {
            _Circular = new CircularRepository();
        }

        public ActionResult GetCircularDetails()
        {
            JsonResult jResult;
            try
            {
                List<CircularModel> list = new List<CircularModel>();
                list = _Circular.GetCircularDetails();
                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error In GetCircularDetails: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                return Json("Error");
            }
        }

        [HttpGet]
        public ActionResult GetCircularDownloadFileDetails(int CircularID)
        {
            try
            {
                CircularModel circularData = new CircularModel();
                circularData.CircularModelData = _Circular.GetCircularDownloadFileDetails(CircularID);
                var supportFileLocation = circularData.CircularModelData[0].Location;
                var DownLoadFileNameAs = circularData.CircularModelData[0].DownloadFileNameAs;
                var fileDownloadName = Server.MapPath(supportFileLocation);
                var contentType = "application/pdf";
                return File(fileDownloadName, contentType, DownLoadFileNameAs);               
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in GetCircularDownloadFileDetails :" + ex.Message + Environment.NewLine + ex.StackTrace);
                throw ex;
            }
        }
    }
}
