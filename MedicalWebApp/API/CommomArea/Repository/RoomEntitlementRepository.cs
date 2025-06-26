using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGHSBilling.Areas.CommonArea.Models;
using CommonDataLayer.DataAccess;
using System.Data;
using CGHSBilling.QueryCollection.CommonArea;
using CGHSBilling.API.CommomArea.Interface;
using CGHSBilling.Models;

namespace CGHSBilling.API.CommomArea.Repository
{
    public class RoomEntitlementRepository : IRoomEntitlement
    {

        public List<RoomEntitlement> GetRoomType(RoomEntitlement model)
        {

            List<RoomEntitlement> roomType = null;
            using (DBHelper dbHelper = new DBHelper())
            {


                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("BillType", model.BillType, DbType.Int32));
                paramCollection.Add(new DBParameter("YearOfRetirement", model.YearOfRetirement, DbType.Int32));
                paramCollection.Add(new DBParameter("LastSalaryDrawn",Int32.Parse(model.LastSalaryDrawn), DbType.Int32));

                DataTable dtStores = dbHelper.ExecuteDataTable(RoomEntitlementQuesries.GetRoomType, paramCollection, CommandType.StoredProcedure);
                roomType = dtStores.AsEnumerable()
                            .Select(row => new RoomEntitlement
                            {
                                RoomType = row.Field<string>("roomtype"),

                            }).ToList();

            }
            return roomType;

        }


        public MenuUserRightsModel SaveMarqueeMessage(MenuUserRightsModel model)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("MarqueeMessage", model.MarqueeMessage, DbType.String));
                dbHelper.ExecuteNonQuery(RoomEntitlementQuesries.SaveMarqueeMessage, paramCollection, CommandType.StoredProcedure);
            }
            return model;
        }
    }
}