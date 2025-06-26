using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGHSBilling.QueryCollection.CommonArea
{
    public class RoomEntitlementQuesries
    {
        public const string GetRoomType = "dbsp_GetRoomType";
        public const string SaveMarqueeMessage = "dbsp_SaveMarqueeMessage";

        //SP For Circular
        public const string GetCircularDetails = "dbsp_GetCircularDetails";
        public const string GetCircularDownloadFileDetails = "dbsp_GetCircularDownloadFileDetails";
    }
}