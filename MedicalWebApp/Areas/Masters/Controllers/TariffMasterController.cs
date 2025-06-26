using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGHSBilling.API.Masters.Interfaces;
using CGHSBilling.API.Masters.Repositories;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Caching;
using CGHSBilling.Common;
using CGHSBilling.Models;
using CommonLayer;

namespace CGHSBilling.Areas.Masters.Controllers
{
    public class TariffMasterController : Controller
    {
        ITariffMasterRepository _service;
        private static readonly ILogger Loggger = Logger.Register(typeof(TariffMasterController));

        public TariffMasterController()
        {
            _service = new TariffMasterRepository();
        }
        [HttpPost]
        public ActionResult Savesertariff(TariffMasterModel jsonData)
        {
            JsonResult jlResult;
            try
            {
                jsonData.InsertedBy = Convert.ToInt32(Session["AppUserId"]);
                jsonData.InsertedOn = System.DateTime.Now;
                jsonData.InsertedIpAddress = Constants.IpAddress;
                jsonData.InsertedMacId = Constants.MacId;
                jsonData.InsertedMacName = Constants.MacName;
                if (jsonData.TariffMasterId > 0)
                {
                    var save = _service.UpdateTariffMaster(jsonData);
                }
                else
                {
                    var save = _service.SaveTariff(jsonData);
                }
                jlResult = Json(new { Message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in TariffMasterController  Savesertariff:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }
        [HttpPost]
        public ActionResult SaveCopytariff(TariffMasterModel jsonData)
        {
            JsonResult jlResult;
            try
            {
                jsonData.InsertedBy = Convert.ToInt32(Session["AppUserId"]);
                jsonData.InsertedOn = System.DateTime.Now;
                jsonData.InsertedIpAddress = Constants.IpAddress;
                jsonData.InsertedMacId = Constants.MacId;
                jsonData.InsertedMacName = Constants.MacName;
                var save = _service.SaveTariff(jsonData);
                //var temp = jsonData.Tariffdtl.GroupBy(m => m.RoomTypeId).ToList();
                //foreach (var detail in temp)
                //{
                //    foreach (var item in detail)
                //    {
                //        jsonData.RoomTypeId = item.RoomTypeId;
                //    }
                //    jsonData.Tariffdtl = detail as List<TariffDetailModel>;
                //    var save = _service.SaveTariff(jsonData);

                //}
                jlResult = Json(new { Message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in TariffMasterController  Savesertariff:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }
        public ActionResult GetTariffMaster()
        {
            JsonResult jlResult;
            try
            {
                List<TariffMasterModel> data = new List<TariffMasterModel>();
                data = _service.GetTariffMaster();
                var jResult = Json(data, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in TariffMasterController  GetTariffMaster:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong !!!", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }
        public ActionResult GetTariffDetailById(int tariffMasterId)
        {
            JsonResult jlResult;
            try
            {
                List<TariffDetailModel> data = new List<TariffDetailModel>();
                data = _service.GetTariffDetailById(tariffMasterId);
                var jResult = Json(data, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in TariffMasterController  GetTariffDetailById:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }

        public ActionResult GetTariffMasterforCopy(int stateId,int cityId,int patientType,int roomType)
        {
            JsonResult jlResult;
            try
            {
                List<TariffDetailModel> data = new List<TariffDetailModel>();
                data = _service.GetTariffMasterforCopy(stateId,cityId,patientType,roomType);
                var jResult = Json(data, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in TariffMasterController  GetTariffDetailById:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }
        public ActionResult GetTariff_forCopy(int stateId, int cityId, int patientType, int roomType)
        {
            JsonResult jlResult;
            try
            {
                List<TariffDetailModel> data = new List<TariffDetailModel>();
                data = _service.GetTariff_forCopy(stateId, cityId, patientType, roomType);
                var jResult = Json(data, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in TariffMasterController  GetTariff_forCopy:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }
        public ActionResult GetTariffDetails_forCopy(string TariffMasterIds)
        {
            JsonResult jlResult;
            try
            {
                List<TariffDetailModel> data = new List<TariffDetailModel>();
                data = _service.GetTariffDetails_forCopy(TariffMasterIds);
                var jResult = Json(data, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in TariffMasterController  GetTariffDetails_forCopy:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }
    }
}
