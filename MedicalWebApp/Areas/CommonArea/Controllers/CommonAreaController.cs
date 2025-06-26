using CommonLayer.EncryptDecrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CGHSBilling.Areas.CommonArea.Controllers
{
    public class CommonAreaController : Controller
    {
        public PartialViewResult AboutUs()
        {
            return PartialView();
        }
        public PartialViewResult Feedback()
        {
            return PartialView();
        }
        public PartialViewResult Enquiry()
        {
            return PartialView();
        }
        public PartialViewResult ImedSerbForms()
        {
            return PartialView();
        }
        public PartialViewResult BillingConfiguration()
        {
            return PartialView();
        }

        public PartialViewResult RoomEntitlement()
        {
            return PartialView();
        }
        public PartialViewResult ResetPassword()
        {
            return PartialView();
        }
        public PartialViewResult Marquee()
        {
            return PartialView();
        }
        public PartialViewResult Circular()
        {
            return PartialView();
        }

        public ActionResult Encryptfunction(string strPassword)
        {
            JsonResult jResult;
            try
            {
                string encryptPass = EncryptDecryptDES.EncryptString(strPassword);
                return Json(encryptPass);
            }
            catch (Exception ex)
            {
                return Json("Error");
            }

        }
        public ActionResult Decryptfunction(string strPassword)
        {
            JsonResult jResult;
            try
            {
                string encryptPass = EncryptDecryptDES.DecryptString(strPassword);
                return Json(encryptPass);
            }
            catch (Exception ex)
            {
                return Json("Error");
            }

        }

    }
}
