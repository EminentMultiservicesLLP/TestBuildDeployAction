using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CGHSBilling.Models;

namespace CGHSBilling.Areas.AdminPanel.Controllers
{
    public class AdminPanelController : Controller
    {
        public ActionResult AdminPanel(int menuId)
        {
            if (Session["AppUserId"] == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            MenuUserRightsModel model = new MenuUserRightsModel();
            List<MenuUserRightsModel> menurights = (List<MenuUserRightsModel>)Session["UserMenu"];

            model.parent = menurights.Select(row => new ParentMenuRights
            {
                MenuId = row.MenuId,
                UserId = row.UserId,
                Access = row.Access,
                MenuName = row.MenuName,
                PageName = row.PageName,
                ParentMenuId = row.ParentMenuId
            }).Where(m => m.ParentMenuId.Equals(4)).ToList();
            if (menuId > 4)
            {
                model.child = menurights.Select(row => new ChildMenuRights
                {
                    MenuId = row.MenuId,
                    UserId = row.UserId,
                    Access = row.Access,
                    MenuName = row.MenuName,
                    PageName = row.PageName,
                    ParentMenuId = row.ParentMenuId
                }).Where(m => m.ParentMenuId.Equals(menuId)).ToList();
            }
            return View(model);
        }
        public PartialViewResult UserCreation()
        {
            return PartialView();
        }
        public PartialViewResult UserAccess()
        {
            return PartialView();
        }
        public PartialViewResult ClientMaster()
        {
            return PartialView();
        }
        public PartialViewResult Enquiry()
        {
            return PartialView();
        }
        public PartialViewResult Feedback()
        {
            return PartialView();
        }
        public PartialViewResult ClientTracker()
        {
            return PartialView();
        }
    }
}
