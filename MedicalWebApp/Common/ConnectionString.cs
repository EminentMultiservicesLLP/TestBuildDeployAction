using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CGHSBilling.Common
{
    public class ConnectionString
    {

        public string getConnectionStringName()
        {

            if (HttpContext.Current.Session["DatabaseSeLection"] != null)           
               return Convert.ToString(HttpContext.Current.Session["DatabaseSeLection"]);            
            else
               return Convert.ToString("DefaultConnection");
        }


        public string getFullConnectionString()
        {

            if (HttpContext.Current.Session["DatabaseSeLection"] != null) {
                string connectionStringToBeSelected = Convert.ToString(HttpContext.Current.Session["DatabaseSeLection"]);
                return ConfigurationManager.ConnectionStrings[connectionStringToBeSelected].ConnectionString;
            }
            else            
                return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            
        }


        public string getHopeConnectionString()
        {
            return Convert.ToString("HopeConnection");
        } 


    }
}