using CGHSBilling.Models;
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
    public class EmailController : Controller
    {
        public void SendInquiryEmail(EmailModel mails)
        {
            var fromAddress = mails.EmailFrom;
            string toAddress = ConfigurationManager.AppSettings["mailToAddress"];
            string ccAddress = ConfigurationManager.AppSettings["mailCCAddress"];
            string bccAddress = ConfigurationManager.AppSettings["mailBCCAddress"];
            string userId = ConfigurationManager.AppSettings["mailUserID"];
            string password = ConfigurationManager.AppSettings["mailPassword"];
            string subject = mails.EmailSubject.ToString();
            string body = "Enquiry raised on Jambo Medical service \n \n";
            body += "From: " + mails.Name + "\n";
            body += "Contact: " + mails.Phone + "\n";
            body += "Email: " + mails.EmailFrom + "\n";
            body += "Hospital Name: " + mails.CompanyName + "\n \n";
            body += "Message: \n" + mails.EmailBody + "\n";
            password = CommonLayer.EncryptDecrypt.EncryptDecryptDES.DecryptString(password);
            var smtp = new System.Net.Mail.SmtpClient();
            {
                smtp.Host = ConfigurationManager.AppSettings["smtpHost"];
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPortNo"]);
                smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["sslSecurityStatus"]);
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            
                smtp.Credentials = new NetworkCredential(userId, password);
                smtp.Timeout = 20000;
            }
            
            //smtp.Send(userId, toAddress, subject, body);
            MailAddress from = new MailAddress(userId);
            MailAddress to = new MailAddress(toAddress);
            MailAddress cc = new MailAddress(ccAddress);
            MailAddress bcc = new MailAddress(bccAddress);
            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = body;
            message.Bcc.Add(bcc);message.CC.Add(cc);

            try
            {
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        public bool SendFeedbackEmail(EmailModel mails)
        {
            bool success = false;
            var fromAddress = mails.EmailFrom;
            string toAddress = ConfigurationManager.AppSettings["mailToAddress"];
            string toPassword = ConfigurationManager.AppSettings["mailToPassword"];
            //var toAddress = "mechconmails@gmail.com";
            //const string toPassword = "mechcon@admin";
            string subject = mails.EmailSubject.ToString();
            string body = "From: " + mails.Name + "\n";
            body += "Email: " + mails.EmailFrom + "\n";
            body += "Message: " + mails.EmailBody + "\n";
            try
            {
                var smtp = new System.Net.Mail.SmtpClient();
                {
                    smtp.Host = ConfigurationManager.AppSettings["smtpHost"];
                    smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPortNo"]);
                    smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["sslSecurityStatus"]);
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(toAddress, toPassword);
                    smtp.Timeout = 20000;
                }
                smtp.Send(toAddress, toAddress, subject, body);
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

            return success;

        }

        public int SignUpDetailsMail(EmailModel mails)
        {
            int success = 0;
            MailMessage message = new MailMessage();
            string userId = ConfigurationManager.AppSettings["mailUserID"];
            string password = ConfigurationManager.AppSettings["mailPassword"];
            string ccAddress = ConfigurationManager.AppSettings["mailCCAddress"];
            password = CommonLayer.EncryptDecrypt.EncryptDecryptDES.DecryptString(password);           
            string[] MultipleToAddress = new String[2];
            MultipleToAddress[0] = mails.EmailTo;
            MultipleToAddress[1] =  ConfigurationManager.AppSettings["mailToAddress"];
            string subject = mails.EmailSubject.ToString();
            string body = mails.EmailBody;
            try
            {
                var smtp = new System.Net.Mail.SmtpClient();
                {
                    smtp.Host = ConfigurationManager.AppSettings["smtpHost"];
                    smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPortNo"]);
                    smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["sslSecurityStatus"]);
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential(userId, password);
                    smtp.Timeout = 20000;
                }
                
                foreach(string toaddress in MultipleToAddress)
                {
                    message.To.Add(new MailAddress(toaddress));
                }
                MailAddress cc = new MailAddress(ccAddress);                
                message.From = new MailAddress(userId);
                message.Subject = subject;
                message.Body = body;               
                message.CC.Add(cc);
                smtp.Send(message);
                success = 1;                                
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                success = 0;
            }
            return success;

        }


        public void LimitExceededMail()
        {

            string userId = ConfigurationManager.AppSettings["mailUserID"];
            string password = ConfigurationManager.AppSettings["mailPassword"];
            string ccAddress = ConfigurationManager.AppSettings["mailCCAddress"];
            password = CommonLayer.EncryptDecrypt.EncryptDecryptDES.DecryptString(password);
            string toAddress =ConfigurationManager.AppSettings["mailToAddress"];
            string division = System.Web.HttpContext.Current.Session["DatabaseSeLection"]!= null 
                              && System.Web.HttpContext.Current.Session["DatabaseSeLection"].ToString() == "DefaultConnection" 
                              ? "Mumbai divison" : "India divison";
            var UserName = System.Web.HttpContext.Current.User.Identity.Name;
            string body = "Limit Exceed on Jambo Medical service for User "+ UserName + " in " + division+ ".\n \n ";
            body += " Kindly deactivate user.";
            var smtp = new System.Net.Mail.SmtpClient();
            {
                smtp.Host = ConfigurationManager.AppSettings["smtpHost"];
                smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPortNo"]);
                smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["sslSecurityStatus"]);
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                smtp.Credentials = new NetworkCredential(userId, password);
                smtp.Timeout = 20000;
            }

            //smtp.Send(userId, toAddress, subject, body);
            MailAddress from = new MailAddress(userId);
            MailAddress to = new MailAddress(toAddress);
            MailAddress cc = new MailAddress(ccAddress);    
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Limit Exceeded Mail";
            message.Body = body;
            message.CC.Add(cc);

            try
            {
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }

        }

         

        }
    }