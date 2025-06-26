using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CGHSBilling.API.AdminPanel.Interfaces;
using CGHSBilling.Areas.AdminPanel.Models;
using CGHSBilling.QueryCollection.AdminPanel;
using CommonDataLayer.DataAccess;
using CommonLayer.Extensions;
using CGHSBilling.QueryCollection.Masters;
using CGHSBilling.Common;

namespace CGHSBilling.API.AdminPanel.Repositories
{
    public class ClientMasterRepository : IClientMasterInterface
    {
        public int CreateClient(ClientMasterModel clientEntity, string ConnectionString)
        {
            try
            {
                int iResult = 0;

                using (DBHelper dbHelper = new DBHelper(ConnectionString))
                {
                    DBParameterCollection paramCollection = new DBParameterCollection();
                    paramCollection.Add(new DBParameter("ClientId", clientEntity.ClientId, DbType.Int32, ParameterDirection.Output));
                    paramCollection.Add(new DBParameter("ClientCode", clientEntity.Code, DbType.String));
                    paramCollection.Add(new DBParameter("ClientName", clientEntity.Name, DbType.String));
                    paramCollection.Add(new DBParameter("ExpiryDate", clientEntity.ExpiryDate, DbType.DateTime));
                    paramCollection.Add(new DBParameter("Address", clientEntity.Address, DbType.String));
                    paramCollection.Add(new DBParameter("InsertedBy", clientEntity.InsertedBy, DbType.Int32));
                    paramCollection.Add(new DBParameter("InsertedMacName", clientEntity.InsertedMacName, DbType.String));
                    paramCollection.Add(new DBParameter("InsertedIPAddress", clientEntity.InsertedIPAddress, DbType.String));
                    paramCollection.Add(new DBParameter("InsertedON", clientEntity.InsertedON, DbType.DateTime));
                    paramCollection.Add(new DBParameter("InsertedMacID", clientEntity.InsertedMacID, DbType.String));
                    paramCollection.Add(new DBParameter("City", clientEntity.City, DbType.Int32));
                    paramCollection.Add(new DBParameter("Pin", clientEntity.Pincode, DbType.String));
                    paramCollection.Add(new DBParameter("ContactPerson", clientEntity.ContactPerson, DbType.String));
                    paramCollection.Add(new DBParameter("ContactDesignation", clientEntity.ContactDesignation, DbType.String));
                    paramCollection.Add(new DBParameter("Fax", clientEntity.Fax, DbType.String));
                    paramCollection.Add(new DBParameter("Phone1", clientEntity.Phone1, DbType.String));
                    paramCollection.Add(new DBParameter("Phone2", clientEntity.Phone2, DbType.String));
                    paramCollection.Add(new DBParameter("CellPhone", clientEntity.CellPhone, DbType.String));
                    paramCollection.Add(new DBParameter("Email", clientEntity.Email, DbType.String));
                    paramCollection.Add(new DBParameter("Web", clientEntity.Web, DbType.String));
                    paramCollection.Add(new DBParameter("Society", clientEntity.Society, DbType.String));
                    paramCollection.Add(new DBParameter("Street", clientEntity.Street, DbType.String));
                    paramCollection.Add(new DBParameter("State", clientEntity.State, DbType.Int32));
                    paramCollection.Add(new DBParameter("Deactive", clientEntity.Deactive, DbType.Boolean));
                    paramCollection.Add(new DBParameter("landmark", clientEntity.Landmark, DbType.String));
                    paramCollection.Add(new DBParameter("GSTIN", clientEntity.GSTIN.NullToString(), DbType.String));
                    paramCollection.Add(new DBParameter("LogoPath", clientEntity.LogoPath, DbType.String));
                    paramCollection.Add(new DBParameter("DeductionAmt", clientEntity.DeductionAmt, DbType.Double));
                    paramCollection.Add(new DBParameter("HospitalTypeId", clientEntity.HospitalTypeId, DbType.Int32));
                    //paramCollection.Add(new DBParameter("NoOfBills", clientEntity.NoOfBills, DbType.Int32));
                    paramCollection.Add(new DBParameter("DeductionModeId", clientEntity.DeductionModeId, DbType.Int32));
                    paramCollection.Add(new DBParameter("HospitalServiceCategoryId", clientEntity.HospitalServiceCategoryId, DbType.Int32));
                    paramCollection.Add(new DBParameter("ClientTypeId", clientEntity.ClientTypeId, DbType.Int32));
                    paramCollection.Add(new DBParameter("IsShowLnk", clientEntity.IsShowLnk, DbType.Boolean));
                    iResult = dbHelper.ExecuteNonQueryForOutParameter<int>(AdminPanelQueries.InsertClient, paramCollection, CommandType.StoredProcedure, "ClientId");
                }
                return iResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateClient(ClientMasterModel clientEntity, string ConnectionString)
        {
            int iResult = 0;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("ClientId", clientEntity.ClientId, DbType.Int32));
                paramCollection.Add(new DBParameter("ClientCode", clientEntity.Code, DbType.String));
                paramCollection.Add(new DBParameter("ClientName", clientEntity.Name, DbType.String));
                paramCollection.Add(new DBParameter("ExpiryDate", clientEntity.ExpiryDate, DbType.DateTime));
                paramCollection.Add(new DBParameter("Address", clientEntity.Address, DbType.String));
                paramCollection.Add(new DBParameter("InsertedBy", clientEntity.InsertedBy, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedMacName", clientEntity.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedIPAddress", clientEntity.InsertedIPAddress, DbType.String));
                paramCollection.Add(new DBParameter("InsertedON", clientEntity.InsertedON, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedMacID", clientEntity.InsertedMacID, DbType.String));
                paramCollection.Add(new DBParameter("City", clientEntity.City, DbType.Int32));
                paramCollection.Add(new DBParameter("Pin", clientEntity.Pincode, DbType.String));
                paramCollection.Add(new DBParameter("ContactPerson", clientEntity.ContactPerson, DbType.String));
                paramCollection.Add(new DBParameter("ContactDesignation", clientEntity.ContactDesignation, DbType.String));
                paramCollection.Add(new DBParameter("Fax", clientEntity.Fax, DbType.String));
                paramCollection.Add(new DBParameter("Phone1", clientEntity.Phone1, DbType.String));
                paramCollection.Add(new DBParameter("Phone2", clientEntity.Phone2, DbType.String));
                paramCollection.Add(new DBParameter("CellPhone", clientEntity.CellPhone, DbType.String));
                paramCollection.Add(new DBParameter("Email", clientEntity.Email, DbType.String));
                paramCollection.Add(new DBParameter("Web", clientEntity.Web, DbType.String));
                paramCollection.Add(new DBParameter("Society", clientEntity.Society, DbType.String));
                paramCollection.Add(new DBParameter("Street", clientEntity.Street, DbType.String));
                paramCollection.Add(new DBParameter("State", clientEntity.State, DbType.Int32));
                paramCollection.Add(new DBParameter("HospitalTypeId", clientEntity.HospitalTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("Deactive", clientEntity.Deactive, DbType.Boolean));
                paramCollection.Add(new DBParameter("landmark", clientEntity.Landmark, DbType.String));
                paramCollection.Add(new DBParameter("GSTIN", clientEntity.GSTIN.NullToString(), DbType.String));
                paramCollection.Add(new DBParameter("LogoPath", clientEntity.LogoPath, DbType.String));
                paramCollection.Add(new DBParameter("DeductionAmt", clientEntity.DeductionAmt, DbType.Double));
                //paramCollection.Add(new DBParameter("NoOfBills", clientEntity.NoOfBills, DbType.Int32));
                paramCollection.Add(new DBParameter("DeductionModeId", clientEntity.DeductionModeId, DbType.Int32));
                paramCollection.Add(new DBParameter("HospitalServiceCategoryId", clientEntity.HospitalServiceCategoryId, DbType.Int32));
                paramCollection.Add(new DBParameter("ClientTypeId", clientEntity.ClientTypeId, DbType.Int32));
                paramCollection.Add(new DBParameter("IsShowLnk", clientEntity.IsShowLnk, DbType.Boolean));
                iResult = dbHelper.ExecuteNonQuery(AdminPanelQueries.InsertClient, paramCollection, CommandType.StoredProcedure);
            }
            if (iResult > 0)
                return true;
            else
                return false;
        }
        public IEnumerable<ClientMasterModel> GetAllClient()
        {
            List<ClientMasterModel> client = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtvendor = dbHelper.ExecuteDataTable(AdminPanelQueries.GetAllClient, CommandType.StoredProcedure);
                client = dtvendor.AsEnumerable()
                            .Select(row => new ClientMasterModel
                            {
                                ClientId = row.Field<int>("ClientId"),
                                Code = row.Field<string>("ClientCode"),
                                Name = row.Field<string>("ClientName"),
                                strExpiryDate = row.Field<DateTime?>("ExpiryDate")?.ToString("dd-MMMM-yyyy") ?? string.Empty,
                                Landmark = row.Field<string>("landmark"),
                                Address = row.Field<string>("Address"),
                                Street = row.Field<string>("Street"),
                                Society = row.Field<string>("Society"),
                                CityId = row.Field<int?>("City"),
                                CityName = row.Field<string>("CityName"),
                                Pincode = row.Field<string>("Pin"),
                                ContactPerson = row.Field<string>("ContactPerson"),
                                ContactDesignation = row.Field<string>("ContactDesignation"),
                                Fax = row.Field<string>("Fax"),
                                Phone1 = row.Field<string>("Phone1"),
                                Phone2 = row.Field<string>("Phone2"),
                                CellPhone = row.Field<string>("CellPhone"),
                                Email = row.Field<string>("Email"),
                                Deactive = row.Field<bool>("Deactive"),
                                State = row.Field<int?>("State"),
                                StateName = row.Field<string>("StateName"),
                                GSTIN = row.Field<string>("GSTIN"),
                                LogoPath = row.Field<string>("LogoPath"),
                                DeductionAmt = row.Field<double>("DeductionAmt"),
                                HospitalTypeId = row.Field<int?>("HospitalTypeId"),
                                // NoOfBills = row.Field<int?>("NoOfBills"),
                                DeductionModeId = row.Field<int?>("DeductionMode"),
                                DeductionType = row.Field<string>("DeductionType"),
                                HospitalServiceCategory = row.Field<string>("HospitalServiceCategory"),
                                HospitalServiceCategoryId = row.Field<int?>("HospitalServiceCategoryId"),
                                ClientTypeId = row.Field<int?>("ClientType"),
                                IsShowLnk = row.Field<bool>("IsShowLink"),
                            }).ToList();
            }
            return client;
        }
        public IEnumerable<ClientMasterModel> GetAllStates()
        {
            List<ClientMasterModel> state = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtvendor = dbHelper.ExecuteDataTable("select * from StateMaster", CommandType.Text);
                state = dtvendor.AsEnumerable()
                            .Select(row => new ClientMasterModel
                            {
                                StateId = row.Field<int>("StateId"),
                                StateName = row.Field<string>("StateName"),
                            }).ToList();
            }
            return state;
        }
        public IEnumerable<ClientMasterModel> GetHospitalServiceCategory()
        {
            List<ClientMasterModel> HospitalServiceCategory = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtvendor = dbHelper.ExecuteDataTable("select * from HospitalServiceCategory", CommandType.Text);
                HospitalServiceCategory = dtvendor.AsEnumerable()
                            .Select(row => new ClientMasterModel
                            {
                                HospitalServiceCategoryId = row.Field<int>("HospitalServiceCategoryId"),
                                HospitalServiceCategory = row.Field<string>("HospitalServiceCategory"),
                            }).ToList();
            }
            return HospitalServiceCategory;
        }
        public IEnumerable<ClientMasterModel> GetClientType()
        {
            List<ClientMasterModel> ClientType = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtvendor = dbHelper.ExecuteDataTable("select * from inv_Mst_ClientType", CommandType.Text);
                ClientType = dtvendor.AsEnumerable()
                            .Select(row => new ClientMasterModel
                            {
                                ClientTypeId = row.Field<int>("ClientTypeId"),
                                TypeName = row.Field<string>("TypeName"),
                            }).ToList();
            }
            return ClientType;
        }
        public IEnumerable<ClientMasterModel> GetCityById(int stateId)
        {
            List<ClientMasterModel> city = null;
            DBParameterCollection paramCollection = new DBParameterCollection();
            paramCollection.Add(new DBParameter("StateId", stateId, DbType.String));
            using (DBHelper dbHelper = new DBHelper())
            {
                DataTable dtvendor = dbHelper.ExecuteDataTable("select * from CityMaster where stateid=" + stateId, CommandType.Text);
                city = dtvendor.AsEnumerable()
                            .Select(row => new ClientMasterModel
                            {
                                CityId = row.Field<int>("CityId"),
                                CityName = row.Field<string>("CityName"),
                            }).ToList();
            }
            return city;
        }
        public bool CheckDuplicateItem(string typecode, int typeid, string type, string ConnectionString)
        {
            bool bResult = false;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("Type", type, DbType.String));
                paramCollection.Add(new DBParameter("Code", typecode, DbType.String));
                paramCollection.Add(new DBParameter("ID", typeid, DbType.String));
                paramCollection.Add(new DBParameter("IsExist", true, DbType.Boolean, ParameterDirection.Output));

                bResult = dbHelper.ExecuteNonQueryForOutParameter<bool>(MasterQueries.CheckDuplicateItem, paramCollection, CommandType.StoredProcedure, "IsExist");
            }
            return bResult;
        }
        public bool CheckDuplicateUpdate(int typeid, string typecode, int otherid, string type, string ConnectionString)
        {
            bool bResult = false;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("Type", type, DbType.String));
                paramCollection.Add(new DBParameter("ID", typeid, DbType.String));
                paramCollection.Add(new DBParameter("Code", typecode, DbType.String));
                paramCollection.Add(new DBParameter("OtherID", otherid, DbType.String));
                paramCollection.Add(new DBParameter("IsExist", true, DbType.Boolean, ParameterDirection.Output));

                bResult = dbHelper.ExecuteNonQueryForOutParameter<bool>(MasterQueries.CheckDuplicateUpdate, paramCollection, CommandType.StoredProcedure, "IsExist");
            }
            return bResult;
        }
        public int SaveClientConfiguration(ClientConfiguration model, string ConnectionString)
        {
            int iResult = 0;
            string ConfigList = CommonMethods.ToXML(model.ConfigList);

            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                try
                {
                    dbHelper.BeginTransaction(); // Start transaction

                    DBParameterCollection paramCollection = new DBParameterCollection();
                    paramCollection.Add(new DBParameter("iResult", iResult, DbType.Int32, ParameterDirection.Output));
                    paramCollection.Add(new DBParameter("ConfigId", model.ConfigId, DbType.String));
                    paramCollection.Add(new DBParameter("ConfigName", model.ConfigName, DbType.String));
                    paramCollection.Add(new DBParameter("ConfigList", ConfigList, DbType.Xml));
                    paramCollection.Add(new DBParameter("Deactive", model.Deactive, DbType.Boolean));
                    paramCollection.Add(new DBParameter("InsertedBy", model.InsertedBy, DbType.Int32));
                    paramCollection.Add(new DBParameter("InsertedON", model.InsertedON, DbType.DateTime));

                    iResult = dbHelper.ExecuteNonQueryForOutParameter<int>(AdminPanelQueries.SaveClientConfiguration, paramCollection, CommandType.StoredProcedure, "iResult");

                    dbHelper.CommitTransaction(); // Commit transaction
                    return iResult;
                }
                catch (Exception ex)
                {
                    dbHelper.RollbackTransaction(); // Rollback if error
                    throw;
                }
            }
        }
        public IEnumerable<ClientConfiguration> GetClientConfiguration(int? LoginId)
        {
            List<ClientConfiguration> client = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("LoginId", LoginId, DbType.Int32));
                DataTable dtvendor = dbHelper.ExecuteDataTable(AdminPanelQueries.GetClientConfiguration, paramCollection, CommandType.StoredProcedure);
                client = dtvendor.AsEnumerable()
                            .Select(row => new ClientConfiguration
                            {
                                ConfigId = row.Field<int>("ConfigId"),
                                ConfigName = row.Field<string>("ConfigName"),
                                ClientId = row.Field<int>("ClientId"),
                                ClientName = row.Field<string>("Clientname"),
                                Deactive = row.Field<bool>("Deactive"),
                            }).ToList();
            }
            return client;
        }
        public IEnumerable<ClientConfigurationDetails> GetClientConfigDetails(int? ConfigId)
        {
            List<ClientConfigurationDetails> client = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("ConfigId", ConfigId, DbType.Int32));
                DataTable dtvendor = dbHelper.ExecuteDataTable(AdminPanelQueries.GetClientConfigDetails, paramCollection, CommandType.StoredProcedure);
                client = dtvendor.AsEnumerable()
                            .Select(row => new ClientConfigurationDetails
                            {
                                ServiceTypeId = row.Field<int>("ServiceTypeId"),
                                ServiceType = row.Field<string>("ServiceType"),
                                Percentage = row.Field<decimal?>("Percentage")
                            }).ToList();
            }
            return client;
        }
    }
}