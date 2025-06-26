using CGHSBilling.Filters;
using System.Linq;
using System.Web;
using System;
using System.Web.Mvc;
using CGHSBilling.Models;
using System.Collections.Generic;

namespace CGHSBilling
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new MenuAccessSessionFilter());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SessionExpireAttribute());
        }
    }


    public class MenuAccessSessionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;

            var session = HttpContext.Current?.Session;
            var userId = session?["AppUserID"];
            var accessList = session?["UserMenu"] as List<MenuUserRightsModel>;

            // ✅ Allow everything for super admin (userId == 1)
            if (userId != null && Convert.ToInt32(userId) == 1)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            // Always allow these controller/action combinations
            var alwaysAllow = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Account",              // allow all Account controller actions
                "Home:AccessDenied",    // explicitly allow this action
                ":PackageSummaryView"   // allow any controller with this action
            };

            // Allow actions in alwaysAllow list
            if (alwaysAllow.Contains(controller) ||
                alwaysAllow.Contains($"{controller}:{action}") ||
                alwaysAllow.Contains($":{action}"))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            // Allow Home/Index only if user is logged in (for first-time menu setup)
            if (controller.Equals("Home", StringComparison.OrdinalIgnoreCase) &&
                action.Equals("Index", StringComparison.OrdinalIgnoreCase) &&
                userId != null)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            // ❗ Redirect to login if session is null
            if (userId == null)
            {
                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }

            // ❗ If accessList is null, block all
            if (accessList == null)
            {
                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }

            // ✅ Now enforce reverse check: only allow if controller+action exists in UserMenu
            bool hasPageAccess = accessList.Any(x =>
                x.PageName.Equals(controller, StringComparison.OrdinalIgnoreCase) &&
                x.Access == true);

            bool hasActionAccess = accessList.Any(x =>
                x.PageName.Equals($"{controller}/{action}", StringComparison.OrdinalIgnoreCase) ||
                x.PageName.Equals($"{controller}:{action}", StringComparison.OrdinalIgnoreCase));

            // If NOT found in allowed menu, deny access
            if (!hasPageAccess && !hasActionAccess)
            {
                filterContext.Result = new RedirectResult("/Home/AccessDenied");
                return;
            }

            // ✅ Allow execution
            base.OnActionExecuting(filterContext);

            //string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            //string action = filterContext.ActionDescriptor.ActionName;

            //var session = HttpContext.Current.Session;
            //var userId = session["AppUserID"];
            //var accessList = session["UserMenu"] as List<MenuUserRightsModel>;

            //// ✅ Allow everything for super admin (userId == 1)
            //if (userId != null && Convert.ToInt32(userId) == 1)
            //{
            //    base.OnActionExecuting(filterContext);
            //    return;
            //}

            //// Always allow these controller/action combinations
            //var alwaysAllow = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            //{
            //    "Account",              // allow all Account controller actions
            //    "Home:AccessDenied",    // explicitly allow this action
            //    ":PackageSummaryView"   // allow any controller with this action
            //};

            //// Allow actions in alwaysAllow list
            //if (alwaysAllow.Contains(controller) ||
            //    alwaysAllow.Contains($"{controller}:{action}") ||
            //    alwaysAllow.Contains($":{action}"))
            //{
            //    base.OnActionExecuting(filterContext);
            //    return;
            //}

            //// Allow Home/Index only if user is logged in (for first-time menu setup)
            //if (controller.Equals("Home", StringComparison.OrdinalIgnoreCase) &&
            //    action.Equals("Index", StringComparison.OrdinalIgnoreCase) &&
            //    userId != null)
            //{
            //    base.OnActionExecuting(filterContext);
            //    return;
            //}

            //// If user is not logged in, redirect to login
            //if (userId == null)
            //{
            //    filterContext.Result = new RedirectResult("/Account/Login");
            //    return;
            //}

            //// If menu access not yet loaded, still allow Home/Index, block others
            //if (accessList == null)
            //{
            //    filterContext.Result = new RedirectResult("/Home/Index");
            //    return;
            //}

            //// Now check if user has access to this controller
            //bool hasAccess = accessList.Any(x =>
            //    x.PageName.Equals(controller, StringComparison.OrdinalIgnoreCase) && x.Access == true);

            //if (!hasAccess)
            //{
            //    filterContext.Result = new RedirectResult("/Home/AccessDenied");
            //    return;
            //}

            //base.OnActionExecuting(filterContext);
        }
    }


}