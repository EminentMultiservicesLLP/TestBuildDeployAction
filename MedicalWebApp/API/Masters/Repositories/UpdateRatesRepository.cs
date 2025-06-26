using CGHSBilling.API.Masters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGHSBilling.Areas.Masters.Models;
using CommonDataLayer.DataAccess;
using System.Data;
using CGHSBilling.QueryCollection.Masters;
using CommonLayer;

namespace CGHSBilling.API.Masters.Repositories
{
    public class UpdateRatesRepository : IUpdateRates
    {
        private static readonly ILogger Loggger = Logger.Register(typeof(UpdateRatesRepository));
        public List<UpdateRatesModel> GetRates(int StateId, int CityId)
        {

            List<UpdateRatesModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("StateId", StateId, DbType.Int32));
            paramCollection.Add(new DBParameter("CityId", CityId, DbType.Int32));

            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtTable = dbHelper.ExecuteDataTable(MasterQueries.GetRates, paramCollection, CommandType.StoredProcedure);
                list = dtTable.AsEnumerable()
                   .Select(row => new UpdateRatesModel
                   {
                       ServiceId = row.Field<int>("ServiceId"),
                       Code = row.Field<string>("Code"),
                       ServiceType = row.Field<string>("ServiceType"),
                       ServiceName = row.Field<string>("ServiceName"),
                       RoomType = row.Field<string>("RoomType"),
                       PatientType = row.Field<string>("PatientType"),
                       TariffDtlId = row.Field<int>("TariffDtlId"),
                       TariffMasterId = row.Field<int>("TariffMasterId"),
                       NABHRate = row.Field<double>("NABHRate"),
                       NonNABHRate = row.Field<double>("NonNABHRate"),
                       CghsCode = row.Field<string>("CghsCode")
                   }).ToList();
            }
            return list;

        }
        public List<UpdateRatesModel> GetServiceTariff(int? ServiceId)
        {

            List<UpdateRatesModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ServiceId", ServiceId, DbType.Int32));

            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtTable = dbHelper.ExecuteDataTable(MasterQueries.GetServiceTariff, paramCollection, CommandType.StoredProcedure);
                list = dtTable.AsEnumerable()
                   .Select(row => new UpdateRatesModel
                   {
                       ServiceId = row.Field<int>("ServiceId"),
                       Code = row.Field<string>("Code"),
                       ServiceType = row.Field<string>("ServiceType"),
                       ServiceName = row.Field<string>("ServiceName"),
                       RoomType = row.Field<string>("RoomType"),
                       PatientType = row.Field<string>("PatientType"),
                       TariffDtlId = row.Field<int>("TariffDtlId"),
                       TariffMasterId = row.Field<int>("TariffMasterId"),
                       NABHRate = row.Field<double>("NABHRate"),
                       NonNABHRate = row.Field<double>("NonNABHRate"),
                       CghsCode = row.Field<string>("CghsCode"),
                       CityName = row.Field<string>("CityName"),
                   }).ToList();
            }
            return list;

        }

        public int UpdateRates(List<UpdateRatesModel> UpdateRates)
        {
            int Result = 0;
            using (DBHelper dbHelper = new DBHelper())
            {
                IDbTransaction transaction = dbHelper.BeginTransaction();
                try
                {
                    foreach (var detail in UpdateRates)
                    {
                        DBParameterCollection paramCollection = new DBParameterCollection();
                        paramCollection.Add(new DBParameter("TariffDtlId", detail.TariffDtlId, DbType.Int32));
                        paramCollection.Add(new DBParameter("NABHRate", detail.NABHRate, DbType.Double));
                        paramCollection.Add(new DBParameter("NonNABHRate", detail.NonNABHRate, DbType.Double));
                        dbHelper.ExecuteNonQuery(MasterQueries.UpdateRates, paramCollection, CommandType.StoredProcedure);
                    }
                    dbHelper.CommitTransaction(transaction);
                    Result = 1;
                }
                catch (Exception ex)
                {
                    dbHelper.RollbackTransaction(transaction);
                    Loggger.LogError("Error in UpdateRates repository :" + ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
            return Result;
        }
    }
}