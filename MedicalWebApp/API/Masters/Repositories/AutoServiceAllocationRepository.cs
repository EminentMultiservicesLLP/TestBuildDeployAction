using CGHSBilling.API.Masters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGHSBilling.Areas.Masters.Models;
using CommonDataLayer.DataAccess;
using CommonLayer;
using System.Data;
using CGHSBilling.QueryCollection.Masters;
using CommonLayer.Extensions;

namespace CGHSBilling.API.Masters.Repositories
{
    public class AutoServiceAllocationRepository : IAutoServiceAllocationRepository
    {
        private static readonly ILogger Loggger = Logger.Register(typeof(AutoServiceAllocationRepository));
        public AutoServiceAllocationModel SaveAutoServiceAllocation(AutoServiceAllocationModel entity)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                IDbTransaction transaction = dbHelper.BeginTransaction();
                try
                {
                    var tempateResult = CreateAutoServiceAllocationMaster(entity, dbHelper);
                    if (tempateResult.AutoAllocationId > 0 && entity.AllocationDtl != null && entity.AllocationDtl.Count > 0)
                    {
                        foreach (var detail in entity.AllocationDtl)
                        {
                            detail.AutoAllocationId = entity.AutoAllocationId;
                            CreateAutoServiceAllocationDtl(detail, dbHelper);
                        }
                        dbHelper.CommitTransaction(transaction);
                    }

                }
                catch (Exception ex)
                {
                    dbHelper.RollbackTransaction(transaction);
                    Loggger.LogError("Error in SaveAutoServiceAllocation :" + ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
            return entity;
        }

        public AutoServiceAllocationModel CreateAutoServiceAllocationMaster(AutoServiceAllocationModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("AutoAllocationId", entity.AutoAllocationId, DbType.Int32, ParameterDirection.Output));
                paramCollection.Add(new DBParameter("ServiceTypeId", entity.ServiceTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("ServiceId", entity.ServiceId, DbType.Int32));
                paramCollection.Add(new DBParameter("ServiceDaysId", entity.ServiceDaysId, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
                var parameterList = dbHelper.ExecuteNonQueryForOutParameter<int>(MasterQueries.CreateAutoServiceAllocationMaster, paramCollection, CommandType.StoredProcedure, "AutoAllocationId");
                entity.AutoAllocationId = parameterList;
            }).IfNotNull(ex => { throw (ex); });
            return entity;
        }
        public AutoServiceAllocationDtlModel CreateAutoServiceAllocationDtl(AutoServiceAllocationDtlModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("AutoAllocationId", entity.AutoAllocationId, DbType.Int32));
                paramCollection.Add(new DBParameter("LinkedWithServiceTypeId", entity.LinkedWithServiceTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("ServiceId", entity.ServiceId, DbType.Int32));
                dbHelper.ExecuteNonQuery(MasterQueries.CreateAutoServiceAllocationDtl, paramCollection, CommandType.StoredProcedure);
            }).IfNotNull(ex => { throw (ex); });
            return entity;
        }
        public AutoServiceAllocationModel UpdateAutoServiceAllocation(AutoServiceAllocationModel entity)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                IDbTransaction transaction = dbHelper.BeginTransaction();
                try
                {
                    var tempateResult = UpdateAutoServiceAllocationMaster(entity, dbHelper);
                    if (entity.AutoAllocationId > 0 && entity.AllocationDtl != null && entity.AllocationDtl.Count > 0)
                    {
                        foreach (var detail in entity.AllocationDtl)
                        {
                            detail.AutoAllocationId = entity.AutoAllocationId;
                            CreateAutoServiceAllocationDtl(detail, dbHelper);
                        }
                        dbHelper.CommitTransaction(transaction);
                    }

                }
                catch (Exception ex)
                {
                    dbHelper.RollbackTransaction(transaction);
                    Loggger.LogError("Error in UpdateAutoServiceAllocation :" + ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
            return entity;
        }

        public AutoServiceAllocationModel UpdateAutoServiceAllocationMaster(AutoServiceAllocationModel entity, DBHelper dbHelper)
        {
            TryCatch.Run(() =>
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("AutoAllocationId", entity.AutoAllocationId, DbType.Int32));
                paramCollection.Add(new DBParameter("ServiceTypeId", entity.ServiceTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("ServiceDaysId", entity.ServiceDaysId, DbType.Int32));
                paramCollection.Add(new DBParameter("ServiceId", entity.ServiceId, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
                dbHelper.ExecuteNonQuery(MasterQueries.UpdateAutoServiceAllocationDtl, paramCollection, CommandType.StoredProcedure);
            }).IfNotNull(ex => { throw (ex); });
            return entity;
        }
        public List<AutoServiceAllocationModel> GetAutoServiceAllocation()
        {
            List<AutoServiceAllocationModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAutoServiceAllocation, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new AutoServiceAllocationModel
                    {
                        AutoAllocationId = row.Field<int>("AutoAllocationId"),
                        ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        ServiceType = row.Field<string>("ServiceType"),
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        ServiceDaysId = row.Field<int>("ServiceDaysId"),
                    }).ToList();
            }
            return list;
        }

        public List<AutoServiceAllocationDtlModel> GetAutoServiceAllocationDetailById(int AutoAllocationId, int ServiceTypeId)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("AutoAllocationId", AutoAllocationId, DbType.Int32));
            paramCollection.Add(new DBParameter("ServiceTypeId", ServiceTypeId, DbType.Int32));
            List<AutoServiceAllocationDtlModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAutoServiceAllocationDetailById,paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new AutoServiceAllocationDtlModel
                    {
                        AutoAllocationId = row.Field<int>("AutoAllocationId"),
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        State = row.Field<bool>("State")
                    }).ToList();
            }
            return list;
        }

        public List<AutoServiceAllocationDtlModel> GetLinkedServicesByServiceType_ServiceId(int AutoAllocationId,int ServiceTypeId,int ServiceId)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("AutoAllocationId", AutoAllocationId, DbType.Int32));
            paramCollection.Add(new DBParameter("ServiceTypeId", ServiceTypeId, DbType.Int32));
            paramCollection.Add(new DBParameter("ServiceId", ServiceId, DbType.Int32));
            List<AutoServiceAllocationDtlModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetLinkedServicesByServiceType_ServiceId,paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new AutoServiceAllocationDtlModel
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        MasterServiceName = row.Field<string>("MasterServiceName"),
                        AllowToChangeRate = row.Field<bool>("AllowToChangeRate"),
                        BillRate = row.Field<double>("BillRate"),
                        State = row.Field<bool>("State"),
                        Qty = row.Field<int>("Qty"),
                    }).ToList();
            }
            return list;
        }

        public List<AutoServiceAllocationDtlModel> GetLinkedServicesByServiceTypeServiceId(int ServiceId, int hospitalType, int patientType, int stateId, int cityId, int gender)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ServiceId", ServiceId, DbType.Int32));
            paramCollection.Add(new DBParameter("hospitalType", hospitalType, DbType.Int32));
            paramCollection.Add(new DBParameter("patientType", patientType, DbType.Int32));
            paramCollection.Add(new DBParameter("stateId", stateId, DbType.Int32));
            paramCollection.Add(new DBParameter("cityId", cityId, DbType.Int32));
            paramCollection.Add(new DBParameter("gender", gender, DbType.Int32));
            List<AutoServiceAllocationDtlModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetLinkedServicesByServiceTypeServiceId, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new AutoServiceAllocationDtlModel
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),
                        State = row.Field<bool>("State"),
                        BillRate = row.Field<double>("BillRate"),
                        AllowToChangeRate = row.Field<bool>("AllowToChangeRate")
                    }).ToList();
            }
            return list;
        }

        public List<CommonMasterModel> GetDaysDropdwon()
        {
            List<CommonMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable("Select * from AutoAllocatedDate", CommandType.Text);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        Id = row.Field<int>("Id"),
                        Name = row.Field<string>("Name"),
                    }).ToList();
            }
            return list;
        }
    }
}