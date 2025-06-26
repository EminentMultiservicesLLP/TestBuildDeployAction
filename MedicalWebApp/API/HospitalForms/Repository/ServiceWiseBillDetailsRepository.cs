using CGHSBilling.API.HospitalForms.Intefaces;
using CGHSBilling.Areas.HospitalForms.Models;
using CGHSBilling.Areas.Masters.Models;
using CGHSBilling.QueryCollection.HospitalForms;
using CommonDataLayer.DataAccess;
using CommonLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CGHSBilling.API.HospitalForms.Repository
{
    public class ServiceWiseBillDetailsRepository : IServiceWiseBillDetailsRepository
    {
        private static readonly ILogger _logger = Logger.Register(typeof(ServiceWiseBillDetailsRepository));

        public List<ServiceWiseBillDetailsModel> GetServiceMasterList()
        {
            List<ServiceWiseBillDetailsModel> list = null;

            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetServiceMasterList, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new ServiceWiseBillDetailsModel
                    {
                        ServiceId = row.Field<int>("ServiceId"),
                        ServiceName = row.Field<string>("ServiceName"),

                    }).ToList();
            }

            return list;
        }

        public List<CommonMasterModel> GetAllCategories()
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

        public List<ServiceWiseBillDetailsModel> GetAllServiceWiseBillDtls_Datewise(string Fromtime, string Totime, int ServiceId, string ConnectionString, int CategoryId, int UserId, int BillTypeId)
        {
            List<ServiceWiseBillDetailsModel> result = new List<ServiceWiseBillDetailsModel>();
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("Fromtime", Fromtime, DbType.String));
            paramCollection.Add(new DBParameter("Totime", Totime, DbType.String));
            paramCollection.Add(new DBParameter("ServiceId", ServiceId, DbType.Int32));
            paramCollection.Add(new DBParameter("CategoryId", CategoryId, DbType.Int32));
            paramCollection.Add(new DBParameter("InsertedBy", UserId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                string SP = BillTypeId == 0 ? HospitalFormsQueries.GetAllServiceWiseBillDtls_Datewise : HospitalFormsQueries.GetAllOPDServiceWiseBillDtls_Datewise;

                DataTable dtPaper = dbHelper.ExecuteDataTable(SP, paramCollection, CommandType.StoredProcedure);

                result = dtPaper.AsEnumerable()
                  .Select(row => new ServiceWiseBillDetailsModel
                  {
                      ServiceName = row.Field<string>("ServiceName"),
                      FromDate = row.Field<string>("FromDate"),
                      ToDate = row.Field<string>("ToDate"),
                      BillDate = row.Field<string>("BillDate"),
                      BillNo = row.Field<string>("BillNo"),
                      PatientName = row.Field<string>("PatientName"),
                      Quantity = row.Field<int>("Quantity"),
                      CategoryName = row.Field<string>("CategoryName"),
                      BillRate = row.Field<double>("BillRate"),
                      Amount = row.Field<double>("Amount")

                  }).ToList();
            }
            return result;
        }


        public List<RequestSubmissionBillNoModel> GetAllBillNo(int userid)
        {
            List<RequestSubmissionBillNoModel> list = null;

            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("UserId", userid, DbType.Int32));
                DataTable dtPaper = dbHelper.ExecuteDataTable(HospitalFormsQueries.GetAllBillNo, paramCollection, CommandType.StoredProcedure);
                list = dtPaper.AsEnumerable()
                    .Select(row => new RequestSubmissionBillNoModel
                    {
                        RequestNo = row.Field<string>("RequestNo"),
                        RequestId = row.Field<int>("RequestId"),
                    }).ToList();
            }

            return list;
        }
    }
}