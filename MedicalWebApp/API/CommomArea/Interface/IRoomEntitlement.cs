using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGHSBilling.Areas.CommonArea.Models;
using CGHSBilling.Models;

namespace CGHSBilling.API.CommomArea.Interface
{
    public interface IRoomEntitlement 
    {
        MenuUserRightsModel SaveMarqueeMessage(MenuUserRightsModel model);
        List<RoomEntitlement> GetRoomType(RoomEntitlement model);
    }
}