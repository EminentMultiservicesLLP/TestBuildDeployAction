using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web;

namespace CommonDataLayer.DataAccess
{
    internal static class Configuration
    {
        const string DEFAULT_CONNECTION_KEY = "defaultConnection";
        const string DIFFERENT_CONNECTION_KEY = "cghsdelhi";

        public static string DefaultConnection
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Session["DatabaseSeLection"] == "DefaultConnection")
                        return ConfigurationManager.AppSettings[DEFAULT_CONNECTION_KEY];
                    else
                        return ConfigurationManager.AppSettings[DIFFERENT_CONNECTION_KEY];

                } else
                {
                    return ConfigurationManager.AppSettings[DEFAULT_CONNECTION_KEY];
                }

                
                 // return ConfigurationManager.AppSettings[DEFAULT_CONNECTION_KEY];
            }
        }

        public static string ProviderName
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[DefaultConnection].ProviderName;
            }
        }

        public static string ConnectionString
        {
            get
            {
               // return "Server=192.168.2.212;Database=BISNowERP;User Id=sa;Password=optimal$2009;";
                return ConfigurationManager.ConnectionStrings[DefaultConnection].ConnectionString;
            }
        }

        public static string GetConnectionString(string connectionName)
        {
            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }

        public static string GetProviderName(string connectionName)
        {

            return Convert.ToString(ConfigurationManager.ConnectionStrings[connectionName].ProviderName);
        }

    }
}
