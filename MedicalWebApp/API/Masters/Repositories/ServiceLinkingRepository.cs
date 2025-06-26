using CGHSBilling.API.Masters.Interfaces;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.QueryCollection.Masters;
using CommonDataLayer.DataAccess;
using CommonLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CGHSBilling.API.Masters.Repositories
{
    public class ServiceLinkingRepository: IServiceLinkingInterface
    {
        private static readonly ILogger Loggger = Logger.Register(typeof(ServiceLinkingRepository));


        public List<HospitalServicelinkingModel> GetAllManagementType()
        {
            List<HospitalServicelinkingModel> list = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable("Select ManagementTypeId,ManagementType" +
                                                              " from ManagementTypeMaster", CommandType.Text);
                list = dtPaper.AsEnumerable()
                    .Select(row => new HospitalServicelinkingModel
                    {
                        ManagementTypeId = row.Field<int>("ManagementTypeId"),
                        ManagementType = row.Field<string>("ManagementType"),
                      
                    }).ToList();
            }
            return list;
        }


        public HospitalServicelinkingModel SaveServiceTypeManagementtypeLinking(HospitalServicelinkingModel entity)
        {
            using (DBHelper dbHelper = new DBHelper())
            {

                try
                {
                    var IsFirstCall = true;
                    foreach (var detail in entity.ServiceType_ManagementTypeData)
                    {
                        detail.HospitalServiceCategoryId = entity.HospitalServiceCategoryId;
                        detail.HospitalServiceCategory = entity.HospitalServiceCategory;
                        SaveServiceTypeManagementtypeLinking(detail, dbHelper,IsFirstCall);
                        IsFirstCall = false;
                    }
                }
                
                catch (Exception ex)
                {

                    Loggger.LogError("Error in SaveServiceType :" + ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
            return entity;
        }


        public void SaveServiceTypeManagementtypeLinking(HospitalServicelinkingModel entity, DBHelper dbHelper,bool IsFirstCall)
        {
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("HospitalServiceCategoryId", entity.HospitalServiceCategoryId, DbType.Int32));
            paramCollection.Add(new DBParameter("ManagementTypeId", entity.ManagementTypeId, DbType.String));
            paramCollection.Add(new DBParameter("IsFirstCall", IsFirstCall, DbType.String));
            dbHelper.ExecuteNonQuery(MasterQueries.SaveServiceTypeManagementtypeLinking, paramCollection, CommandType.StoredProcedure);
        }

        public List<HospitalServicelinkingModel> GetAllLinkedRecordById(int HospitalServiceCategoryId)
        {
            List<HospitalServicelinkingModel> list = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("HospitalServiceCategoryId", HospitalServiceCategoryId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtPaper = dbHelper.ExecuteDataTable(MasterQueries.GetAllLinkedRecordById, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new HospitalServicelinkingModel
                    {
                        ManagementTypeId = row.Field<int>("ManagementTypeId"),
                        ManagementType = row.Field<string>("ManagementType"),
                        State = row.Field<bool>("State")
                    }).ToList();
            }
            return list;
        }

    }
}