using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CGHSBilling.API.AdminPanel.Interfaces;
using CGHSBilling.Areas.AdminPanel.Models;
using CGHSBilling.QueryCollection.AdminPanel;
using CommonDataLayer.DataAccess;
using CGHSBilling.QueryCollection.Masters;

namespace CGHSBilling.API.AdminPanel.Repositories
{
    public class UserCreationRepository : IUserCreationInterface
    {

        public List<UserCreationModel> GetUserCode(string ConnectionString)
        {
            List<UserCreationModel> items = null;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {

                DataTable dtManufacturer = dbHelper.ExecuteDataTable(AdminPanelQueries.GetUserCode, CommandType.StoredProcedure);
                items = dtManufacturer.AsEnumerable()
                            .Select(row => new UserCreationModel
                            {
                                UserCode = row.Field<string>("UserCode"),

                            }).ToList();

            }
            return items;
        }

        public List<UserCreationModel> GetUserDetails(string ConnectionString)
        {
            List<UserCreationModel> items = null;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {

                DataTable dtManufacturer = dbHelper.ExecuteDataTable(AdminPanelQueries.GetUserDetails, CommandType.StoredProcedure);
                items = dtManufacturer.AsEnumerable()
                            .Select(row => new UserCreationModel
                            {
                                UserCode = row.Field<string>("UserCode"),
                                UserID = row.Field<int>("UserID"),
                                UserName = row.Field<string>("UserName"),
                                IsDeactive = row.Field<bool>("IsDeactive"),
                                LoginName = row.Field<string>("LoginName"),
                                Password = row.Field<string>("Password"),
                                ClientId = row.Field<int>("ClientId"),
                                ClientName = row.Field<string>("ClientName"),
                                EmailID = row.Field<string>("EmailID"),
                                strLastLoginDate = row.Field<DateTime?>("LastLoginDate")?.ToString("dd-MMMM-yyyy") ?? string.Empty,
                            }).ToList();

            }
            return items;
        }
        public UserCreationModel GetUserDetailsByUserId(int userId,string ConnectionString)
        {
            UserCreationModel items = null;
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("UserId", userId, DbType.Int32));
                DataTable dtManufacturer = dbHelper.ExecuteDataTable(AdminPanelQueries.GetUserDetailsByUserId, paramCollection, CommandType.StoredProcedure);
                items = dtManufacturer.AsEnumerable()
                            .Select(row => new UserCreationModel
                            {
                                UserCode = row.Field<string>("UserCode"),
                                UserID = row.Field<int>("UserID"),
                                UserName = row.Field<string>("UserName"),
                                IsDeactive = row.Field<bool>("IsDeactive"),
                                LoginName = row.Field<string>("LoginName"),
                                Password = row.Field<string>("Password"),
                                ClientId = row.Field<int>("ClientId"),
                                ClientName = row.Field<string>("ClientName"),
                            }).FirstOrDefault();

            }
            return items;
        }


        public int SaveUser(UserCreationModel Items, string ConnectionString)
        {
            int iResult = 0;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("UserID", Items.UserID, DbType.Int32));
                paramCollection.Add(new DBParameter("ClientId", Items.ClientId, DbType.Int32));
                paramCollection.Add(new DBParameter("UserCode", Items.UserCode, DbType.String));
                paramCollection.Add(new DBParameter("UserName", Items.UserName, DbType.String));
                paramCollection.Add(new DBParameter("LoginName", Items.LoginName, DbType.String));
                paramCollection.Add(new DBParameter("Password", Items.Password, DbType.String));
                paramCollection.Add(new DBParameter("InsertedBy", Items.InsertedBy, DbType.Int32));
                paramCollection.Add(new DBParameter("InsertedON", Items.InsertedON, DbType.DateTime));
                paramCollection.Add(new DBParameter("InsertedIPAddress", Items.InsertedIPAddress, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacName", Items.InsertedMacName, DbType.String));
                paramCollection.Add(new DBParameter("InsertedMacID", Items.InsertedMacID, DbType.String));
                paramCollection.Add(new DBParameter("IsDeactive", Items.IsDeactive, DbType.Boolean));
                paramCollection.Add(new DBParameter("EmailID", Items.EmailID, DbType.String));
                //iResult = dbHelper.ExecuteNonQueryForOutParameter<int>(AdminPanelQueries.SaveUser, paramCollection, CommandType.StoredProcedure);
                iResult = dbHelper.ExecuteNonQuery(AdminPanelQueries.SaveUser, paramCollection, CommandType.StoredProcedure);

            }
            return iResult;
        }

        public bool CheckDuplicateItem(string LoginName, string EmailID, int typeid, string type)
        {
            bool bResult = false;
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("Type", type, DbType.String));
                paramCollection.Add(new DBParameter("LoginName", LoginName, DbType.String));
                paramCollection.Add(new DBParameter("EmailID", EmailID, DbType.String));
                paramCollection.Add(new DBParameter("ID", typeid, DbType.String));
                paramCollection.Add(new DBParameter("IsExist", true, DbType.Boolean, ParameterDirection.Output));

                bResult = dbHelper.ExecuteNonQueryForOutParameter<bool>(MasterQueries.CheckDuplicateItem, paramCollection, CommandType.StoredProcedure, "IsExist");
            }
            return bResult;
        }

        public bool CheckDuplicateUpdate(string EmailID, string LoginName, int typeid, string type)
        {
            bool bResult = false;
            using (DBHelper dbHelper = new DBHelper())
            {
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(new DBParameter("Type", type, DbType.String));
                paramCollection.Add(new DBParameter("EmailID", EmailID, DbType.String));
                paramCollection.Add(new DBParameter("LoginName", LoginName, DbType.String));
                paramCollection.Add(new DBParameter("ID", typeid, DbType.String));
                paramCollection.Add(new DBParameter("IsExist", true, DbType.Boolean, ParameterDirection.Output));

                bResult = dbHelper.ExecuteNonQueryForOutParameter<bool>(MasterQueries.CheckDuplicateUpdate, paramCollection, CommandType.StoredProcedure, "IsExist");
            }
            return bResult;
        }
        public List<UserCreationModel> GetClientTracker(string ConnectionString)
        {
            List<UserCreationModel> items = null;
            using (DBHelper dbHelper = new DBHelper(ConnectionString))
            {

                DataTable dtManufacturer = dbHelper.ExecuteDataTable(AdminPanelQueries.GetClientTracker, CommandType.StoredProcedure);
                items = dtManufacturer.AsEnumerable()
                            .Select(row => new UserCreationModel
                            {
                                UserID = row.Field<int>("UserID"),
                                UserCode = row.Field<string>("UserCode"),
                                UserName = row.Field<string>("UserName"),
                                LoginName = row.Field<string>("LoginName"),
                                ClientId = row.Field<int>("ClientId"),
                                ClientName = row.Field<string>("ClientName"),
                                strLastLoginDate = row.Field<DateTime?>("LastLoginDate")?.ToString("dd-MMM-yyyy") ?? string.Empty,
                                strExpiryDate = row.Field<DateTime?>("ExpiryDate")?.ToString("dd-MMM-yyyy") ?? string.Empty,
                            }).ToList();

            }
            return items;
        }
    }
}