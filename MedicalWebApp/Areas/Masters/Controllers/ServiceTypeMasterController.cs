using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.API.Masters.Interfaces;
using CGHSBilling.API.Masters.Repositories;
using CGHSBilling.Models;
using CommonLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGHSBilling.Areas.AdminPanel.Models;
using CGHSBilling.Caching;

namespace CGHSBilling.Areas.Masters.Controllers
{
    public class ServiceTypeMasterController : Controller
    {
        IServiceTypeMasterRepository _serviceType;
        private static readonly ILogger Loggger = Logger.Register(typeof(ServiceTypeMasterController));

        public ServiceTypeMasterController()
        {
            _serviceType = new ServiceTypeMasterRepository();
        }


        [HttpPost]
        public ActionResult SaveserviceType(ServiceTypeMasterModel jsonData)
        {
            JsonResult jlResult; bool success = false;
            try
            {
                CheckDuplicateModel chkmodal = new CheckDuplicateModel();
                chkmodal.Id = jsonData.ServiceTypeId;
                chkmodal.Code = jsonData.Code;
                chkmodal.Name = jsonData.ServiceType;
                jsonData.InsertedBy = Convert.ToInt32(Session["AppUserId"]);
                jsonData.InsertedOn = System.DateTime.Now;
                if (jsonData.ServiceTypeId == 0)
                {
                    success = _serviceType.CheckDuplicateInsert(chkmodal);
                    if (success)
                    {
                        jlResult = Json(new { Message = "Duplicate Entry Found! Same Service Type defined already", success = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var save = _serviceType.SaveServiceType(jsonData);
                        jlResult = Json(new { Message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
                        MemoryCaching.RemoveCacheValue(CachingKeys.ServiceTypeMaster.ToString());
                    }
                        
                }
                else
                {
                    success = _serviceType.CheckDuplicateUpdate(chkmodal);
                    if (success)
                    {
                        jlResult = Json(new { Message = "Duplicate Entry Found! Same Service Type defined already", success = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var save = _serviceType.UpdateServiceType(jsonData);
                        jlResult = Json(new { Message = "Record Updated Successfully", success = true }, JsonRequestBehavior.AllowGet);
                        MemoryCaching.RemoveCacheValue(CachingKeys.ServiceTypeMaster.ToString());
                    }
                   
                }
               

            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in ServiceTypeMasterController  SaveserviceType:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }
        public ActionResult GetAllServiceTypeMaster()
        {
            JsonResult jlResult;
            {
                var list = GetAllServiceTypeMasterInCache();
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }

        public ActionResult GetAllActiveServiceTypeMaster()
        {
            JsonResult jlResult;
            {
                List<ServiceTypeMasterModel> serviceType = new List<ServiceTypeMasterModel>();
                serviceType = GetAllServiceTypeMasterInCache();
                serviceType = serviceType.Where(m => m.Deactive == false).ToList();
                var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        public List<ServiceTypeMasterModel> GetAllServiceTypeMasterInCache()
        {
            List<ServiceTypeMasterModel> serviceType =new List<ServiceTypeMasterModel>();
            try
            {

                if (!MemoryCaching.CacheKeyExist(CachingKeys.ServiceTypeMaster.ToString()))
                {
                    var list = _serviceType.GetAllServiceTypeMaster();
                    serviceType = list.ToList();
                    MemoryCaching.AddCacheValue(CachingKeys.ServiceTypeMaster.ToString(), serviceType);

                }
                else
                {
                    serviceType = (List<ServiceTypeMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.ServiceTypeMaster.ToString()));
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllServiceTypeMasterInCache :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return serviceType;
            }
            return serviceType;
        }

        public ActionResult GetAllCategory()
        {
            JsonResult jlResult;
            {
                List<CommonMasterModel> serviceType = new List<CommonMasterModel>();
                serviceType = _serviceType.GetAllCategory();
                var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }

        public ActionResult GetAllLinkedCategoryByTypeId(int ServiceTypeId)
        {
            JsonResult jlResult;
            {
                List<ServiceCategoryLinkingModel> serviceType = new List<ServiceCategoryLinkingModel>();
                serviceType = _serviceType.GetAllLinkedCategoryByTypeId(ServiceTypeId);
                var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }

    }
}
