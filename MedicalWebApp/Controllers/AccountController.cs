using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using CGHSBilling.Filters;
using CGHSBilling.Models;
using CommonLayer;
using CGHSBilling.Caching;
using CGHSBilling.API.AdminPanel.Interfaces;
using CGHSBilling.API.AdminPanel.Repositories;
using CommonLayer.EncryptDecrypt;
using System.Configuration;

namespace CGHSBilling.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private static readonly ILogger Logger = CommonLayer.Logger.Register(typeof(AccountController));
        //
        // GET: /Account/Login

        IsignUpInterface _acc;
        public AccountController(IsignUpInterface acc)
        {
            _acc = acc;
        }
        public AccountController()
        {
            _acc = new signUpRepository();
        }


        [AllowAnonymous]
        public ActionResult Login()
        {
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [AllowAnonymous]
        public ActionResult ReturnToLogin()
        {
            return PartialView();
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult Enquiry(EnquiryLogin model)
        {
            bool isSuccess = false; string response = "";
            try
            {
                EmailModel email = new EmailModel()
                {
                    Name = model.Name,
                    Phone = model.Mobile,
                    EmailFrom = model.Email,
                    CompanyName = model.HospitalName,
                    EmailSubject = "Enquiry - Jambo Medical Service",
                    EmailTo = "info@eminentmultiservices.com",
                    EmailBody = model.Message
                };
                new EmailController().SendInquiryEmail(email);
                isSuccess = true;
                response = "Succesfully send Enquiry";
            }
            catch (Exception ex)
            {
                isSuccess = false;
                response = "Failed to send Enquiry";
            }
            return Json(new { response = response, success = isSuccess }, JsonRequestBehavior.AllowGet);
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool IsAdmin = false;
                    Logger.LogInfo("Validating login as :" + model.Username);
                    // Logic to set db from Drop down at Login Page
                    
                    
                    if (model.LoginFor == 0) Session["DatabaseSeLection"] = "DefaultConnection";
                    else Session["DatabaseSeLection"] = "CghsDelhi";
                                     
                    Session["AppUserId"] = model.GetUserId(model.Username, model.Password, out IsAdmin);
                    ClearAllCache();
                    Session["IsAdmin"] = IsAdmin;
                    if (Session["AppUserId"].ToString() == "0" || Session["AppUserId"].ToString() == "")
                    {
                        Logger.LogInfo("Login Failed, User Name or PWD do not match");
                        //return Content("<h1>The user name or password provided is incorrect.</h1>");
                        ViewBag.LoginFailedMsg = "Login Failed, User Name or PWD do not match";
                        return View();
                    }
                    else
                    {
                        Logger.LogInfo("Loggin successful for user:" + model.Username + "(" + Session["AppUserId"].ToString() + ")");
                        FormsAuthentication.SetAuthCookie(model.Password, model.RememberMe);
                        FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                        return RedirectToAction("Index", "Home");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Logger.LogError("Login Failed:", ex);
                return View();
            }
        }

        //
        // POST: /Account/LogOff

        public ActionResult LogOff()
        {
            Logger.LogInfo("Loggin off to User:" + Session["AppUserId"].ToString());
            FormsAuthentication.SignOut();
            Session["AppUserId"] = "";
            ClearAllCache();
            System.Web.HttpContext.Current.Session.Clear();
            System.Web.HttpContext.Current.Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        public void ClearAllCache()
        {
            Logger.LogInfo("Clear All Cache by User:" + Session["AppUserId"].ToString());
            Caching.MemoryCaching.ClearAllCache();
        }
        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }



        [HttpPost]
        [AllowAnonymous]
        public ActionResult SaveSignUpDetails(SignUpModel model)
         {
             bool success = false;
            try
            {
                if (model.LoginFor == 0) Session["DatabaseSeLection"] = "DefaultConnection";
                else Session["DatabaseSeLection"] = "CghsDelhi";
                model.InsertedON = System.DateTime.Now;
                model.InsertedIpAddress = Common.Constants.IpAddress;
                model.InsertedMacId = Common.Constants.MacId;
                model.InsertedMacName = Common.Constants.MacName;
                var Name = model.SignUpContactName;
                model.NameEncrypt = EncryptDecryptDES.EncryptString(Name);
                int  result = 0;
                if ( !IfUserExist(model)) {
                    result = _acc.SaveSignUpDetails(model);
                }
               
                if (result != 0)
                {
                    var mail = SendSignUpDetails(model);
                    if (mail == 0)
                    {
                        success = false;
                        return Json(new { message = "Mail Not Sent!!", success = false }, JsonRequestBehavior.AllowGet);
                    }
                    success = true;
                }
               
                if(success) return Json(new { message = "Sign Up Saved Successfully", success }, JsonRequestBehavior.AllowGet);
                else  return Json(new { message = "Please change either Name or Hospital Name or Email!", success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in SaveSignUp :" + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                return Json(new { message = "Please Contact administrator!" });
            }
        }

        public int SendSignUpDetails(SignUpModel model)
        {
             int Result = 0;
            try
            {
                model.Date = DateTime.Now;
                string body = "From: " + "Eminent MultiServices" + "\n";
                body += "UserName: " + model.SignUpContactName + "\n";
                body += "Password: " + model.SignUpContactName + "\n";
                body += "Subject: " + "Your Sign Up Details!" + "\n";
                body += "Note:" + "Sign Up Details Will Be Valid For 1 Month!";

                EmailModel email = new EmailModel()
                {                
                    EmailSubject = "Your SignUp Details!",
                    EmailTo = model.SignUpemail,
                    EmailBody = body
                };
            
                var check= new EmailController().SignUpDetailsMail(email);                
                if (check != 0)  Result = 1;                                                
            }
            catch (Exception ex)
            {
                Result = 0;
            }
            return Result;
        }


        public bool IfUserExist(SignUpModel model)
        {
            bool success = false;
            success = _acc.CheckDuplicateItem(model.SignUpContactName, model.SignUpHospitalName,model.SignUpemail, 0, "SignUp");                      
            return success;
        }




        [HttpPost]
        public ActionResult SaveResetPassword(ResetPasswordModel model)
        {
            JsonResult jlResult; bool success = false;
            try
            {
                model.UserId = Convert.ToInt32(Session["AppUserId"]);
                var saveuser = _acc.SaveResetPassword(model);
                if (saveuser != 0)
                {
                    success = true;
                    jlResult = Json(new { message = "Password Reset Successfully!", success = true }, JsonRequestBehavior.AllowGet);
                    Logger.LogInfo("Loggin off to User:" + Session["AppUserId"].ToString());
                    FormsAuthentication.SignOut();
                    Session["AppUserId"] = "";
                    MemoryCaching.ClearAllCache();
                }
                else
                {
                    success = false;
                    jlResult = Json(new { success = false, message = "Record Save Failed" });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in AccountController SaveResetPassword" + ex.Message + Environment.NewLine + ex.StackTrace);
                jlResult = Json(new { message = "Something Went Wrong", success = false }, JsonRequestBehavior.AllowGet);
            }
            return jlResult;
        }










    }

    #endregion
}

