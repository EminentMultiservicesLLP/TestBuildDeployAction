using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CGHSBilling.API.AdminPanel.Interfaces;
using CGHSBilling.API.AdminPanel.Repositories;
using CGHSBilling.Models;
using CommonLayer;
using Microsoft.Ajax.Utilities;
using CGHSBilling.Common;

namespace CGHSBilling.Areas.AdminPanel.Controllers
{
    public class UserAccessController : Controller
    {
        IUserAccessInterface _action;
        private static readonly ILogger Loggger = Logger.Register(typeof(UserAccessController));
        ConnectionString _ConnectionString;

        public UserAccessController()
        {
            _action = new UserAccessRepository();
            _ConnectionString = new ConnectionString();
        }

        public async Task<JsonResult> GetMenuDetails()
        {
            JsonResult jResult = null;
            Session["menusession"] = null;

            try
            {
                var records = Session["UserMenu"] as List<MenuUserRightsModel>;
                if (records != null)
                {
                    var allMenus =
                        records.Select(
                            s =>
                                new ParentMenuRights
                                {
                                    MenuName = s.MenuName,
                                    ParentMenuId = s.ParentMenuId,
                                    MenuId = s.MenuId,
                                    PageName = s.PageName,
                                    Access = s.Access,
                                    UserId = s.UserId
                                }).ToList();
                    Session["menusession"] = allMenus;

                    records = records.Where(m => m.ParentMenuId == 0).ToList();
                    jResult = Json(records, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetMenuDetails :" + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                jResult = Json("Error");
            }
            return jResult;
        }


        [HttpGet]
        public async Task<JsonResult> GetSubMenuDetails(int menuid)
        {
            JsonResult jResult;

            try
            {

                var records = Session["menusession"] as List<ParentMenuRights>;
                records = records.Where(m => m.ParentMenuId == menuid).ToList();
               // records.ForEach(m => m.SubParentMenuId = menuid);
                jResult = Json(records, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Loggger.LogError("Error in GetSubMenuDetails :" + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                jResult = Json("Error");
            }
            return jResult;
        }

        [HttpGet]
        public async Task<JsonResult> GetUserMenuByparentid(int menuid, bool state)
        {
            JsonResult jResult;

            var records = Session["menusession"] as List<ParentMenuRights>;
            var sessionRecords = records;
            records = records.Where(m => m.ParentMenuId == menuid).ToList();
            sessionRecords.Where(m => m.MenuId == menuid).ToList()[0].State = state;
            Session["menusession"] = sessionRecords;
            jResult = Json(records, JsonRequestBehavior.AllowGet);
            return jResult;
        }

        [HttpGet]
        public async Task<JsonResult> GetMenuByUser(int userId)//clearSession
        {
            JsonResult jResult;
            try
            {
               
                Session["menusession"] = null;
                Session["StdSubSession"] = null;
                string ConnectionString = _ConnectionString.getConnectionStringName();
                var records = _action.GetMenuByUser(userId, ConnectionString);
               // var subAccess = _action.GetSubAccessData(userId);
                Session["menusession"] = records.ToList();
                //Session["StdSubSession"] = subAccess.ToList();
                //var standardData = subAccess.DistinctBy(m => m.StandardId).ToList();
                var distinctRoleAccess = records.Where(m => m.ParentMenuId == 0).ToList();

                jResult = distinctRoleAccess != null ? Json(new { data = distinctRoleAccess}, JsonRequestBehavior.AllowGet) : Json("Error");
            }
              catch (Exception ex)
            {
                Loggger.LogError("Error in GetSubMenuDetails :" + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                jResult = Json("Error");
            }
            return jResult;
        }

        [HttpPost]
        public async Task<JsonResult> SaveUserAccess(MenuUserRightsModel accessData)
        {
            string _url = "";
            bool _isSuccess = false;
            var records = Session["menusession"] as List<ParentMenuRights>;
            //var varsubStandard = Session["StdSubSession"] as List<SubjectMasterModel>;
            //if (varsubStandard != null)
            //{
            //    accessData.SubStandardData = varsubStandard.Where(m => m.State).ToList();
            //}
            accessData.parent = records.Where(m => m.State == true).ToList();
            accessData.InsertedBy = Convert.ToInt32(Session["AppUserId"].ToString());
            accessData.InsertedON = System.DateTime.Now;
            accessData.InsertedIPAddress = Common.Constants.IpAddress;
            accessData.InsertedMacID = Common.Constants.MacId;
            accessData.InsertedMacName =Common.Constants.MacName;

            try
            {
                string ConnectionString = _ConnectionString.getConnectionStringName();
                var list = _action.SaveUserAccess(accessData, ConnectionString);
                if (list == 0)
                {
                    _isSuccess = true;
                    Session["menusession"] = null;
                    Session["StdSubSession"] = null;

                }
                if (!_isSuccess)
                    return Json(new { success = false, Message = "Record Save Failed" });
                else
                    return Json(new { success = true, Message = " Record Saved Successfully" });
            }
            catch (Exception ex)
            {

                Loggger.LogError("Error in GetUserDetails :" + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                return Json("Error");
            }

          
        }
        ///******************Standard Subject Access Area *******************/
        //public async Task<JsonResult> SaveAcademicAccess(SubjectMasterModel AccessData)
        //{
        //    string _url = "";
        //    bool _isSuccess = false;
        //    var records = Session["StdSubsession"] as List<SubjectMasterModel>;
        //    var abc=records.Where(m => m.State == true).ToList();
        //    abc.ForEach(m=> m.UserId=AccessData.UserId);
        //    try
        //    {
        //        var list = _action.SaveAcademicAccess(abc);
        //        if (list == 0)
        //        {
        //            _isSuccess = true;
        //            Session["StdSubsession"] = null;

        //        }
        //        if (!_isSuccess)
        //            return Json(new { success = false, Message = "Record Save Failed" });
        //        else
        //            return Json(new { success = true, Message = " Record Saved Successfully" });
        //    }
        //    catch (Exception ex)
        //    {

        //        Loggger.LogError("Error in SaveAcademicAccess :" + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
        //        return Json("Error");
        //    }

        //}
        //public ActionResult GetAllSubStandard()
        //{
        //    JsonResult jResult;
        //    try
        //    {
        //        var list = _action.GetAllStdSub();
        //        var jlist = list.Where(m => m.Deactive == false).ToList();
        //        var jlist1 = jlist.DistinctBy(m => m.StandardId).ToList();
        //        Session["StdSubSession"] = jlist.ToList();
        //        jResult = Json(jlist1, JsonRequestBehavior.AllowGet);
        //        return jResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        Loggger.LogError("Error in GetAllStdSub :" + ex.Message + Environment.NewLine + ex.StackTrace);
        //        return Json("Error");
        //    }

        //}
        //[HttpGet]
        //public async Task<JsonResult> GetAllSubjectById(int standardId)
        //{
        //    JsonResult jResult;
        //    var records = Session["StdSubSession"] as List<SubjectMasterModel>;
        //    records = records.Where(m => m.StandardId == standardId).ToList();
        //    jResult = Json(records, JsonRequestBehavior.AllowGet);
        //    return jResult;
        //}

        //[HttpGet]
        //public async Task<JsonResult> SetStandardAccess(int standardId,int subjectId,bool state)
        //{
        //    JsonResult jResult;
        //    var records = Session["StdSubSession"] as List<SubjectMasterModel>;
        //    var sessionRecords = records;
        //    sessionRecords.Where(m => m.StandardId == standardId).ToList()[0].State = state;
        //    records = records.DistinctBy(m => m.StandardId).ToList();
        //    Session["StdSubSession"] = sessionRecords;
        //    jResult = Json(records, JsonRequestBehavior.AllowGet);
        //    return jResult;
        //}

        //[HttpGet]
        //public async Task<JsonResult> SetSubjectAccess(int standardId, int subjectId, bool state)
        //{
        //    JsonResult jResult;
        //    var records = Session["StdSubSession"] as List<SubjectMasterModel>;
        //    var sessionRecords = records;
        //    sessionRecords.Where(m => m.SubjectId == subjectId).ToList()[0].State = state;
        //    records = records.Where(m => m.StandardId == standardId).ToList();
        //    Session["StdSubSession"] = sessionRecords;
        //    jResult = Json(records, JsonRequestBehavior.AllowGet);
        //    return jResult;
        //}
      

    }
}
