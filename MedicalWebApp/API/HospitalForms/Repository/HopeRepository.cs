using CGHSBilling.API.HospitalForms.Intefaces;
using CGHSBilling.Areas.AdminPanel.Models;
using CGHSBilling.Areas.HospitalForms.Models;
using CGHSBilling.QueryCollection.HospitalForms;
using CommonDataLayer.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CGHSBilling.API.HospitalForms.Repository
{
    public class HopeRepository : IHopeRepository
    {
        public List<PatientModel> GetHopePatients(string ConnectionString)
        {
            List<PatientModel> list = null;
            if (Caching.MemoryCaching.CacheKeyExist(Caching.CachingKeys.HopePatients.ToString()))
            {
                list = Caching.MemoryCaching.GetCacheValue(Caching.CachingKeys.HopePatients.ToString()) as List<PatientModel>;
                if (list != null && list.Count() > 0)
                    return list;
            }
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DataTable dtPatients = dbHelper.ExecuteDataTable(HopeQueries.GetHopePatients, CommandType.StoredProcedure);
                list = dtPatients.AsEnumerable()
                    .Select(row => new PatientModel
                    {
                        PatientId = row.Field<int>("PatientId"),
                        RegName = row.Field<string>("RegName"),
                        ParentPatientId = row.Field<int>("ParentPatientId"),
                        IsDependent = row.Field<bool>("IsDependent"),
                        CompanyName = row.Field<string>("CompanyName"),
                        PatientName = row.Field<string>("PatientName"),
                        Age = row.Field<double>("Age"),
                        GenderId = row.Field<int>("GenderId"),
                        Address = row.Field<string>("Address"),
                        RoomTypeId = row.Field<int>("RoomTypeId"),
                    }).ToList();
            }
            Caching.MemoryCaching.AddCacheValue(Caching.CachingKeys.HopePatients.ToString(), list);
            return list;
        }

        public ClientMasterModel GetClientConfiguration(int UserId) {

            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("UserId", UserId, DbType.Int32));
            ClientMasterModel  ClientModel =  new ClientMasterModel();
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtconfig = dbHelper.ExecuteDataTable(HopeQueries.GetClientConfiguration,paramCollection, CommandType.StoredProcedure);
                DataRow row = dtconfig.Rows[0];
                {
                    ClientModel.IsHopeClient = row.Field<bool>("IsHopeClient");
                    ClientModel.IsBothClient = row.Field<bool>("IsBothClient");
                    ClientModel.IsShowLnk = row.Field<bool>("IsShowLnk");
                }
                    

            }
                return ClientModel;
        }
     }
}