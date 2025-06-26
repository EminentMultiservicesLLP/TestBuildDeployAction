using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGHSBilling.API.Masters.Interfaces;
using CGHSBilling.Areas.Masters.Models;
using CommonDataLayer.DataAccess;
using System.Data;
using CGHSBilling.Areas.Masters.Controllers;
using CGHSBilling.QueryCollection.Masters;
using CommonLayer;
using CommonLayer.Extensions;

namespace CGHSBilling.API.Masters.Repositories
{
    public class TariffMasterRepository : ITariffMasterRepository
    {
        private static readonly ILogger Loggger = Logger.Register(typeof(TariffMasterRepository));
        public TariffMasterModel SaveTariff(TariffMasterModel entity)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                IDbTransaction transaction = dbHelper.BeginTransaction();
                try
                {
                    var tempateResult = CreatetariffMaster(entity, dbHelper);
                    if (tempateResult.TariffMasterId > 0 && entity.Tariffdtl!=null && entity.Tariffdtl.Count>0)
                    {
                        foreach (var detail in entity.Tariffdtl)
                        {
                            detail.TariffMasterId = entity.TariffMasterId;
                            detail.InsertedBy = entity.InsertedBy;
                            detail.InsertedOn = entity.InsertedOn;
                            detail.InsertedMacName = entity.InsertedMacName;
                            detail.InsertedMacId = entity.InsertedMacId;
                            detail.InsertedIpAddress = entity.InsertedIpAddress;
                            CreatetariffDetail(detail, dbHelper);
                        }
                        dbHelper.CommitTransaction(transaction);
                    }
                   
                }
                catch (Exception ex)
                {
                    dbHelper.RollbackTransaction(transaction);
                    Loggger.LogError("Error in Savesertariff :" + ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
            return entity;
        }
        public TariffMasterModel UpdateTariffMaster(TariffMasterModel entity)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                IDbTransaction transaction = dbHelper.BeginTransaction();
                try
                {
                    var tempateResult = UpdateTariffMaster(entity, dbHelper);
                    if (tempateResult.TariffMasterId > 0 && entity.Tariffdtl != null && entity.Tariffdtl.Count > 0)
                    {
                        foreach (var detail in entity.Tariffdtl)
                        {
                            detail.TariffMasterId = entity.TariffMasterId;
                            detail.InsertedBy = entity.InsertedBy;
                            detail.InsertedOn = entity.InsertedOn;
                            detail.InsertedMacName = entity.InsertedMacName;
                            detail.InsertedMacId = entity.InsertedMacId;
                            detail.InsertedIpAddress = entity.InsertedIpAddress;
                            CreatetariffDetail(detail, dbHelper);
                        }
                        dbHelper.CommitTransaction(transaction);
                    }

                }
                catch (Exception ex)
                {
                    dbHelper.RollbackTransaction(transaction);
                    Loggger.LogError("Error in Savesertariff :" + ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
            return entity;
        }

        public TariffMasterModel CreatetariffMaster(TariffMasterModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("TariffMasterId", entity.TariffMasterId, DbType.Int32, ParameterDirection.Output));
                paramCollection.Add(new DBParameter("StateId", entity.StateId, DbType.Int32));
                paramCollection.Add(new DBParameter("CityId", entity.CityId, DbType.Int32));
                paramCollection.Add(new DBParameter("PatientTypeId", entity.PatientTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("RoomTypeId", entity.RoomTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("Sequence", entity.Sequence, DbType.Int32));
                paramCollection.Add(new DBParameter("Deactive", entity.Deactive, DbType.Boolean));
                paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
                var parameterList = dbHelper.ExecuteNonQueryForOutParameter<int>(MasterQueries.CreateTariffMaster, paramCollection, CommandType.StoredProcedure, "TariffMasterId");
                entity.TariffMasterId = parameterList;
            }).IfNotNull(ex => { throw (ex); });
            return entity;
        }
        public TariffMasterModel UpdateTariffMaster(TariffMasterModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("TariffMasterId", entity.TariffMasterId, DbType.Int32));
                paramCollection.Add(new DBParameter("StateId", entity.StateId, DbType.Int32));
                paramCollection.Add(new DBParameter("CityId", entity.CityId, DbType.Int32));
                paramCollection.Add(new DBParameter("PatientTypeId", entity.PatientTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("RoomTypeId", entity.RoomTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("Sequence", entity.Sequence, DbType.Int32));
                paramCollection.Add(new DBParameter("Deactive", entity.Deactive, DbType.Boolean));
                paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
                dbHelper.ExecuteNonQuery(MasterQueries.UpdateTariffMaster, paramCollection, CommandType.StoredProcedure);
            }).IfNotNull(ex => { throw (ex); });
            return entity;
        }

        public TariffDetailModel CreatetariffDetail(TariffDetailModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("TariffMasterId", entity.TariffMasterId, DbType.Int32));
                paramCollection.Add(new DBParameter("ServiceId", entity.ServiceId, DbType.Int32));
                paramCollection.Add(new DBParameter("NABHRate", entity.NABHRate, DbType.Double));
                paramCollection.Add(new DBParameter("NonNABHRate", entity.NonNABHRate, DbType.Double));
                paramCollection.Add(new DBParameter("AimsRate", entity.AIMSRate, DbType.Double));
                paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
                dbHelper.ExecuteNonQuery(MasterQueries.CreateTariffDetails, paramCollection, CommandType.StoredProcedure);
            }).IfNotNull(ex => { throw (ex); });
            return entity;
        }

        public List<TariffMasterModel> GetTariffMaster()
        {
            List<TariffMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper =dbHelper.ExecuteDataTable(MasterQueries.GetTariffMaster, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new TariffMasterModel
                    {
                        TariffMasterId = row.Field<int>("TariffMasterId"),
                        StateId = row.Field<int>("StateId"),
                        CityId = row.Field<int>("CityId"),
                        RoomTypeId = row.Field<int>("RoomTypeId"),
                        PatientTypeId = row.Field<int>("PatientTypeId"),
                        StateName = row.Field<string>("StateName"),
                        CityName = row.Field<string>("CityName"),
                        RoomType = row.Field<string>("RoomType"),
                        PatientType = row.Field<string>("PatientType"),
                    }).ToList();
            }
            return list;
        }

        public List<TariffDetailModel> GetTariffDetailById(int tariffMasterId)
        {
            List<TariffDetailModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("TariffMasterId", tariffMasterId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetTariffDetailById, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new TariffDetailModel
                    {
                        TariffMasterId = row.Field<int>("TariffMasterId"),
                        TariffDetailId = row.Field<int>("TariffDetailId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        NABHRate = row.Field<double>("NABHRate"),
                        NonNABHRate = row.Field<double>("NonNABHRate"),
                        AIMSRate = row.Field<double>("AimsRate"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Code = row.Field<string>("Code"),
                    }).ToList();
            }
            return list;
        }

        public List<TariffDetailModel> GetTariffMasterforCopy(int stateId, int cityId, int patientType, int roomType)
        {
            List<TariffDetailModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("stateId", stateId, DbType.Int32));
            paramCollection.Add(new DBParameter("cityId", cityId, DbType.Int32));
            paramCollection.Add(new DBParameter("patientType", patientType, DbType.Int32));
            paramCollection.Add(new DBParameter("roomType", roomType, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetTariffMasterforCopy, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new TariffDetailModel
                    {
                        TariffMasterId = row.Field<int>("TariffMasterId"),
                        TariffDetailId = row.Field<int>("TariffDetailId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        NABHRate = row.Field<double>("NABHRate"),
                        NonNABHRate = row.Field<double>("NonNABHRate"),
                        AIMSRate = row.Field<double>("AimsRate"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Code = row.Field<string>("Code"),
                        RoomTypeId = row.Field<int>("RoomTypeId"),
                    }).ToList();
            }
            return list;
        }

        public List<TariffDetailModel> GetTariff_forCopy(int stateId, int cityId, int patientType, int roomType)
        {
            List<TariffDetailModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("stateId", stateId, DbType.Int32));
            paramCollection.Add(new DBParameter("cityId", cityId, DbType.Int32));
            paramCollection.Add(new DBParameter("patientType", patientType, DbType.Int32));
            paramCollection.Add(new DBParameter("roomType", roomType, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetTariff_forCopy, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new TariffDetailModel
                    {
                        TariffMasterId = row.Field<int>("TariffMasterId"),
                    }).ToList();
            }
            return list;
        }

        public List<TariffDetailModel> GetTariffDetails_forCopy(string TariffMasterIds)
        {
            List<TariffDetailModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("TariffMasterIds", TariffMasterIds, DbType.String));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetTariffDetails_forCopy, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new TariffDetailModel
                    {
                        TariffMasterId = row.Field<int>("TariffMasterId"),
                        TariffDetailId = row.Field<int>("TariffDetailId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        NABHRate = row.Field<double>("NABHRate"),
                        NonNABHRate = row.Field<double>("NonNABHRate"),
                        AIMSRate = row.Field<double>("AimsRate"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceName = row.Field<string>("ServiceName"),
                        Code = row.Field<string>("Code"),
                        RoomTypeId = row.Field<int>("RoomTypeId"),
                    }).ToList();
            }
            return list;
        }
    }
}