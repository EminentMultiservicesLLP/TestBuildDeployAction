using CGHSBilling.API.Masters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.Models;
using CommonDataLayer.DataAccess;
using System.Data;
using CGHSBilling.QueryCollection.Masters;
using CommonLayer;

namespace CGHSBilling.API.Masters.Repositories
{
    
    public class ServiceTypeMasterRepository : IServiceTypeMasterRepository
    {
        private static readonly ILogger Loggger = Logger.Register(typeof(ServiceTypeMasterRepository));
        public bool CheckDuplicateInsert(CheckDuplicateModel chkmodel)
        {
            bool IsDuplicate = false;
            object result;
            using (DBHelper dbHelper = new DBHelper())
            {
                result = dbHelper.ExecuteScalar("Select 1 from ServiceTypeMaster where Code = '"+ chkmodel.Code + "' OR ServiceType='"+ chkmodel.Name+"'", CommandType.Text);
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
                result = dbHelper.ExecuteScalar("Select 1 from ServiceTypeMaster where ( Code = '" + chkmodel.Code + "' OR ServiceType='" + chkmodel.Name + "') AND ServiceTypeId <> "+chkmodel.Id, CommandType.Text);
                if (Convert.ToInt32(result) > 0)
                {
                    IsDuplicate = true;
                }
            }
            return IsDuplicate;

        }

        public ServiceTypeMasterModel SaveServiceType(ServiceTypeMasterModel entity)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                IDbTransaction transaction = dbHelper.BeginTransaction();
                try
                {
                    var tempateResult = SaveServiceTypeMaster(entity, dbHelper);
                    if (tempateResult.ServiceTypeId > 0 && entity.Category != null && entity.Category.Count > 0)
                    {
                        foreach (var detail in entity.Category)
                        {
                            detail.ServiceTypeId = entity.ServiceTypeId;
                            CreateServiceCategoryLinking(detail, dbHelper);
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
        public ServiceTypeMasterModel UpdateServiceType(ServiceTypeMasterModel entity)
        {
            using (DBHelper dbHelper = new DBHelper())
            {
                IDbTransaction transaction = dbHelper.BeginTransaction();
                try
                {
                    var tempateResult = UpdateServiceTypeMaster(entity, dbHelper);
                    if (entity.ServiceTypeId > 0 && entity.Category != null && entity.Category.Count > 0)
                    {
                        foreach (var detail in entity.Category)
                        {
                            detail.ServiceTypeId = entity.ServiceTypeId;
                            CreateServiceCategoryLinking(detail, dbHelper);
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
        public ServiceTypeMasterModel SaveServiceTypeMaster(ServiceTypeMasterModel entity,DBHelper dbHelper)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ServiceTypeId", entity.ServiceTypeId, DbType.Int32, ParameterDirection.Output));
            paramCollection.Add(new DBParameter("Code", entity.Code, DbType.String));
            paramCollection.Add(new DBParameter("ServiceType", entity.ServiceType, DbType.String));
            paramCollection.Add(new DBParameter("Sequence", entity.Sequence, DbType.Int32));
            paramCollection.Add(new DBParameter("CategoryId", entity.CategoryId, DbType.Int32));
            paramCollection.Add(new DBParameter("Deactive", entity.Deactive, DbType.Boolean));
            paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
            paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
            paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
            paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
            paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
           var param= dbHelper.ExecuteNonQueryForOutParameter<int>(MasterQueries.SaveServiceType, paramCollection, CommandType.StoredProcedure, "ServiceTypeId");
            entity.ServiceTypeId = param;
            return entity;
        }

        public ServiceTypeMasterModel UpdateServiceTypeMaster(ServiceTypeMasterModel entity, DBHelper dbHelper)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ServiceTypeId", entity.ServiceTypeId, DbType.Int32));
            paramCollection.Add(new DBParameter("Code", entity.Code, DbType.String));
            paramCollection.Add(new DBParameter("ServiceType", entity.ServiceType, DbType.String));
            paramCollection.Add(new DBParameter("Sequence", entity.Sequence, DbType.Int32));
            paramCollection.Add(new DBParameter("CategoryId", entity.CategoryId, DbType.Int32));
            paramCollection.Add(new DBParameter("Deactive", entity.Deactive, DbType.Boolean));
            paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
            paramCollection.Add(new DBParameter("InsertedOn", entity.InsertedOn, DbType.DateTime));
            paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
            paramCollection.Add(new DBParameter("InsertedMacId", entity.InsertedMacId, DbType.String));
            paramCollection.Add(new DBParameter("InsertedIpAddress", entity.InsertedIpAddress, DbType.String));
            dbHelper.ExecuteNonQuery(MasterQueries.SaveServiceType, paramCollection, CommandType.StoredProcedure);
            return entity; 
        }
        public void CreateServiceCategoryLinking(ServiceCategoryLinkingModel entity, DBHelper dbHelper)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ServiceTypeId", entity.ServiceTypeId, DbType.Int32));
            paramCollection.Add(new DBParameter("CategoryId", entity.Id, DbType.String));
            dbHelper.ExecuteNonQuery(MasterQueries.CreateSrviceCategoryLinking, paramCollection, CommandType.StoredProcedure);
        }


        public List<ServiceTypeMasterModel> GetAllServiceTypeMaster()
        {
            List<ServiceTypeMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable("Select ServiceTypeId,Code,ServiceType,Sequence,Deactive,isnull(CategoryId,0)CategoryId" +
                                                              " from ServiceTypeMaster order by Sequence asc", CommandType.Text);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceTypeMasterModel
                    {
                        ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        Code = row.Field<string>("Code"),
                        ServiceType = row.Field<string>("ServiceType"),
                        Sequence = row.Field<int>("Sequence"),
                        CategoryId = row.Field<int>("CategoryId"),
                        Deactive = row.Field<bool>("Deactive")
                    }).ToList();
            }
            return list;
        }
        public List<CommonMasterModel> GetAllCategory()
        {
            List<CommonMasterModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable("select * from ServiceTypeCategory", CommandType.Text);
                list = dtPaper.AsEnumerable()
                    .Select(row => new CommonMasterModel
                    {
                        Id = row.Field<int>("ID"),
                        Name = row.Field<string>("NAME"),
                    }).ToList();
            }
            return list;
        }
        public List<ServiceCategoryLinkingModel> GetAllLinkedCategoryByTypeId(int ServiceTypeId)
        {
            List<ServiceCategoryLinkingModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ServiceTypeId", ServiceTypeId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAllLinkedCategoryByTypeId, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceCategoryLinkingModel
                    {
                        ServiceTypeId = row.Field<int>("ServiceTypeId"),
                        Id = row.Field<int>("ID"),
                        Name = row.Field<string>("NAME"),
                        State = row.Field<bool>("State")
                    }).ToList();
            }
            return list;
        }
    }
}