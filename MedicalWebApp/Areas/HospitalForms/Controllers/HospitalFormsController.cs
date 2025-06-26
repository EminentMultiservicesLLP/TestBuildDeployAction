using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CGHSBilling.Areas.HospitalForms.Controllers
{
    public class HospitalFormsController : Controller
    {
        public PartialViewResult RequestSubmission()
        {
            return PartialView();
        }
        public PartialViewResult RequestSubmissionNew()
        {
            return PartialView();
        }
        public PartialViewResult RequestSubmissionOPD()
        {
            return PartialView();
        }

        public PartialViewResult RequestSubmissionReport()
        {
            return PartialView();
        }

        public PartialViewResult MakePayment()
        {
            return PartialView("Payment");
        }

        public PartialViewResult ServiceWiseBillDetails()
        {
            return PartialView();
        }
        public PartialViewResult BCertificate()
        {
            return PartialView();
        }
    }
}
