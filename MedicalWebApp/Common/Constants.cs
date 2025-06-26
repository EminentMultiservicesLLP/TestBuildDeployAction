using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Linq.Expressions;
using System.Reflection;
using System.Net;
using System.Net.NetworkInformation;

namespace CGHSBilling.Common
{
    public enum SurgeryType
    {
        Surgery = 7,
        CancerSurgery = 8
    }

    public class Constants
    {
        public const string JsonTypeResult = "?type=Json";
        public const string XMLTypeResult = "?type=XML";

        public static readonly string WebAPIAddress = Convert.ToString(ConfigurationManager.AppSettings["WebAPIAddress"]);
        public static readonly string ScandocUrl = Convert.ToString(ConfigurationManager.AppSettings["ScandocUrl"]);
        public static readonly bool EnableEmailNotification = Convert.ToBoolean(ConfigurationManager.AppSettings["enableEmailNotification"]);
        public const int AppUserId = 1;
        public static readonly string MacName = HttpContext.Current.Request.UserHostAddress;
        public static readonly string MacId = Convert.ToString(HttpContext.Current.Session["MACAddress"]); // GetMACAddress();
        public static readonly string IpAddress = Convert.ToString(HttpContext.Current.Session["IPAddress"]); //GetIPAddress();

        public static readonly string MailHostAddress = Convert.ToString(ConfigurationManager.AppSettings["MailHostAddress"]);
        public static readonly int MainPortAddress = Convert.ToInt16(ConfigurationManager.AppSettings["MainPortAddress"]);
        public static readonly string MailUserID = Convert.ToString(ConfigurationManager.AppSettings["MailUserID"]);
        public static readonly string MailPWD = Convert.ToString(ConfigurationManager.AppSettings["MailPWD"]);

        public static readonly string AdmissionThresholdTime = Convert.ToString(ConfigurationManager.AppSettings["AdmissionStartTIme"]);
        public static readonly string DischargeThresholdTime = Convert.ToString(ConfigurationManager.AppSettings["AdmissionEndTIme"]);

        public static readonly string MailCommonSender =
            Convert.ToString(ConfigurationManager.AppSettings["MailSenderID"]);

        public static readonly string ClientFileDownloadLoc = Convert.ToString(ConfigurationManager.AppSettings["FileDownloadLocation"]);
        public static readonly string FileDownloadpath = Convert.ToString(ConfigurationManager.AppSettings["FilePath"]);

        public static readonly string PharmacySection = Convert.ToString(ConfigurationManager.AppSettings["PharmacySection"]);
        public static readonly string DefaultSection = Convert.ToString(ConfigurationManager.AppSettings["DefaultSection"]);
        public static readonly string ManualSection = Convert.ToString(ConfigurationManager.AppSettings["ManualSection"]);
        public static readonly string BedSection = Convert.ToString(ConfigurationManager.AppSettings["BedSection"]);
        public static readonly string SurgerySection = Convert.ToString(ConfigurationManager.AppSettings["SurgerySection"]);
        public static readonly string CancerSurgerySection = Convert.ToString(ConfigurationManager.AppSettings["CancerSurgerySection"]);
        public static readonly string LinkingSection = Convert.ToString(ConfigurationManager.AppSettings["LinkingSection"]);

        public static readonly string SurgeryAlertMessage = Convert.ToString(ConfigurationManager.AppSettings["SurgeryAlertMessage"]);
        public static readonly string SurgeryManualSection = Convert.ToString(ConfigurationManager.AppSettings["SurgeryManualSection"]);
        public static readonly double DiscountPercSurgerySecondOnward = Convert.ToDouble(ConfigurationManager.AppSettings["DiscountPercSurgerySecondOnward"]);

        //public static string GetMACAddress()
        //{
        //    NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        //    String sMacAddress = string.Empty;
        //    foreach (NetworkInterface adapter in nics)
        //    {
        //        if (sMacAddress == String.Empty)// only return MAC Address from first card  
        //        {
        //            sMacAddress = adapter.GetPhysicalAddress().ToString();
        //        }
        //    } return sMacAddress;
        //}

        //protected static string GetIPAddress()
        //{
        //    HttpContext context = System.Web.HttpContext.Current;
        //    string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //    if (!string.IsNullOrEmpty(ipAddress))
        //    {
        //        string[] addresses = ipAddress.Split(',');
        //        if (addresses.Length != 0)
        //        {
        //            return addresses[0];
        //        }
        //    }
        //    return context.Request.ServerVariables["REMOTE_ADDR"];
        //}
    }

    public enum ParentMenuEnum
    {
        DashBoards = 1,
        Masters = 2,
        Purchase = 3,
        Store = 4,
        DailyData = 5,
        UserAccess = 6,
        Accounts = 7,
        Miscellaneous = 8,
        VendorProcess = 143,
        Billing = 163,
    }
}