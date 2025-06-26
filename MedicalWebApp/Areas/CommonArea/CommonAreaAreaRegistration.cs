using System.Web.Mvc;

namespace CGHSBilling.Areas.CommonArea
{
    public class CommonAreaAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CommonArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CommonArea_default",
                "CommonArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
