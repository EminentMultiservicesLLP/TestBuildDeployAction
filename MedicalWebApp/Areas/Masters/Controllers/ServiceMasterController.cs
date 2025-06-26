using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGHSBilling.API.Masters.Interfaces;
using CGHSBilling.API.Masters.Repositories;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Caching;
using CGHSBilling.Models;
using CommonLayer;
using static CGHSBilling.Controllers.HomeController;

namespace CGHSBilling.Areas.Masters.Controllers
{
    [ValidateHeaderAntiForgeryToken]
    public class ServiceMasterController : Controller
    {
        IServiceMasterRepository _service;
        private static readonly ILogger Loggger = Logger.Register(typeof(ServiceMasterController));

        public ServiceMasterController()
        {
            _service = new ServiceMasterRepository();
        }
        [HttpPost]
        public ActionResult SaveServicemaster(ServiceMasterModel jsonData)
        {
            JsonResult jlResult; bool success = false;
            try
            {
                CheckDuplicateModel chkmodal = new CheckDuplicateModel();
                chkmodal.Id = jsonData.ServiceId;
                chkmodal.Code = jsonData.Code;
                chkmodal.Name = jsonData.ServiceName;
                jsonData.InsertedBy = Convert.ToInt32(Session["AppUserId"]);
                jsonData.InsertedOn = System.DateTime.Now;
                if (jsonData.ServiceId == 0)
                {
                    success = _service.CheckDuplicateInsert(chkmodal);
                    if (success)
                    {
                        jlResult = Json(new { Message = "Duplicate Entry Found! Same Service Name defined already", success = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var save = _service.SaveService(jsonData);
                        jlResult = Json(new { Message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
                        MemoryCaching.RemoveCacheValue(CachingKeys.ServiceMaster.ToString());
                    }

                }
                else
                {
                    success = _service.CheckDuplicateUpdate(chkmodal);
                    if (success)
                    {
                        jlResult = Json(new { Message = "Duplicate Entry Found! Same Service Name defined already", success = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var save = _service.UpdateService(jsonData);
                        jlResult = Json(new { Message = "Record Updated Successfully", success = true }, JsonRequestBehavior.AllowGet);
                        MemoryCaching.RemoveCacheValue(CachingKeys.ServiceMaster.ToString());
                    }

                }


            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in ServiceMasterController  SaveserviceType:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }
        public ActionResult GetAllLinkedGenderByServiceId(int ServiceId)
        {
            JsonResult jlResult;
            {
                List<ServiceGenderLinking> serviceType = new List<ServiceGenderLinking>();
                serviceType = _service.GetAllLinkedGenderByServiceId(ServiceId);
                var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        public ActionResult GetServicesByServiceTypeId(int ServiceTypeId)
        {
            JsonResult jlResult;
            {
                List<ServiceMasterModel> serviceType = new List<ServiceMasterModel>();
                serviceType = _service.GetServicesByServiceTypeId(ServiceTypeId);
                var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        public ActionResult GetAllServiceMaster()
        {
            JsonResult jlResult;
            {
                var list = GetAllServiceMasterInCache();
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }


        public ActionResult GetAllActiveServiceMaster()
        {
            JsonResult jlResult;
            {
                List<ServiceMasterModel> serviceType = new List<ServiceMasterModel>();
                serviceType = GetAllServiceMasterInCache();
                serviceType = serviceType.Where(m => m.Deactive == false).ToList();
                var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }

        public List<ServiceMasterModel> GetAllServiceMasterInCache()
        {
            List<ServiceMasterModel> serviceType = new List<ServiceMasterModel>();
            try
            {

                if (!MemoryCaching.CacheKeyExist(CachingKeys.ServiceMaster.ToString()))
                {
                    var list = _service.GetAllServiceMaster();
                    serviceType = list.ToList();
                    MemoryCaching.AddCacheValue(CachingKeys.ServiceMaster.ToString(), serviceType);

                }
                else
                {
                    serviceType = (List<ServiceMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.ServiceMaster.ToString()));
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllServiceMasterInCache :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return serviceType;
            }
            return serviceType;
        }

        public ActionResult GetAllActiveDefaultServiceMaster()
        {
            JsonResult jlResult;
            {
                List<ServiceMasterModel> serviceType = new List<ServiceMasterModel>();
                serviceType = _service.GetAllActiveDefaultServiceMaster();
                var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }

    }
}
