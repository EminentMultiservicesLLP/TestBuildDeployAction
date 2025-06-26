using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CGHSBilling.API.AdminPanel.Interfaces;
using CGHSBilling.API.AdminPanel.Repositories;
using CGHSBilling.Areas.AdminPanel.Models;
using CGHSBilling.Caching;
using CommonLayer;
using CGHSBilling.Common;
using Newtonsoft.Json;

namespace CGHSBilling.Areas.AdminPanel.Controllers
{
    public class ClientMasterController : Controller
    {
        IClientMasterInterface _action;
        private static readonly ILogger Loggger = Logger.Register(typeof(ClientMasterController));
        ConnectionString _ConnectionString;
        public ClientMasterController()
        {
            _action = new ClientMasterRepository();
            _ConnectionString = new ConnectionString();
        }
        public ActionResult AllClient()
        {
            JsonResult jResult;
            try
            {
                //var list = GetCacchedClient();
                var list = _action.GetAllClient();
                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllClient :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }

        }
        public ActionResult AllActiveClient()
        {
            JsonResult jResult;
            try
            {
                var list = GetCacchedClient();
                list = list.Where(m => m.Deactive = true).ToList();
                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllClient :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }

        }
        public ActionResult AllStates()
        {
            JsonResult jResult;
            try
            {
                var list = _action.GetAllStates();
                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllClient :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }
        }
        public ActionResult GetHospitalServiceCategory()
        {
            JsonResult jResult;
            try
            {
                var list = _action.GetHospitalServiceCategory();
                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllClient :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }
        }
        public ActionResult GetClientType()
        {
            JsonResult jResult;
            try
            {
                var list = _action.GetClientType();
                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllClient :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }
        }
        public ActionResult GetCityById(int stateId)
        {
            JsonResult jResult;
            try
            {
                var list = _action.GetCityById(stateId);
                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetCityById :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }

        }
        public async Task<JsonResult> SaveClient(ClientMasterModel model)
        {

            bool isSuccess = false, isDuplicate = false;
            model.InsertedBy = Convert.ToInt32(Session["AppUserId"].ToString());
            model.InsertedON = DateTime.Now;
            model.InsertedIPAddress = Common.Constants.IpAddress;
            model.InsertedMacID = Common.Constants.MacId;
            model.InsertedMacName = Common.Constants.MacName;
            try
            {
                string ConnectionString = _ConnectionString.getConnectionStringName();
                if (model.ClientId == 0)
                {
                    isDuplicate = _action.CheckDuplicateItem(model.Code, 0, "Client", ConnectionString);
                    if (isDuplicate == false)
                    {
                        var newId = _action.CreateClient(model, ConnectionString);
                        model.ClientId = newId;
                        isSuccess = true;
                        model.Message = "Record Saved Successfully";
                        MemoryCaching.RemoveCacheValue(CachingKeys.GetAllClient.ToString());
                    }
                    else
                    {
                        isSuccess = false;
                        model.Message = "Client Code Already Exists";
                    }
                }
                else
                {
                    isDuplicate = _action.CheckDuplicateUpdate(model.ClientId, model.Code, 0, "Client", ConnectionString);
                    if (isDuplicate == false)
                    {
                        isSuccess = _action.UpdateClient(model, ConnectionString);
                        isSuccess = true;
                        model.Message = "Record updated Successfully";
                        MemoryCaching.RemoveCacheValue(CachingKeys.GetAllClient.ToString());
                    }
                    else
                    {
                        isSuccess = false;
                        model.Message = "Client Code Already Exists";
                    }
                }
            }
            catch (Exception ex)
            {

                Loggger.LogError("Error in Save Client :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }

            if (!isSuccess)
                return Json(new { success = false, message = model.Message });
            else
                return Json(new { success = true, message = model.Message, clientId = model.ClientId });
        }
        public List<ClientMasterModel> GetCacchedClient()
        {
            List<ClientMasterModel> client;
            try
            {
                if (!MemoryCaching.CacheKeyExist(CachingKeys.GetAllClient.ToString()))
                {
                    var list = _action.GetAllClient();
                    client = list.ToList();
                    MemoryCaching.AddCacheValue(CachingKeys.GetAllClient.ToString(), client);
                }
                else
                {
                    client = (List<ClientMasterModel>)(MemoryCaching.GetCacheValue(CachingKeys.GetAllClient.ToString()));
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetAllClient :" + ex.Message + Environment.NewLine + ex.StackTrace);
                throw ex;
            }
            return client;
        }
        public async Task<JsonResult> SaveClientConfiguration(ClientConfiguration model)
        {
            bool isSuccess = false;
            model.InsertedBy = Convert.ToInt32(Session["AppUserId"].ToString());
            model.InsertedON = DateTime.Now;
            try
            {
                string ConnectionString = _ConnectionString.getConnectionStringName();
                var newId = _action.SaveClientConfiguration(model, ConnectionString);
                model.ConfigId = newId;
                isSuccess = true;
                model.Message = "Record Saved Successfully";
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in Save Client Configuration :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }

            if (!isSuccess)
                return Json(new { success = false, message = model.Message });
            else
                return Json(new { success = true, message = model.Message, clientId = model.ConfigId });
        }
        public ActionResult GetClientConfiguration(int? LoginId)
        {
            JsonResult jResult;
            try
            {
                if (LoginId != null)
                {
                    LoginId = Convert.ToInt32(Session["AppUserId"].ToString());
                }

                var list = _action.GetClientConfiguration(LoginId);
                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetClientConfiguration :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }

        }
        public ActionResult GetClientConfigDetails(int? ConfigId)
        {
            JsonResult jResult;
            try
            {
                var list = _action.GetClientConfigDetails(ConfigId);
                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetClientConfigDetails :" + ex.Message + Environment.NewLine + ex.StackTrace);
                return Json("Error");
            }

        }
    }
}
