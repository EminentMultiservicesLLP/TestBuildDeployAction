using CGHSBilling.API.Masters.Interfaces;
using CGHSBilling.API.Masters.Repositories;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Caching;
using CGHSBilling.Models;
using CommonLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CGHSBilling.Areas.Masters.Controllers
{
    public class PatientTypeMasterController : Controller
    {
        IPatientTypeMasterRepository _serviceType;
        private static readonly ILogger Loggger = Logger.Register(typeof(PatientTypeMasterController));

        public PatientTypeMasterController()
        {
            _serviceType = new PatientTypeMasterRepository();
        }

        [HttpPost]
        public ActionResult SavePatientTypeMaster(PatientTypeMasterModel jsonData)
        {
            JsonResult jlResult; bool success = false;
            try
            {
                CheckDuplicateModel chkmodal = new CheckDuplicateModel();
                chkmodal.Id = jsonData.PatientTypeId;
                chkmodal.Code = jsonData.Code;
                chkmodal.Name = jsonData.PatientType;
                jsonData.InsertedBy = Convert.ToInt32(Session["AppUserId"]);
                jsonData.InsertedOn = System.DateTime.Now;
                if (jsonData.PatientTypeId == 0)
                {
                    success = _serviceType.CheckDuplicateInsert(chkmodal);
                    if (success)
                    {
                        jlResult = Json(new { Message = "Duplicate Entry Found! Same Patient Type defined already", success = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var save = _serviceType.SavePatientTypeMaster(jsonData);
                        jlResult = Json(new { Message = "Record Saved Successfully", success = true }, JsonRequestBehavior.AllowGet);
                        MemoryCaching.RemoveCacheValue(CachingKeys.PatientType.ToString());
                    }

                }
                else
                {
                    success = _serviceType.CheckDuplicateUpdate(chkmodal);
                    if (success)
                    {
                        jlResult = Json(new { Message = "Duplicate Entry Found! Same Patient Type defined already", success = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var save = _serviceType.SavePatientTypeMaster(jsonData);
                        jlResult = Json(new { Message = "Record Updated Successfully", success = true }, JsonRequestBehavior.AllowGet);
                        MemoryCaching.RemoveCacheValue(CachingKeys.PatientType.ToString());
                    }

                }


            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in PatientTypeMasterController  SavePatientTypeMaster:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }

        public ActionResult GetAllPatientTypeMaster()
        {
            JsonResult jlResult;
            {
                var list = GetAllPatientTypeMasterInCache();
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        public ActionResult GetAllActivePatientTypeMaster()
        {
            JsonResult jlResult;
            {
                var list = GetAllPatientTypeMasterInCache();
                list = list.Where(m => m.Deactive == false).ToList();
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }

        public List<PatientTypeMasterModel> GetAllPatientTypeMasterInCache()
        {
            List<PatientTypeMasterModel> serviceType = new List<PatientTypeMasterModel>();
            try
            {

                if (!MemoryCaching.CacheKeyExist(CachingKeys.PatientType.ToString()))
                {
                    var list = _serviceType.GetAllPatientTypeMaster();
                    serviceType = list.ToList();
                    MemoryCaching.AddCacheValue(CachingKeys.PatientType.ToString(), serviceType);

                }
                else
                {
                    serviceType = (List<PatientTypeMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.PatientType.ToString()));
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllPatientTypeMasterInCache :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return serviceType;
            }
            return serviceType;
        }

    }
}
