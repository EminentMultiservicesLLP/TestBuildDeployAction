using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer.Extensions
{
    public static class DateTimeExtensions
    {
        public static string DateTimeFormat1(this DateTime? dt)
        {
            if (dt == null) return "";
            else return Convert.ToDateTime(dt).ToString("dd-MMMM-yyyy");
        }
        public static string DateTimeFormat2(this DateTime? dt)
        {
            if (dt == null) return "";
            else return Convert.ToDateTime(dt).ToString("dd-MM-yyyy");
        }

        public static string TimeFormat(this DateTime? dt)
        {
            if (dt == null) return "";
            else return Convert.ToDateTime(dt).ToString("HH:mm");
        }
    }
}
