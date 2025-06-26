using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGHSBilling.API.Masters.Interfaces;
using CGHSBilling.API.Masters.Repositories;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Caching;
using CommonLayer;
using CommonLayer.Extensions;

namespace CGHSBilling.Areas.Masters.Controllers
{
    public class CommonMasterController : Controller
    {
        //
        ICommonMasterRepository _data;
        private static readonly ILogger Loggger = Logger.Register(typeof(CommonMasterController));

        public CommonMasterController()
        {
            _data = new CommonMasterRepository();
        }

        public ActionResult Gender()
        {
            JsonResult jlResult;
            {
                var list = GetAllActiveGender();
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        
        public ActionResult GetAllStateMaster()
        {
            JsonResult jlResult;
            {
                var list = GetAllActiveState();
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        public ActionResult GetAllCityByState(int stateId)
        {
            JsonResult jlResult;
            {
                var list = _data.GetCitybyStateId(stateId);
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        public ActionResult GetActivPatientType()
        {
            JsonResult jlResult;
            {
                var list = GetAllActivPatientType();
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        public ActionResult GetctiveRoomType()
        {
            JsonResult jlResult;
            {
                var list = GetAllActiveRoomType();
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        public ActionResult GetActiveTypeOfAdmission()
        {
            JsonResult jlResult;
            {
                var list = GetAllActiveTypeOfAdmission();
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        public ActionResult GetActiveTypeofManagement()
        {
            JsonResult jlResult;
            {
                var list = GetAllActiveTypeofManagement();
                var jResult = Json(list, JsonRequestBehavior.AllowGet);
                jlResult = jResult;
            }
            return jlResult;
        }
        [OutputCache(Duration = 120)]
        public ActionResult CalculationDropdownData()
        {
            JsonResult jlResult;
            try
            {
                var list = new List<KeyValuePair<string, int>>();
                foreach (var e in Enum.GetValues(typeof(CalculationDropDown)))
                {
                    list.Add(new KeyValuePair<string, int>(e.ToString(), (int)e));
                }
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in CommonMasterController  CalculationDropdownData:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }
        [OutputCache(Duration = 120)]
        public ActionResult IncreasedtypeDropdown()
        {
            JsonResult jlResult;
            try
            {
                var list = new List<KeyValuePair<string, int>>();
                foreach (var e in Enum.GetValues(typeof(Increasedtype)))
                {
                    list.Add(new KeyValuePair<string, int>(e.ToString(), (int)e));
                }
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in CommonMasterController  IncreasedtypeDropdown:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }
        [OutputCache(Duration = 120)]
        public ActionResult HospitalTypeDropDown()
        {
            JsonResult jlResult;
            try
            {
                var list = new List<KeyValuePair<string, int>>();
                foreach (var e in Enum.GetValues(typeof(HospitalType)))
                {
                    list.Add(new KeyValuePair<string, int>(e.ToString(), (int)e));
                }
                jlResult = Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in CommonMasterController  HospitalTypeDropDown:" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }
        public List<CommonMasterModel> GetAllActiveGender()
        {
            List<CommonMasterModel> datalist = new List<CommonMasterModel>();
            try
            {
                if (!MemoryCaching.CacheKeyExist(CachingKeys.ActiveGender.ToString()))
                {
                    var list = _data.GetAllActiveGender();
                    datalist = list.ToList();
                    MemoryCaching.AddCacheValue(CachingKeys.ActiveGender.ToString(), datalist);
                }
                else
                {
                    datalist = (List<CommonMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.ActiveGender.ToString()));
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in CommonMasterController  Gender:" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return datalist;
        }
        public List<CommonMasterModel> GetAllActiveState()
        {
            List<CommonMasterModel> datalist = new List<CommonMasterModel>();
            try
            {
                if (!MemoryCaching.CacheKeyExist(CachingKeys.State.ToString()))
                {
                    var list = _data.GetAllState();
                    datalist = list.ToList();
                    MemoryCaching.AddCacheValue(CachingKeys.State.ToString(), datalist);
                }
                else
                {
                    datalist = (List<CommonMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.State.ToString()));
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllActiveState :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return datalist;
            }
            return datalist;
        }
        public List<CommonMasterModel> GetAllActivPatientType()
        {
            List<CommonMasterModel> datalist = new List<CommonMasterModel>();
            try
            {
                if (!MemoryCaching.CacheKeyExist(CachingKeys.PatientType.ToString()))
                {
                    var list = _data.GetAllPatientType();
                    datalist = list.ToList();
                    MemoryCaching.AddCacheValue(CachingKeys.PatientType.ToString(), datalist);
                }
                else
                {
                    datalist = (List<CommonMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.PatientType.ToString()));
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllActivPatientType :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return datalist;
            }
            return datalist;
        }

        public ActionResult GetRoomEntitlementList()
        {
            JsonResult jsonResult = null;
            List<CommonMasterModel> list = new List<CommonMasterModel>();
            TryCatch.Run(() =>
            {
                list = GetAllActiveRoomType().Where(W=> W.IsValidForEntitlement == true).ToList();
                jsonResult = Json(new { data = list }, JsonRequestBehavior.AllowGet);
            }).IfNotNull(ex =>
            {
                Loggger.LogError("Error in class:CommonMasterController method :GetRoomEntitlementList :" + Environment.NewLine + ex.StackTrace);
            });

            return jsonResult;
        }

        public List<CommonMasterModel> GetAllActiveRoomType()
        {
            List<CommonMasterModel> datalist = new List<CommonMasterModel>();
            try
            {
                if (!MemoryCaching.CacheKeyExist(CachingKeys.RoomType.ToString()))
                {
                    var list = _data.GetAllRoomType();
                    datalist = list.ToList();
                    MemoryCaching.AddCacheValue(CachingKeys.RoomType.ToString(), datalist);
                }
                else
                {
                    datalist = (List<CommonMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.RoomType.ToString()));
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllActiveRoomType :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return datalist;
            }
            return datalist;
        }
        public List<CommonMasterModel> GetAllActiveTypeOfAdmission()
        {
            List<CommonMasterModel> datalist = new List<CommonMasterModel>();
            try
            {
                if (!MemoryCaching.CacheKeyExist(CachingKeys.AdmissionType.ToString()))
                {
                    var list = _data.GetAllTypeOfAdmission();
                    datalist = list.ToList();
                    MemoryCaching.AddCacheValue(CachingKeys.AdmissionType.ToString(), datalist);
                }
                else
                {
                    datalist = (List<CommonMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.AdmissionType.ToString()));
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllActiveRoomType :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return datalist;
            }
            return datalist;
        }
        public List<CommonMasterModel> GetAllActiveTypeofManagement()
        {
            List<CommonMasterModel> datalist = new List<CommonMasterModel>();
            try
            {
                if (!MemoryCaching.CacheKeyExist(CachingKeys.ManagementType.ToString()))
                {
                    var list = _data.GetAllTypeofManagement();
                    datalist = list.ToList();
                    MemoryCaching.AddCacheValue(CachingKeys.ManagementType.ToString(), datalist);
                }
                else
                {
                    datalist = (List<CommonMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.ManagementType.ToString()));
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllActiveTypeofManagement :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return datalist;
            }
            return datalist;
        }


        public ActionResult GetActiveTypeofManagementNew()
        {
            JsonResult jlResult;
            List<CommonMasterModel> datalist = new List<CommonMasterModel>();
            try
            {
                int Userid = System.Web.HttpContext.Current.Session["AppUserId"] != null ?
                             Convert.ToInt32(System.Web.HttpContext.Current.Session["AppUserId"]) : 0;

                if (!MemoryCaching.CacheKeyExist(CachingKeys.ManagementLinking.ToString()))
                {
                    var list = _data.GetAllManagementLinking(Userid);
                    datalist = list.ToList();
                    MemoryCaching.AddCacheValue(CachingKeys.ManagementLinking.ToString(), datalist);
                }
                else
                {
                    datalist = (List<CommonMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.ManagementLinking.ToString()));
                }

                jlResult = Json(datalist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllActiveTypeofManagement :" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(datalist, JsonRequestBehavior.AllowGet);
            }            
            return jlResult;
        }



        }
}
