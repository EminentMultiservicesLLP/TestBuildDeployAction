using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.Areas.HospitalForms.Models
{
    public class ChargeDetailsDayWise
    {
        public int RoomTypeId { get; set; }
        public int Day { get; set; }
        public double BillRate { get; set; }
    }
    public static class CalculateDateTimeRate
    {
        public static int CalculateDays(DateTime admissionDateTime, DateTime dischargeDateTime)
        {
            int yr = 2018, mn = 10, dy = 12, hr = 06, min = 00, se = 00;
            int yr1 = 2018, mn1 = 10, dy1 = 12, hr1 = 12, min1 = 00, se1 = 00;

            DateTime admissionTime = admissionDateTime; //new DateTime(yr, mn, dy, hr, min, se);
            DateTime dischargeTime = dischargeDateTime;//new DateTime(yr1, mn1, dy1, hr1, min1, se1);
            double daysCount = 0;
            if (admissionTime.TimeOfDay <
                new DateTime(admissionTime.Year, admissionTime.Month, admissionTime.Day, 6, 0, 0).TimeOfDay)
            {
                admissionTime = new DateTime(admissionTime.Year, admissionTime.Month, admissionTime.Day - 1, 6, 0, 0);
            }

            if (dischargeTime.TimeOfDay >
                new DateTime(dischargeTime.Year, dischargeTime.Month, dischargeTime.Day, 23, 59, 59).TimeOfDay)
            {
                dischargeTime = new DateTime(dischargeTime.Year, dischargeTime.Month, dischargeTime.Day + 1, 12, 00, 00);
            }

            var totaldays = (dischargeTime - admissionTime).TotalDays;
            return Convert.ToInt32(Math.Ceiling(totaldays));
        }

       // public int CalculateHigherRoomRate(DateTime admissionDateTime, DateTime dischargeDateTime)
       // {
            ////DateTime st = new DateTime(startTimes[0].Year, startTimes[0].Month, startTimes[0].Day, 06, 00, 00);
            ////DateTime et = st.AddHours(30); var days = 0; List<RoomType> temp = new List<RoomType>();
            ////List<RoomType> finalRoomCharge = new List<RoomType>();
            ////List<ChargeDetailsDayWise> _ChargeDetailsDayWise = new List<ChargeDetailsDayWise>();

            //for (int i = 0; i < startTimes.Count; i++)
            //{
            //    temp.Add(s[i]);
            //    if (endTimes[i] >= et)
            //    {
            //        var tempDt = new DateTime(endTimes[i].Year, endTimes[i].Month, endTimes[i].Day, 12, 00, 00);
            //        days = days + tempDt.Subtract(st).Days;
            //        for (int iDay = 0; i < tempDt.Subtract(st).Days; iDay++)
            //        {
            //            _ChargeDetailsDayWise.Add(
            //                new ChargeDetailsDayWise {Day = iDay, DayRoomTypeApplied = temp.Min()});
            //        }

            //        st = tempDt;
            //        et = st.AddHours(24);
            //        //finalRoomCharge.Add(temp.Min());//   Enum.GetValues(typeof(troemp)).Cast<int>().Min);
            //        temp.Clear();

            //        //if (i == startTimes.Count - 1 && DateTime.Compare(endTimes[i], st) >= 1)
            //        //{
            //        //    days = days + 1;
            //        //    _ChargeDetailsDayWise.Add(new ChargeDetailsDayWise { Day = i, DayRoomTypeApplied = temp.Min() });
            //        //}

            //    }
            //    else if (i == startTimes.Count - 1)
            //    {
            //        _ChargeDetailsDayWise.Add(new ChargeDetailsDayWise {Day = i, DayRoomTypeApplied = temp.Min()});
            //        days = days + 1;
            //    }
            //}

            //bool roomTypeFailed = false;
            //for (int i = 0; i < finalRoomCharge.Count(); i++)
            //{
            //    if (finalRoomCharge[0] != RoomType.ICU)
            //        roomTypeFailed = true;
            //    if (finalRoomCharge[1] != RoomType.Private)
            //        roomTypeFailed = true;

            //    if (!roomTypeFailed)
            //        Console.WriteLine("Failed Room Type, Expected room type:" + RoomType.ICU.ToString() +
            //                          "  --> but received:" + finalRoomCharge[0].ToString());

            //}

        //}

    }
}