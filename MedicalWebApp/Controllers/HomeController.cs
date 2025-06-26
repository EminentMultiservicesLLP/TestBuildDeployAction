using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using CGHSBilling.Models;
using CommonLayer;
using CommonLayer.Extensions;
using Microsoft.Ajax.Utilities;
using CGHSBilling.Areas.HospitalForms.Controllers;
using CGHSBilling.Common;
using CGHSBilling.Areas.AdminPanel.Controllers;
using CommonDataLayer.DataAccess;

namespace CGHSBilling.Controllers
{
    public class HomeController : Controller
    {
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
        public sealed class ValidateHeaderAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationContext filterContext)
            {
                if (filterContext == null)
                {
                    throw new ArgumentNullException("filterContext");
                }

                var httpContext = filterContext.HttpContext;
                var cookie = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
                AntiForgery.Validate(cookie != null ? cookie.Value : null, httpContext.Request.Headers["__RequestVerificationToken"]);
            }
        }


        private static readonly ILogger Logger = CommonLayer.Logger.Register(typeof(HomeController));
        public ActionResult Index()
        {
            Session["EmpList"] = null;
            if (Session["AppUserId"].IsNotNull() && !Session["AppUserId"].ToString().IsNullOrWhiteSpace())
            {

                List<MenuUserRightsModel> records;
                int userId = Convert.ToInt32(Session["AppUserId"]);
                records = GetAllMenuRights(userId, 0);

                if (records.IsNotNull() && records.Count > 0)
                {
                    Session["UserMenu"] = records;

                    var clientId = records.FirstOrDefault()?.ClientId;
                    var strExpiryDate = records.FirstOrDefault()?.strExpiryDate;

                    Session["ClientId"] = clientId;
                    Session["ClientExpiryDate"] = strExpiryDate;

                    int clientid = clientId.HasValue ? clientId.Value : 0;

                    GetBalanceAmtByUserId(clientid);
                }
                else
                {
                    Logger.LogInfo("User :" + userId + ", do not have access to any menu");
                }
                return View(records);
            }
            else
            {
                Logger.LogInfo("Missing userID, hence redirecting to Login Page");
                return RedirectToAction("Login", "Account");
            }
        }

        readonly DataSet _ds = new DataSet();
        readonly string _conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        public List<MenuUserRightsModel> GetAllMenuRights(int userId, int parentMenuId)
        {

            List<MenuUserRightsModel> menu = null;

            TryCatch.Run(() =>
            {

                using (DBHelper Dbhelper = new DBHelper())
                {

                    DBParameterCollection paramCollection = new DBParameterCollection();
                    paramCollection.Add(new DBParameter("UserId", userId, DbType.Int32));
                    DataSet dsUserMenuRights = Dbhelper.ExecuteDataSet("sp_GetUserMenuRights", paramCollection, CommandType.StoredProcedure);
                    DataTable dtmenuRights = dsUserMenuRights.Tables[0];
                    // DataTable dtmenuRights = Dbhelper.ExecuteDataTable("sp_GetUserMenuRights", paramCollection, CommandType.StoredProcedure);

                    menu = dtmenuRights.AsEnumerable().Select(row => new MenuUserRightsModel
                    {
                        MenuId = row.Field<int>("MenuId"),
                        UserId = row.Field<int>("UserId"),
                        Access = row.Field<bool>("Access"),
                        Add = row.Field<bool>("Add"),
                        Edit = row.Field<bool>("Edit"),
                        DeletePerm = row.Field<bool>("DeletePerm"),
                        SuperPerm = row.Field<bool>("SuperPerm"),
                        MenuName = row.Field<string>("MenuName"),
                        PageName = row.Field<string>("PageName"),
                        ParentMenuId = row.Field<int>("ParentMenuId"),
                        PageShortDescription = row.Field<string>("PageShortDescription"),
                        Icon = row.Field<string>("Icon"),
                        ClientId = row.Field<int>("ClientId"),
                        strExpiryDate = row.Field<DateTime?>("ExpiryDate")?.ToString("dd-MMM-yyyy") ?? string.Empty,
                        MarqueeMessage = row.Field<string>("MarqueeMessage")
                    }).ToList();

                }
            }).IfNotNull((ex) =>
            {
                Logger.LogError("Error in HomeController  GetAllMenuRights:" + ex.Message + Environment.NewLine + ex.StackTrace);
            });
            return menu;
        }

        [ValidateAntiForgeryToken]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public PartialViewResult PackageSummaryView()
        {
            return PartialView("_PackageSummaryView");
        }

        public ActionResult SupportView()
        {
            var viewData = RequestSubmissionNewController.RenderPartialViewToString(this, "~/Views/Home/_SupportView.cshtml", null);
            return Json(new { view = viewData }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSupporytPDF()
        {
            var supportFilePath = Convert.ToString(ConfigurationManager.AppSettings["SupportFilePath"]);
            var DownLoadFileAsName = Convert.ToString(ConfigurationManager.AppSettings["DownloadFileAsName"]);
            var fileDownloadName = Server.MapPath(supportFilePath);
            var contentType = "application/pdf";

            return File(fileDownloadName, contentType, DownLoadFileAsName);
        }

        public void GetBalanceAmtByUserId(int clientid)
        {
            new BillingConfigurationController().GetBalanceAmount(clientid, true);

        }

        public ActionResult AccessDenied()
        {
            return View();
        }

    }
}
