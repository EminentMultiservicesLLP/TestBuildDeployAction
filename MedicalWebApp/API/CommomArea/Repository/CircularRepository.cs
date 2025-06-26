using CGHSBilling.API.CommomArea.Interface;
using CGHSBilling.Areas.CommonArea.Models;
using CGHSBilling.QueryCollection.CommonArea;
using CommonDataLayer.DataAccess;
using CommonLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CGHSBilling.API.CommomArea.Repository
{
    public class CircularRepository : ICircularInterface
    {
        private static readonly ILogger _logger = Logger.Register(typeof(CircularRepository));

        public List<CircularModel> GetCircularDetails()
        {
            List<CircularModel> items = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                DataTable dt = dbHelper.ExecuteDataTable(RoomEntitlementQuesries.GetCircularDetails, paramCollection, CommandType.StoredProcedure);
                items = dt.AsEnumerable()
                    .Select(row => new CircularModel
                    {
                        CircularID = row.Field<int>("CircularID"),
                        CircularName = row.Field<string>("CircularName"),
                    }).ToList();
            }
            return items;
        }

        public List<CircularModel> GetCircularDownloadFileDetails(int CircularID)
        {
            List<CircularModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("CircularID", CircularID, DbType.Int32));
                DataTable dtCircular = dbHelper.ExecuteDataTable(RoomEntitlementQuesries.GetCircularDownloadFileDetails, paramCollection, CommandType.StoredProcedure);
                list = dtCircular.AsEnumerable()
                            .Select(row => new CircularModel
                            {
                                Location = row.Field<string>("Location"),
                                DownloadFileNameAs = row.Field<string>("DownloadFileNameAs"),
                            }).ToList();
            }
            return list;
        }
    }
}