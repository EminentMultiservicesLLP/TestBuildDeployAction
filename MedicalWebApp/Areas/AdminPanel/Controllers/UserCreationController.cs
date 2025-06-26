using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CGHSBilling.API.AdminPanel.Interfaces;
using CGHSBilling.API.AdminPanel.Repositories;
using CGHSBilling.Areas.AdminPanel.Models;
using CommonLayer;
using CommonLayer.EncryptDecrypt;
using System.Collections.Generic;
using CGHSBilling.Common;

namespace CGHSBilling.Areas.AdminPanel.Controllers
{
    public class UserCreationController : Controller
    {
        IUserCreationInterface _action;
        ConnectionString _ConnectionString;
        private static readonly ILogger Loggger = Logger.Register(typeof(UserCreationController));

        public UserCreationController()
        {
            _action = new UserCreationRepository();
            _ConnectionString = new ConnectionString();
        }

        [HttpGet]
        public async Task<JsonResult> GetUserCode()
        {
            JsonResult jResult;

            try
            {
                string ConnectionString = _ConnectionString.getConnectionStringName();
                var list = _action.GetUserCode(ConnectionString);
                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetUserCode :" + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                return Json("Error");
            }

        }
        public async Task<JsonResult> GetUserDetails()
        {
            JsonResult jResult;
            try
            {
                List<UserCreationModel> list = new List<UserCreationModel>();
                string ConnectionString = _ConnectionString.getConnectionStringName();
                list = _action.GetUserDetails(ConnectionString);
                list.ForEach(x => x.Password = string.IsNullOrWhiteSpace(x.Password)? 
                                               "": EncryptDecryptDES.DecryptString(x.Password)); 
                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetUserDetails :" + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                return Json("Error");
            }

        }

        public UserCreationModel GetUserDetailsByUserId(int userId,string ConnectionString)
        {
            UserCreationModel list = new UserCreationModel();
            try
            {
                list = _action.GetUserDetailsByUserId(userId, ConnectionString);
                return list;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetUserDetails :" + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                return list;
            }

        }

        public async Task<JsonResult> SaveUser(UserCreationModel details)
        {
            JsonResult jResult; 
            UserCreationModel userData = new UserCreationModel();
            bool isSuccess = false;
            details.InsertedBy = Convert.ToInt32(Session["AppUserId"].ToString());
            details.InsertedON = DateTime.Now;
            details.InsertedIPAddress = Common.Constants.IpAddress;
            details.InsertedMacID = Common.Constants.MacId;
            details.InsertedMacName = Common.Constants.MacName;
            string ConnectionString = _ConnectionString.getConnectionStringName();
          
            // userData = GetUserDetailsByUserId(details.UserID, ConnectionString);
            // if (details.LoginName != null && details.Password != null)
            // {

            //}
            //else
            //{
            //   details.Password = userData.Password;
            //}

            try
            {
                if(details.Password != null)
                details.Password = EncryptDecryptDES.EncryptString(details.Password);

                if (details.UserID == 0)
                {
                    isSuccess = _action.CheckDuplicateItem(details.LoginName, details.EmailID, 0, "UserLogin");
                    if (isSuccess)
                    {
                        jResult = Json(new { Message = "Duplicate Entry Found!", success = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var list = _action.SaveUser(details, ConnectionString);
                        if (list == 1)
                        {
                            isSuccess = true;
                        }
                        if (!isSuccess)
                            jResult = Json(new { success = false, Message = "Record Save Failed" });
                        else
                            jResult = Json(new { success = true, Message = " Record Saved Successfully" });
                    }
                }
                else
                {
                    isSuccess = _action.CheckDuplicateUpdate(details.EmailID,details.LoginName,details.UserID, "UserLogin");
                    if (isSuccess)
                    {
                        jResult = Json(new { Message = "Duplicate Entry Found!", success = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var list = _action.SaveUser(details, ConnectionString);
                        jResult = Json(new { Message = "Record Updated Successfully", success = true }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in SaveUser :" + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                jResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
                //return Json("Error");
            }
            return jResult;
        }

        public async Task<JsonResult> SaveUserLogin(UserCreationModel details)
        {
            JsonResult jResult;
            UserCreationModel userData = new UserCreationModel();
            bool isSuccess = false;
            string ConnectionString = _ConnectionString.getConnectionStringName();
            userData = GetUserDetailsByUserId(details.UserID, ConnectionString);
            details.InsertedBy = Convert.ToInt32(Session["AppUserId"].ToString());
            details.InsertedON = DateTime.Now;
            details.InsertedIPAddress = Common.Constants.IpAddress;
            details.InsertedMacID = Common.Constants.MacId;
            details.InsertedMacName = Common.Constants.MacName;
            details.Password = EncryptDecryptDES.EncryptString(details.Password);
            string oldpassWord = EncryptDecryptDES.DecryptString(userData.Password);
            if (oldpassWord != details.OldPassword)
            {
                return Json(new { success = false, Message = "Old Password Verification Failed" });
            }
            try
            {
                var list = _action.SaveUser(details, ConnectionString);
                if (list == 1)
                {
                    isSuccess = true;

                }
                if (!isSuccess)
                    return Json(new { success = false, Message = "Record Save Failed" });
                else
                    return Json(new { success = true, Message = " Record Saved Successfully" });
            }
            catch (Exception ex)
            {

                Loggger.LogError("Error in GetUserDetails :" + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                return Json("Old Password Verification Failed");
            }


        }
        public async Task<JsonResult> GetClientTracker()
        {
            JsonResult jResult;
            try
            {
                List<UserCreationModel> list = new List<UserCreationModel>();
                string ConnectionString = _ConnectionString.getConnectionStringName();
                list = _action.GetClientTracker(ConnectionString);
                jResult = Json(list, JsonRequestBehavior.AllowGet);
                return jResult;
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetClientTracker :" + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                return Json("Error");
            }

        }
    }
}
;