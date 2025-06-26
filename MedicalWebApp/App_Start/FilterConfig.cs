using CGHSBilling.Filters;
using System.Web.Mvc;

namespace CGHSBilling
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SessionExpireAttribute());
        }
    }
}