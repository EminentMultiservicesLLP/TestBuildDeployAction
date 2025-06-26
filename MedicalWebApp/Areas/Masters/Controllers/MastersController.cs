using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CGHSBilling.Areas.Masters.Controllers
{
    public class MastersController : Controller
    {
        //
        // GET: /Masters/Masters/

        public PartialViewResult ServiceTypeMaster()
        {
            return PartialView();
        }

        public PartialViewResult ServiceMaster()
        {
            return PartialView();
        }

        public PartialViewResult TariffMaster()
        {
            return PartialView();
        }
        public PartialViewResult CopyTariffMaster()
        {
            return PartialView();
        }
        public PartialViewResult HintNotification()
        {
            return PartialView();
        }
        public PartialViewResult PatientTypeMaster()
        {
            return PartialView();
        }
        public PartialViewResult AutoServiceAllocation()
        {
            return PartialView();
        }
        public PartialViewResult SurgeryMaster()
        {
            return PartialView();
        }
        public PartialViewResult HospitalServiceCategory()
        {
            return PartialView();
        }
        public PartialViewResult UpdateRates()
        {
            return PartialView();
        }
        public PartialViewResult UpdateCityRates()
        {
            return PartialView();
        }
        public PartialViewResult CopyTariff()
        {
            return PartialView();
        }
        public PartialViewResult ClientConfiguration()
        {
            return PartialView();
        }
    }
}
