using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CGHSBilling.API.Masters.Interfaces;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.QueryCollection.HospitalForms;
using CGHSBilling.QueryCollection.Masters;
using CommonDataLayer.DataAccess;
using CommonLayer;
using CommonLayer.Extensions;

namespace CGHSBilling.API.Masters.Repositories
{
    public class HintNotificationRepository: IHintNotificationRepository
    {
        private static readonly ILogger Loggger = Logger.Register(typeof(HintNotificationRepository));
        public bool CreateNotification(HintNotificationModel jsonData)
        {
            bool success = false;
            TryCatch.Run(() =>
            {
                using (DBHelper dbHelper = new DBHelper())
                {
                    DBParameterCollection paramCollection = new DBParameterCollection();
                    paramCollection.Add(new DBParameter("NotificationId", jsonData.NotificationId, DbType.Int32));
                    paramCollection.Add(new DBParameter("ControlId", jsonData.ControlId, DbType.String));
                    paramCollection.Add(new DBParameter("ControlName", jsonData.ControlName, DbType.String));
                    paramCollection.Add(new DBParameter("Message", jsonData.Message, DbType.String));
                    paramCollection.Add(new DBParameter("StepNo", jsonData.StepNo, DbType.String));
                    paramCollection.Add(new DBParameter("SubControlId", jsonData.SubControlId, DbType.String));
                    dbHelper.ExecuteNonQuery(MasterQueries.CreateNotification, paramCollection,CommandType.StoredProcedure);
                }

                success = true;
            }).IfNotNull(ex =>
            {
                success = false;
                Loggger.LogError("error at CreateNotification of HintNotificationRepository" + ex.Message + Environment.NewLine + ex.StackTrace);
            });
            return success;
        }

        public bool UpdateNotification(HintNotificationModel jsonData)
        {
            throw new NotImplementedException();
        }

        public List<HintNotificationModel> GetAllNotification()
        {
            List<HintNotificationModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAllNotification, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new HintNotificationModel
                    {
                        NotificationId = row.Field<int>("NotificationId"),
                        StepNo = row.Field<int>("StepNo"),
                        ControlId = row.Field<string>("ControlId"),
                        ControlName = row.Field<string>("ControlName"),
                        Message = row.Field<string>("Message"),
                        SubControlId = row.Field<int>("SubControlId"),
                    }).ToList();
            }
            return list;
        }

        public HintNotificationModel GetAllNotificationById(string controlId)
        {
            throw new NotImplementedException();
        }
    }
}