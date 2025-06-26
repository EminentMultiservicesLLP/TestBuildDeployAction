using System.Web.Mvc;

namespace CGHSBilling.Areas.HospitalForms
{
    public class HospitalFormsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "HospitalForms";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "HospitalForms_default",
                "HospitalForms/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
