using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using paytm;
using CommonLayer;
using CGHSBilling.API.HospitalForms.Intefaces;
using CGHSBilling.API.HospitalForms.Repository;
using System.Configuration;
using CGHSBilling.Areas.HospitalForms.Models;
using System.Net.Http;
using System.Net;

namespace CGHSBilling.Areas.HospitalForms.Controllers
{
    public class PaymentController : Controller
    {
        IPaymentRepository _data;
        private static readonly ILogger Logger = CommonLayer.Logger.Register(typeof(PaymentController));

        public PaymentController()
        {
            _data = new PaymentRepository();
        }

        [HttpPost]
        public ActionResult Payment(string amount)
        {
            string PaymentType = "Paytm";
            string PaymentStatus = "NotPaid";
            string OrderId = Convert.ToString(Session["AppUserId"]) + DateTime.Now.ToString("ddMMyyyyhhmmsss");

            DateTime PaymentDate = DateTime.Now;
            string userId = Convert.ToString(Session["AppUserId"]);

            string ConnectionString = Convert.ToString(Session["DatabaseSeLection"]) == "Mumbai" ? "DefaultConnection" : "CghsDelhi";
            TryCatch.Run(() =>
            {
                _data.MakePayment(userId, OrderId, amount, PaymentDate, PaymentType, PaymentStatus, ConnectionString);
            });

            string customerid = Session["DatabaseSeLection"] + "_" + Session["AppUserId"];
            string email = Convert.ToString(ConfigurationManager.AppSettings["mailToAddress"]);
            string mobile = Convert.ToString(ConfigurationManager.AppSettings["mobile"]);

            string ss = PaytmPayment(email, mobile, customerid, OrderId, amount, Url.Action("PaytmResponse").ToString());

            //String merchantKey = "AdSSyU21Le%9&1TI";
            //List < KeyValuePair < string, string>> parameters = new List<KeyValuePair<string, string>>();
            //parameters.Add(new KeyValuePair<string, string>("MID", "YSjrrm56602282924689"));
            //parameters.Add(new KeyValuePair<string, string>("CHANNEL_ID", "Web"));
            //parameters.Add(new KeyValuePair<string, string>("INDUSTRY_TYPE_ID", "Software service"));
            //parameters.Add(new KeyValuePair<string, string>("WEBSITE", "www.cgmsbill.com"));
            //parameters.Add(new KeyValuePair<string, string>("EMAIL", email));
            //parameters.Add(new KeyValuePair<string, string>("MOBILE_NO", mobile));
            //parameters.Add(new KeyValuePair<string, string>("CUST_ID", customerid));
            //parameters.Add(new KeyValuePair<string, string>("ORDER_ID", "DDDDDD"));
            //parameters.Add(new KeyValuePair<string, string>("TXN_AMOUNT", amount));
            //parameters.Add(new KeyValuePair<string, string>("CALLBACK_URL", "https://securegw-stage.paytm.in/theia/processTransaction")); //This parameter is not mandatory. Use this to pass the callback url dynamically.
            //string paytmChecksum = CheckSum.generateCheckSum(merchantKey, parameters);
            //parameters.Add(new KeyValuePair<string, string>("CHECKSUMHASH", paytmChecksum));

            ////return Json(parameters);
            //string ss = PaytmPayment(email, mobile, customerid, OrderId, amount, Url.Action("PaytmResponse").ToString());
            ////var response = new HttpResponseMessage(HttpStatusCode.Accepted)
            ////{
            ////    Content = new StringContent(ss)
            ////};

            //using (var client = new HttpClient())
            //{
            //    var values = new List<KeyValuePair<string, string>>();
            //    values.Add(new KeyValuePair<string, string>("n", "42"));
            //    values.Add(new KeyValuePair<string, string>("s", "string value"));

            //    var content = new FormUrlEncodedContent(values);

            //    var response = await client.PostAsync("https://securegw-stage.paytm.in/theia/processTransaction", content);

            //    var responseString = await response.Content.ReadAsStringAsync();
            //}

            //HttpContext.Response.Output.Write(ss);
            return Json(new { response = ss });
            //return PartialView();
        }

        //[HttpPost]
        //public ActionResult PaytmResponse()
        //{

        //}



        private string PaytmPayment(string email, string mobile, string customerid, string orderid, string amount, string callbackurl)
        {
            string outputHTML = "<html>";
            String merchantKey = "tdG#a5Z1Fe0wJeoM";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("MID", "kvTEEb07026139038738");
            parameters.Add("CHANNEL_ID", "Web");
            parameters.Add("INDUSTRY_TYPE_ID", "Software service");
            parameters.Add("WEBSITE", "www.cgmsbill.com");
            parameters.Add("EMAIL", email);
            parameters.Add("MOBILE_NO", "7777777777");
            parameters.Add("CUST_ID", customerid);
            parameters.Add("ORDER_ID", orderid);
            parameters.Add("TXN_AMOUNT", amount);
            parameters.Add("CALLBACK_URL", callbackurl); //This parameter is not mandatory. Use this to pass the callback url dynamically.

            string paytmChecksum = CheckSum.generateCheckSum(merchantKey, parameters);

            string paytmURL = "https://securegw-stage.paytm.in/theia/processTransaction?orderid=" + orderid;

            // for staging 
            //string transactionURL = "https://securegw-stage.paytm.in/theia/processTransaction";
            // for production 
            // string transactionURL = "https://securegw.paytm.in/theia/processTransaction"; 
            try
            {

                //outputHTML += "<head>";
                //outputHTML += "<title>Merchant Checkout Page</title>";
                //outputHTML += "</head>";
                //outputHTML += "<body style='color:black'>";
                //outputHTML += "<center><h1>Please do not refresh this page...</h1></center>";
                //outputHTML += "<form method='post' action='" + transactionURL + "' name='f1'>";
                //foreach (string key in parameters.Keys)
                //{
                //    outputHTML += "<input type='hidden' name='" + key + "' value='" + parameters[key] + "'>";
                //}
                //outputHTML += "<input type='hidden' name='CHECKSUMHASH' value='" + paytmChecksum + "'>";
                //outputHTML += "<script type='text/javascript'>";
                //outputHTML += "document.f1.submit();";
                //outputHTML += "</script>";
                //outputHTML += "</form>";
                //outputHTML += "</body>";
                //outputHTML += "</html>";


                outputHTML += "<head>";
                outputHTML += "<title>Merchant Check Out Page</title>";
                outputHTML += "</head>";
                outputHTML += "<body>";
                outputHTML += "<center><h1>Please do not refresh this page...</h1></center>";
                outputHTML += "<form method='post' action='" + paytmURL + "' name='f1'>";
                outputHTML += "<table border='1'>";
                outputHTML += "<tbody>";
                foreach (string key in parameters.Keys)
                {
                    outputHTML += "<input type='hidden' name='" + key + "' value='" + parameters[key] + "'>";
                }
                outputHTML += "<input type='hidden' name='CHECKSUMHASH' value='" + paytmChecksum + "'>";
                outputHTML += "</tbody>";
                outputHTML += "</table>";
                outputHTML += "<script type='text/javascript'>";
                outputHTML += "document.f1.submit();";
                outputHTML += "</script>";
                outputHTML += "</form>";
                outputHTML += "</body>";
                outputHTML += "</html>";

                //Response.Write(outputHTML);
                ViewBag.PaymentPage = outputHTML;
            }
            catch (Exception ex)
            {
                Response.Write("Exception message: " + ex.Message.ToString());
            }
            //HttpContext.Response.Write(outputHTML);

            return outputHTML;
        }

        

        [HttpPost]
        public ActionResult PaytmResponse(PaytmResponseModel data)
        {
            String merchantKey = "tdG#a5Z1Fe0wJeoM";
            Dictionary<String, String> paytmParams = new Dictionary<String, String>();
            string paytmChecksum = "";
            foreach (string key in Request.Form.Keys)
            {
                paytmParams.Add(key.Trim(), Request.Form[key].Trim());
            }
            if (paytmParams.ContainsKey("CHECKSUMHASH"))
            {
                paytmChecksum = paytmParams["CHECKSUMHASH"];
                paytmParams.Remove("CHECKSUMHASH");
            }
            bool isValidChecksum = CheckSum.verifyCheckSum(merchantKey, paytmParams, paytmChecksum);
            if (isValidChecksum)
            {
                Response.Write("Checksum Matched");
            }
            else
            {
                Response.Write("Checksum MisMatch");
            }

            return View();
        }


    }
}
