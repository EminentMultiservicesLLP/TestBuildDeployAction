using CGHSBilling.API.Masters.Interfaces;
using CGHSBilling.API.Masters.Repositories;
using CGHSBilling.Areas.Masters.Models;
using CommonLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGHSBilling.Common;

namespace CGHSBilling.Areas.Masters.Controllers
{
    public class AutoServiceAllocationController : Controller
    {
        IAutoServiceAllocationRepository _service;
        private static readonly ILogger Loggger = Logger.Register(typeof(ServiceMasterController));

        public AutoServiceAllocationController()
        {
            _service = new AutoServiceAllocationRepository();
        }
        [HttpPost]
        public ActionResult SaveAutoServiceAllocation(AutoServiceAllocationModel jsonData)
        {
            JsonResult jlResult; bool success = false;
            try
            {
                jsonData.InsertedBy = Convert.ToInt32(Session["AppUserId"]);
                jsonData.InsertedOn = System.DateTime.Now;
                jsonData.InsertedIpAddress = Constants.IpAddress.ToString();
                jsonData.InsertedMacId = Constants.MacId.ToString();
                jsonData.InsertedMacName = Constants.MacName.ToString();
                foreach (var list in jsonData.AllocationLeftDtl)
                {
                    jsonData.ServiceId = list.ServiceId;
                    if (jsonData.AutoAllocationId == 0)
                    {
                        var save = _service.SaveAutoServiceAllocation(jsonData);
                        success = true;
                    }
                    else
                    {
                        var save = _service.UpdateAutoServiceAllocation(jsonData);
                        success = true;
                    }
                }
                jlResult = Json(new { Message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in AutoServiceAllocationController  SaveAutoServiceAllocation:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }

        public ActionResult GetAllAutoServiceAllocation()
        {
            JsonResult jlResult;
            {
                List<AutoServiceAllocationModel> serviceType = new List<AutoServiceAllocationModel>();
                serviceType = _service.GetAutoServiceAllocation();
                var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }

        public ActionResult GetDaysDropdwon()
        {
            JsonResult jlResult;
            {
                List<CommonMasterModel> serviceType = new List<CommonMasterModel>();
                serviceType = _service.GetDaysDropdwon();
                var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }

        public ActionResult GetAutoServiceAllocationDetailById(int AutoAllocationId, int ServiceTypeId)
        {
            JsonResult jlResult;
            {
                List<AutoServiceAllocationDtlModel> serviceType = new List<AutoServiceAllocationDtlModel>();
                serviceType = _service.GetAutoServiceAllocationDetailById(AutoAllocationId,ServiceTypeId);
                var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }

        [HttpGet]
        public ActionResult GetLinkedServicesByServiceType_ServiceId(int AutoAllocationId = 0, int ServiceTypeId = 0, int ServiceId = 0)
        {
            JsonResult jlResult;
            {
                List<AutoServiceAllocationDtlModel> serviceType = new List<AutoServiceAllocationDtlModel>();
                serviceType = _service.GetLinkedServicesByServiceType_ServiceId(AutoAllocationId, ServiceTypeId,ServiceId);
                var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        //[HttpGet]
        //public ActionResult GetLinkedServicesByServiceType_ServiceId()
        //{
        //    JsonResult jlResult;
        //    {
        //        List<AutoServiceAllocationDtlModel> serviceType = new List<AutoServiceAllocationDtlModel>();
        //        serviceType = _service.GetLinkedServicesByServiceType_ServiceId(0, 0, 0);
        //        var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
        //        jlResult = jResult;
        //    }
        //    return jlResult;
        //}
        public ActionResult GetLinkedServicesByServiceTypeServiceId(int ServiceId,int hospitalType,int patientType,int stateId,int cityId,int gender)
        {
            JsonResult jlResult;
            {
                List<AutoServiceAllocationDtlModel> serviceType = new List<AutoServiceAllocationDtlModel>();
                serviceType = _service.GetLinkedServicesByServiceTypeServiceId(ServiceId, hospitalType,patientType,stateId,cityId,gender);
                var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
    }
}
