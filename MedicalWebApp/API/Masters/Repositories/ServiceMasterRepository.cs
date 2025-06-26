using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CGHSBilling.API.Masters.Interfaces;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Caching;
using CGHSBilling.Models;
using CGHSBilling.QueryCollection.Masters;
using CommonDataLayer.DataAccess;
using CommonLayer;

namespace CGHSBilling.API.Masters.Repositories
{
    public class ServiceMasterRepository: IServiceMasterRepository
    {
        private static readonly ILogger Loggger = Logger.Register(typeof(ServiceMasterRepository));
        public ServiceMasterModel SaveService(ServiceMasterModel entity)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                IDbTransaction transaction = dbHelper.BeginTransaction();
                try
                {
                    var tempateResult = SaveServiceMaster(entity, dbHelper);
                    if (tempateResult.ServiceId > 0 && entity.ServiceGender != null && entity.ServiceGender.Count > 0)
                    {
                        foreach (var detail in entity.ServiceGender)
                        {
                            detail.ServiceId = entity.ServiceId;
                            CreateServiceGenderLinking(detail, dbHelper);
                        }
                        dbHelper.CommitTransaction(transaction);
                    }
                }
                catch (Exception ex)
                {
                    dbHelper.RollbackTransaction(transaction);
                    Loggger.LogError("Error in SaveServiceType :" + ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
            return entity;
        }

        public ServiceMasterModel UpdateService(ServiceMasterModel entity)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                IDbTransaction transaction = dbHelper.BeginTransaction();
                try
                {
                    var tempateResult = UpdateServiceMaster(entity, dbHelper);
                    if (entity.ServiceId > 0 && entity.ServiceGender != null && entity.ServiceGender.Count > 0)
                    {
                        foreach (var detail in entity.ServiceGender)
                        {
                            detail.ServiceId = entity.ServiceId;
                            CreateServiceGenderLinking(detail, dbHelper);
                        }
                        dbHelper.CommitTransaction(transaction);
                    }
                }
                catch (Exception ex)
                {
                    dbHelper.RollbackTransaction(transaction);
                    Loggger.LogError("Error in SaveServiceType :" + ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
            return entity;
        }
        public ServiceMasterModel SaveServiceMaster(ServiceMasterModel entity,DBHelper dbHelper)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ServiceId", entity.ServiceId, DbType.Int32, ParameterDirection.Output));
            paramCollection.Add(new DBParameter("ServiceTypeId", entity.ServiceTypeId, DbType.Int32));
            paramCollection.Add(new DBParameter("Code", entity.Code, DbType.String));
            paramCollection.Add(new DBParameter("ServiceName", entity.ServiceName, DbType.String));
            paramCollection.Add(new DBParameter("Sequence", entity.Sequence, DbType.Int32));
            paramCollection.Add(new DBParameter("CghsCode", entity.CghsCode, DbType.String));
            paramCollection.Add(new DBParameter("Deactive", entity.Deactive, DbType.Boolean));
            paramCollection.Add(new DBParameter("GenderId", entity.GenderId, DbType.Int32));
            paramCollection.Add(new DBParameter("PatientTypeId", entity.PatientTypeId, DbType.Int32));
            paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
            paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
            paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
            paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
            paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
            paramCollection.Add(new DBParameter("AllowToChangeRate", entity.AllowToChangeRate, DbType.Boolean));
            paramCollection.Add(new DBParameter("Default", entity.Default, DbType.Boolean));
            paramCollection.Add(new DBParameter("Qty", entity.Qty, DbType.Int32));
            paramCollection.Add(new DBParameter("Surgery", entity.Surgery, DbType.Boolean));
            paramCollection.Add(new DBParameter("NoOfDays", entity.NoOfDays, DbType.Int32));
            var param = dbHelper.ExecuteNonQueryForOutParameter<int>(MasterQueries.SaveServiceMaster, paramCollection, CommandType.StoredProcedure, "ServiceId");
            entity.ServiceId = param;
            return entity;
        }

        public ServiceMasterModel UpdateServiceMaster(ServiceMasterModel entity, DBHelper dbHelper)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ServiceId", entity.ServiceId, DbType.Int32));
            paramCollection.Add(new DBParameter("ServiceTypeId", entity.ServiceTypeId, DbType.Int32));
            paramCollection.Add(new DBParameter("Code", entity.Code, DbType.String));
            paramCollection.Add(new DBParameter("ServiceName", entity.ServiceName, DbType.String));
            paramCollection.Add(new DBParameter("Sequence", entity.Sequence, DbType.Int32));
            paramCollection.Add(new DBParameter("CghsCode", entity.CghsCode, DbType.String));
            paramCollection.Add(new DBParameter("Deactive", entity.Deactive, DbType.Boolean));
            paramCollection.Add(new DBParameter("GenderId", entity.GenderId, DbType.Int32));
            paramCollection.Add(new DBParameter("PatientTypeId", entity.PatientTypeId, DbType.Int32));
            paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
            paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
            paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
            paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
            paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
            paramCollection.Add(new DBParameter("AllowToChangeRate", entity.AllowToChangeRate, DbType.Boolean));
            paramCollection.Add(new DBParameter("Default", entity.Default, DbType.Boolean));
            paramCollection.Add(new DBParameter("Qty", entity.Qty, DbType.Int32));
            paramCollection.Add(new DBParameter("Surgery", entity.Surgery, DbType.Boolean));
            paramCollection.Add(new DBParameter("NoOfDays", entity.NoOfDays, DbType.Int32));
            dbHelper.ExecuteNonQuery(MasterQueries.UpdateServiceMaster, paramCollection, CommandType.StoredProcedure);
            return entity;
        }
        public void CreateServiceGenderLinking(ServiceGenderLinking entity, DBHelper dbHelper)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ServiceId", entity.ServiceId, DbType.Int32));
            paramCollection.Add(new DBParameter("GenderId", entity.Id, DbType.String));
            dbHelper.ExecuteNonQuery(MasterQueries.CreateServiceGenderLinking, paramCollection, CommandType.StoredProcedure);
        }
        public List<ServiceMasterModel> GetAllServiceMaster()
        {
            List<ServiceMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAllServiceMaster, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceMasterModel
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        Code = row.Field<string>("Code"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        CghsCode = row.Field<string>("CghsCode"),
                        Sequence = row.Field<int>("SequenceNo"),
                        GenderId = row.Field<int>("GenderId"),
                        PatientTypeId = row.Field<int>("PatientTypeId"),
                        Deactive = row.Field<bool>("Deactive"),
                        AllowToChangeRate = row.Field<bool>("AllowToChangeRate"),
                        Default = row.Field<bool>("Default"),
                        Qty = row.Field<int>("Qty"),
                        Surgery = row.Field<bool>("Surgery"),
                        NoOfDays = row.Field<int>("NoOfDays")
                    }).ToList();
            }
            return list;
        }
        public List<ServiceMasterModel> GetAllActiveDefaultServiceMaster()
        {
            List<ServiceMasterModel> list = null;
            if (MemoryCaching.CacheKeyExist(CachingKeys.DefaultServiceMasters.ToString()))
            {
                list = MemoryCaching.GetCacheValue(CachingKeys.DefaultServiceMasters.ToString()) as List<ServiceMasterModel>;
                if (list != null && list.Count > 0)
                    return list;
            }

            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAllActiveDefaultServiceMaster, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceMasterModel
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        Code = row.Field<string>("Code"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        CghsCode = row.Field<string>("CghsCode"),
                        Sequence = row.Field<int>("SequenceNo"),
                        GenderId = row.Field<int>("GenderId"),
                        PatientTypeId = row.Field<int>("PatientTypeId"),
                        Deactive = row.Field<bool>("Deactive"),
                        AllowToChangeRate = row.Field<bool>("AllowToChangeRate"),
                        Default = row.Field<bool>("Default"),
                        Qty = row.Field<int>("Qty"),
                        Surgery = row.Field<bool>("Surgery"),
                        NoOfDays = row.Field<int>("NoOfDays"),
                    }).ToList();
            }
            MemoryCaching.AddCacheValue(CachingKeys.DefaultServiceMasters.ToString(), list);
            return list;
        }

        public List<ServiceMasterModel> GetAllActiveDefaultServiceMaster_WithRates(int RoomTypeId, int HospitalTypeId, int PatientTypeId, int GenderId, int StateId, int CityId,string ConnectionString)
        {
            List<ServiceMasterModel> list = null;
            if (MemoryCaching.CacheKeyExist(CachingKeys.DefaultServiceMasters_Rates.ToString()))
            {
                list = MemoryCaching.GetCacheValue(CachingKeys.DefaultServiceMasters_Rates.ToString()) as List<ServiceMasterModel>;
                if (list != null && list.Count > 0
                    && list.Where(w => w.RoomTypeId == RoomTypeId && w.HospitalTypeId == HospitalTypeId && w.PatientTypeId == PatientTypeId && w.GenderId == GenderId && w.StateId == StateId && w.CityId == CityId).Any())
                    return list;
            }

            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("GenderId", GenderId, DbType.Int32));
                paramCollection.Add(new DBParameter("PatientTypeId", PatientTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("RoomTypeId", RoomTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("HospitalTypeId", HospitalTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("StateId", StateId, DbType.Int32));
                paramCollection.Add(new DBParameter("CityId", CityId, DbType.Int32));

                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAllActiveDefaultServiceMaster_Rates, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceMasterModel
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        Code = row.Field<string>("Code"),
                        CghsCode = row.Field<string>("CghsCode"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Qty = row.Field<int>("Qty"),
                        BillRate = row.Field<double>("BillRate"),
                    }).ToList();
            }
            MemoryCaching.AddCacheValue(CachingKeys.DefaultServiceMasters_Rates.ToString(), list);
            return list;
        }

        public List<ServiceGenderLinking> GetAllLinkedGenderByServiceId(int ServiceId)
        {
            List<ServiceGenderLinking> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ServiceId", ServiceId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAllLinkedGenderByServiceId, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceGenderLinking
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        Id = row.Field<int>("ID"),
                        Name = row.Field<string>("NAME"),
                        State = row.Field<bool>("State")
                    }).ToList();
            }
            return list;
        }

        public List<ServiceMasterModel> GetServicesByServiceTypeId(int ServiceTypeId)
        {
            List<ServiceMasterModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ServiceTypeId", ServiceTypeId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetServicesByServiceTypeId, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceMasterModel
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        Code = row.Field<string>("Code"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        CghsCode = row.Field<string>("CghsCode"),
                        Sequence = row.Field<int>("SequenceNo"),
                        GenderId = row.Field<int>("GenderId"),
                        PatientTypeId = row.Field<int>("PatientTypeId"),
                        Deactive = row.Field<bool>("Deactive"),
                        AllowToChangeRate = row.Field<bool>("AllowToChangeRate"),
                        Default = row.Field<bool>("Default"),
                        Surgery = row.Field<bool>("Surgery"),
                        NoOfDays = row.Field<int>("NoOfDays")
                    }).ToList();
            }
            return list;
        }
        public bool CheckDuplicateInsert(CheckDuplicateModel chkmodel)
        {
            bool IsDuplicate = false;
            object result;
            using (DBHelper dbHelper = new DBHelper())
            {
                result = dbHelper.ExecuteScalar("Select 1 from ServiceMaster where Code = '" + chkmodel.Code + "' OR ServiceName='" + chkmodel.Name + "'", CommandType.Text);
                if (Convert.ToInt32(result) > 0)
                {
                    IsDuplicate = true;
                }
            }
            return IsDuplicate;
        }
        public bool CheckDuplicateUpdate(CheckDuplicateModel chkmodel)
        {
            bool IsDuplicate = false;
            object result;
            using (DBHelper dbHelper = new DBHelper())
            {
                result = dbHelper.ExecuteScalar("Select 1 from ServiceMaster where ( Code = '" + chkmodel.Code + "' OR ServiceName='" + chkmodel.Name + "') AND ServiceId <> " + chkmodel.Id, CommandType.Text);
                if (Convert.ToInt32(result) > 0)
                {
                    IsDuplicate = true;
                }
            }
            return IsDuplicate;
        }
    }
}