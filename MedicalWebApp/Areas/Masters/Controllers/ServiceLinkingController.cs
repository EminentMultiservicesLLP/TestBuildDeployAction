using CGHSBilling.API.Masters.Interfaces;
using CGHSBilling.API.Masters.Repositories;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Caching;
using CommonLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CGHSBilling.Areas.Masters.Controllers
{
    public class ServiceLinkingController : Controller
    {
        //
        // GET: /Masters/ServiceLinking/
        IServiceLinkingInterface _serviceCategory;
        private static readonly ILogger Loggger = Logger.Register(typeof(ServiceLinkingController));

        public ServiceLinkingController()
        {
            _serviceCategory = new ServiceLinkingRepository();
        }



        public ActionResult GetAllManagementType()
        {
            JsonResult jlResult;
            {
                var list = GetAllManagementTypeInCache();
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        
        public List<HospitalServicelinkingModel> GetAllManagementTypeInCache()
        {
            List<HospitalServicelinkingModel> ManagementType = new List<HospitalServicelinkingModel>();
            try
            {

                if (!MemoryCaching.CacheKeyExist(CachingKeys.ServiceCategory.ToString()))
                {
                    var list = _serviceCategory.GetAllManagementType();
                    ManagementType = list.ToList();
                    MemoryCaching.AddCacheValue(CachingKeys.ServiceCategory.ToString(), ManagementType);

                }
                else
                {
                    ManagementType = (List<HospitalServicelinkingModel>)(MemoryCaching.GetCacheValue(CachingKeys.ServiceCategory.ToString()));
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllServiceTypeMasterInCache :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return ManagementType;
            }
            return ManagementType;
        }

        [HttpPost]
        public ActionResult SaveServiceTypeManagementtypeLinking(HospitalServicelinkingModel entity)
        {
            JsonResult jlResult; bool success = false;
            try
            {
                        var save = _serviceCategory.SaveServiceTypeManagementtypeLinking(entity);
                        jlResult = Json(new { Message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
                        MemoryCaching.RemoveCacheValue(CachingKeys.ServiceCategory.ToString());
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in ServiceTypeMasterController  SaveserviceType:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }

        public ActionResult GetLinkedHospitalServicesById(int HospitalServiceCategoryId)
        {
            JsonResult jlResult;
            {
                List<HospitalServicelinkingModel> serviceType = new List<HospitalServicelinkingModel>();
                serviceType = _serviceCategory.GetAllLinkedRecordById(HospitalServiceCategoryId);
                var jResult = Json(serviceType, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }



    }
}
