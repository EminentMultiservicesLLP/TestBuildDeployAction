using CGHSBilling.API.AdminPanel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CGHSBilling.Areas.CommonArea.Models;
using CGHSBilling.QueryCollection.AdminPanel;
using CommonDataLayer.DataAccess;
using System.Data;

namespace CGHSBilling.API.AdminPanel.Repositories
{
    public class BillingConfigurationRepository : IBillingConfigurationRepository
    {
        public List<BillConfigurationModel> GetAllBill(int ClientId)
        {
            List<BillConfigurationModel> client = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ClientId", ClientId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtvendor = dbHelper.ExecuteDataTable(AdminPanelQueries.GetAllBill, paramCollection, CommandType.StoredProcedure);
                client = dtvendor.AsEnumerable()
                            .Select(row => new BillConfigurationModel
                            {
                                ClientId = row.Field<int>("ClientId"),
                                RecieveAmount = row.Field<double?>("RecieveAmount"),
                                BillConfigurationId = row.Field<int>("BillConfigurationId"),
                                ClientName = row.Field<string>("ClientName"),
                                BillDate = row.Field<string>("BillDate"),
                                Comment = row.Field<string>("Comment"),
                                DeductionModeId = row.Field<int>("DeductionModeId")
                               // NoOfBills = row.Field<int>("NoOfBills")
                            }).ToList();
            }
            return client;
        }

        public List<BillConfigurationDtlModel> GetAllDeductedBills(int ClientId)
        {
            List<BillConfigurationDtlModel> client = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("ClientId", ClientId, DbType.Int32));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtvendor = dbHelper.ExecuteDataTable(AdminPanelQueries.GetAllDeductedBills, paramCollection, CommandType.StoredProcedure);
                client = dtvendor.AsEnumerable()
                    .Select(row => new BillConfigurationDtlModel
                    {
                        BillConfigurationId = row.Field<int>("DeductionId"),
                        RequestNo = row.Field<string>("RequestNo"),
                        DeductedAmount = row.Field<double?>("DeductedAmount"),
                        BalanceAmount = row.Field<double?>("BalanceAmount"),
                        DeductionModeId = row.Field<int>("DeductionModeId")
                    }).ToList();
            }
            return client;
        }

        public bool SaveBillingConfiguration(BillConfigurationModel entity,string ConnectionString)
        {
            int iResult = 0;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("BillConfigurationId", entity.BillConfigurationId, DbType.Int32));
                paramCollection.Add(new DBParameter("ClientId", entity.ClientId, DbType.Int32));
                paramCollection.Add(new DBParameter("RecieveAmount", entity.RecieveAmount, DbType.Int32));
              //  paramCollection.Add(new DBParameter("BalanceAmt", entity.BalanceAmount, DbType.Double));
                paramCollection.Add(new DBParameter("BillDate", entity.BillDate, DbType.DateTime));
                paramCollection.Add(new DBParameter("Comment", entity.Comment, DbType.String));
                paramCollection.Add(new DBParameter("InsertedBy", entity.InsertedBy, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedMacName", entity.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedIPAddress", entity.InsertedIpAddress, DbType.String));
                paramCollection.Add(new DBParameter("InsertedON", entity.InsertedOn, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedMacID", entity.InsertedMacId, DbType.String));
                paramCollection.Add(new DBParameter("DeductionModeId", entity.DeductionModeId, DbType.Int32));
                iResult = dbHelper.ExecuteNonQuery(AdminPanelQueries.SaveBillingConfiguration, paramCollection, CommandType.StoredProcedure);
            }
            if (iResult > 0)
                return true;
            else
                return false;
        }

    }
}