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

namespace CGHSBilling.Areas.Masters.Controllers
{
    public class HintNotificationController : Controller
    {
        IHintNotificationRepository _service;
        private static readonly ILogger Loggger = Logger.Register(typeof(HintNotificationController));
        public HintNotificationController()
        {
            _service = new HintNotificationRepository();
        }
        [HttpPost]
        public ActionResult SaveNotification(HintNotificationModel jsonData)
        {
            JsonResult jlResult; bool success = false;
            try
            {
                var save = _service.CreateNotification(jsonData);
                jlResult = Json(new { Message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
                MemoryCaching.RemoveCacheValue(CachingKeys.Notification.ToString());


            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in HintNotificationController  SaveNotification:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }

        public ActionResult GetAllNotification(int Managment)
        {
            JsonResult jlResult;
            {
                var list = GetAllNotificationInCached();
                var one = list.Where(m => m.StepNo == 12).ToList();
                list =list.Where(m => m.SubControlId == Managment).ToList();
                if (Managment > 0)
                {
                    list.AddRange(one);
                }
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }

        public ActionResult GetAllNotificationData()
        {
            JsonResult jlResult;
            {
                var list = GetAllNotificationInCached();
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        public List<HintNotificationModel> GetAllNotificationInCached()
        {
            List<HintNotificationModel> serviceType = new List<HintNotificationModel>();
            try
            {
                if (!MemoryCaching.CacheKeyExist(CachingKeys.Notification.ToString()))
                {
                    serviceType = _service.GetAllNotification();
                    MemoryCaching.AddCacheValue(CachingKeys.Notification.ToString(), serviceType);
                }
                else
                {
                    serviceType = (List<HintNotificationModel>)(MemoryCaching.GetCacheValue(CachingKeys.Notification.ToString()));
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllNotificationInCached :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return serviceType;
            }
            return serviceType;
        }

    }
}
