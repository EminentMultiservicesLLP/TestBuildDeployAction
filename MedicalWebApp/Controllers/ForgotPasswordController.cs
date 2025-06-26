using CGHSBilling.API.ForgotPassword.Interfaces;
using CGHSBilling.API.ForgotPassword.Repositories;
using CGHSBilling.Models;
using CommonLayer;
using CommonLayer.EncryptDecrypt;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace CGHSBilling.Controllers
{
    public class ForgotPasswordController : Controller
    {
        IForgotPasswordInterface _ForgotPW;

        private static readonly ILogger _loggger = Logger.Register(typeof(ForgotPasswordController));

        public ForgotPasswordController(IForgotPasswordInterface ForgotPW)
        {
            _ForgotPW = ForgotPW;
        }

        public ForgotPasswordController()
        {
            _ForgotPW = new ForgotPasswordRepository();
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword(string Username)
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult SaveForgotPasswordDetails(ForgotPasswordModel model)
        //{
        //    JsonResult jlResult; bool success = false;
        //    try
        //    {
        //        var NewPassword = model.NewPassword;
        //        var ConfirmPassword = model.ConfirmPassword;
        //        if (NewPassword != ConfirmPassword)
        //        {
        //            jlResult = Json(new { message = "NewPassword and ConfirmPassword Must be Same!!", success = false }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            var saveForgotpassword = _ForgotPW.SaveForgotPassword(model);
        //            if (saveForgotpassword == 1)
        //            {
        //                success = true;
        //                jlResult = Json(new { message = "Record Saved Successfully!", success = true }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                success = false;
        //                jlResult = Json(new { success = false, message = "Record Save Failed" });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _loggger.LogError("Error in ForgotPasswordController SaveForgotPasswordDetails" + ex.Message + Environment.NewLine + ex.StackTrace);
        //        jlResult = Json(new { message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
        //    }
        //    return jlResult;
        //}

        public ActionResult UpdatePassword(ForgotPasswordModel model)
        {
            JsonResult jResult; bool success = false;

            var username = model.UserName;
            try
            {
                if (model.LoginFor == 0) Session["DatabaseSeLection"] = "DefaultConnection";
                else Session["DatabaseSeLection"] = "CghsDelhi";

                model.Password = EncryptDecryptDES.EncryptString(username);
                string email = _ForgotPW.UpdatePassword(model);
                if (!string.IsNullOrEmpty(email))
                {
                    model.EmailID = email;
                    var sendmail = SendMail(model);
                    success = true;
                    jResult = Json(new { Message = "Mail has been sent to your registered Email Id.", success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    jResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception Ex)
            {
                _loggger.LogError("Error in UpdatePassword :", Ex);
                jResult = Json(new { Message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jResult;
        }

        public int SendMail(ForgotPasswordModel model)
        {
            int result = 0;
            try
            {            
                string userId = ConfigurationManager.AppSettings["mailUserID"];
                string password = ConfigurationManager.AppSettings["mailPassword"];
                password = CommonLayer.EncryptDecrypt.EncryptDecryptDES.DecryptString(password);
                string toAddress = model.EmailID;
                string subject = "Login Details of User";
                string emailbody = "Dear " + model.UserName + "\n" + "Your UserName : " + model.UserName + "\n"
                                    + "Your Password : " + model.Password + "\n";
                string body = "From: " + "Eminent Multiservices" + "\n";

                body += "Subject : " + subject + "\n";
                body += "UserName : " + model.UserName + "\n";
                body += "Password : " + model.UserName + "\n";         

                // smtp settings
                var smtp = new System.Net.Mail.SmtpClient();
                {
                    smtp.Host = ConfigurationManager.AppSettings["smtpHost"];
                    smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPortNo"]);
                    smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["sslSecurityStatus"]);
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(userId, password);
                    smtp.Timeout = 20000;
                }
                // Passing values to smtp object
                //smtp.Send(fromAddress, toAddress, subject, body);
                MailAddress from = new MailAddress(userId);
                MailAddress to = new MailAddress(toAddress);
                MailMessage message = new MailMessage(from, to);
                message.Body = body;
                message.Subject = subject;
                smtp.Send(message);
                result = 1;
            }
            catch (Exception ex)
            {
                _loggger.LogError("Error in ForgotPasswordController SendEMail :" + ex.Message + Environment.NewLine + ex.StackTrace);
                result = 0;
            }
            
            return result;
        }

    }
}
